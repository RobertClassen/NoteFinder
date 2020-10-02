namespace Todo
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using UnityEditor;
	using UnityEditorInternal;
	using UnityEngine;

	public class ToDoEditor : EditorWindow
	{
		#region Constants
		private const float sidebarWidth = 150f;
		#endregion

		#region Fields
		private FileSystemWatcher watcher = null;
		private FileInfo[] fileInfos = null;
		private NoteList noteList = null;

		private string searchString = string.Empty;

		private string newTagName = string.Empty;

		private Vector2 sidebarScrollPosition = Vector2.zero;
		private Vector2 mainAreaScrollPosition = Vector2.zero;

		private int currentTag = -1;
		private Note[] entriesToShow = null;
		#endregion

		#region Properties
		public string SearchString
		{
			get { return searchString; }
			set
			{
				if(value != searchString)
				{
					searchString = value;
					RefreshEntriesToShow();
				}
			}
		}
		#endregion

		#region Constructors
		[MenuItem("Tools/TODO Manager")]
		public static void Init()
		{
			ToDoEditor window = GetWindow<ToDoEditor>("//TODO");
			window.minSize = new Vector2(400, 250);
			window.Show();
		}
		#endregion

		#region Methods
		void OnEnable()
		{
			if(EditorApplication.isPlayingOrWillChangePlaymode)
			{
				return;
			}

			RefreshFiles();

			NoteList[] noteLists = Resources.FindObjectsOfTypeAll<NoteList>();
			if(noteLists != null && noteLists.Length > 0)
			{
				noteList = noteLists[0];
			}
			RefreshEntriesToShow();

			watcher = new FileSystemWatcher(Application.dataPath, "*.cs");
			watcher.Created += OnCreated;
			watcher.Changed += OnChanged;
			watcher.Renamed += OnRenamed;
			watcher.Deleted += OnDeleted;

			watcher.EnableRaisingEvents = true;
			watcher.IncludeSubdirectories = true;
		}

		void OnGUI()
		{
			if(noteList == null)
			{
				GUILayout.Label("No data loaded", EditorStyles.centeredGreyMiniLabel);
				return;
			}

			Undo.RecordObject(noteList, "tododata");

			DrawToolbar();
			using(new GUILayout.HorizontalScope())
			{
				DrawSidebar();
				DrawMainArea();
			}

			EditorUtility.SetDirty(noteList);
		}

		#region GUI
		private void DrawToolbar()
		{
			using(new GUILayout.HorizontalScope(EditorStyles.toolbar))
			{
				if(GUILayout.Button("Scan", EditorStyles.toolbarButton))
				{
					ScanAllFiles();
				}
				GUILayout.FlexibleSpace();
				SearchString = DrawSearchField(SearchString);
			}
		}

		private void DrawSidebar()
		{
			using(new GUILayout.VerticalScope(GUI.skin.box, GUILayout.Width(sidebarWidth), GUILayout.ExpandHeight(true)))
			{
				using(GUILayout.ScrollViewScope scrollViewScrope = new GUILayout.ScrollViewScope(sidebarScrollPosition))
				{
					sidebarScrollPosition = scrollViewScrope.scrollPosition;
					DrawTagField(-1);
					for(int i = 0; i < noteList.Tags.Count; i++)
					{
						DrawTagField(i);
					}
				}
				AddTagField();
			}
		}

		private void DrawMainArea()
		{
			using(new GUILayout.VerticalScope(GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
			{
				using(GUILayout.ScrollViewScope scrollViewScrope = new GUILayout.ScrollViewScope(mainAreaScrollPosition))
				{
					mainAreaScrollPosition = scrollViewScrope.scrollPosition;
					for(int i = 0; i < entriesToShow.Length; i++)
					{
						EntryField(i);
					}
				}
			}
		}

		private void DrawTagField(int index)
		{
			Event e = Event.current;
			string tag = index == -1 ? "ALL" : noteList.Tags[index];
			using(new GUILayout.HorizontalScope(EditorStyles.helpBox))
			{
				GUILayout.Label(tag);
				GUILayout.FlexibleSpace();
				GUILayout.Label(string.Format("({0})", noteList.GetCountByTag(index)));
				if(index != -1 && index != 0 && index != 1)
				{
					if(GUILayout.Button("-", EditorStyles.miniButton))
					{
						EditorApplication.delayCall += () =>
						{
							noteList.RemoveTag(index);
							Repaint();
						};
					}
				}
			}
			if(e.isMouse && e.type == EventType.MouseDown && GUILayoutUtility.GetLastRect().Contains(e.mousePosition))
			{
				SetCurrentTag(index);
			}
		}

		private void AddTagField()
		{
			using(new GUILayout.HorizontalScope(EditorStyles.helpBox))
			{
				newTagName = EditorGUILayout.TextField(newTagName);
				if(GUILayout.Button("Add", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
				{
					noteList.AddTag(newTagName);
					newTagName = "";
					GUI.FocusControl(null);
				}
			}
		}

		private void EntryField(int index)
		{
			Note entry = entriesToShow[index];
			using(new GUILayout.VerticalScope(EditorStyles.helpBox))
			{
				using(new GUILayout.HorizontalScope())
				{
					GUILayout.Label(entry.Tag, EditorStyles.boldLabel);
					GUILayout.FlexibleSpace();
					GUILayout.Label(entry.PathToShow, EditorStyles.miniBoldLabel);
				}
				GUILayout.Space(5f);
				GUILayout.Label(entry.Text, EditorStyles.largeLabel);
			}
			Event e = Event.current;
			Rect rect = GUILayoutUtility.GetLastRect();
			if(e.isMouse && e.type == EventType.MouseDown && rect.Contains(e.mousePosition) && e.clickCount == 2)
			{
				OpenScript(entry);
			}
		}

		private static void OpenScript(Note entry)
		{
			EditorApplication.delayCall += () => InternalEditorUtility.OpenFileAtLineExternal(entry.FilePath, entry.Line);
		}
		#endregion

		#region Files envents handlers
		private void OnCreated(object obj, FileSystemEventArgs e)
		{
			EditorApplication.delayCall += () => ScanFile(e.FullPath);
		}

		private void OnChanged(object obj, FileSystemEventArgs e)
		{
			EditorApplication.delayCall += () => ScanFile(e.FullPath);
		}

		private void OnRenamed(object obj, FileSystemEventArgs e)
		{
			EditorApplication.delayCall += () => ScanFile(e.FullPath);
		}

		private void OnDeleted(object obj, FileSystemEventArgs e)
		{
			EditorApplication.delayCall += () => noteList.Notes.RemoveAll(en => en.FilePath == e.FullPath);
		}
		#endregion

		#region Files Helpers
		private void ScanAllFiles()
		{
			RefreshFiles();
			foreach(FileInfo file in fileInfos.Where(file => file.Exists))
			{
				ScanFile(file.FullName);
			}
		}

		private void ScanFile(string filePath)
		{
			FileInfo file = new FileInfo(filePath);
			if(!file.Exists)
			{
				return;
			}

			List<Note> notes = new List<Note>();
			noteList.Notes.RemoveAll(e => e.FilePath == filePath);

			notes.AddRange(Note.Parse(filePath, noteList.Tags));
			noteList.Notes.AddRange(notes.Except(noteList.Notes));
		}

		private void RefreshFiles()
		{
			DirectoryInfo assetsDir = new DirectoryInfo(Application.dataPath);
			fileInfos = assetsDir.GetFiles("*.cs", SearchOption.AllDirectories)
				.Concat(assetsDir.GetFiles("*.js", SearchOption.AllDirectories))
				.ToArray();
		}
		#endregion

		#region UI helpers
		private void RefreshEntriesToShow()
		{
			if(currentTag == -1)
			{
				entriesToShow = noteList.Notes.ToArray();
			}
			else if(currentTag >= 0)
			{
				entriesToShow = noteList.Notes.Where(e => e.Tag == noteList.Tags[currentTag]).ToArray();
			}
			if(!string.IsNullOrEmpty(SearchString))
			{
				entriesToShow = entriesToShow.Where(e => e.Text.Contains(searchString)).ToArray();
			}
		}

		private void SetCurrentTag(int index)
		{
			EditorApplication.delayCall += () =>
			{
				currentTag = index;
				RefreshEntriesToShow();
				Repaint();
			};
		}

		public static string AssetsRelativePath(string absolutePath)
		{
			if(!absolutePath.StartsWith(Application.dataPath))
			{
				throw new ArgumentException("Full path does not contain the current project's Assets folder", "absolutePath");
			}
			return "Assets" + absolutePath.Substring(Application.dataPath.Length);
		}

		private string DrawSearchField(string searchString)
		{
			searchString = GUILayout.TextField(searchString, EditorStyles.toolbarSearchField, GUILayout.Width(250));
			if(GUILayout.Button(string.Empty, string.IsNullOrEmpty(searchString) ? 
				"ToolbarSeachCancelButtonEmpty" : "ToolbarSeachCancelButton"))
			{
				searchString = string.Empty;
				GUI.FocusControl(null);
			}
			return searchString;
		}
		#endregion

		#endregion
	}
}