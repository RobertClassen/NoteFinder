namespace Todo
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;

	public class ToDoManager : EditorWindow
	{
		#region Constants
		private const string fileExtension = "*.cs";
		private const float sidebarWidth = 150f;
		#endregion

		#region Fields
		private DirectoryInfo directory = null;
		private FileSystemWatcher watcher = null;
		private NoteList noteList = null;

		private string searchString = string.Empty;

		private string newTagName = string.Empty;

		private Vector2 sidebarScrollPosition = Vector2.zero;
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
				}
			}
		}
		#endregion

		#region Constructors
		[MenuItem("Tools/TODO Manager")]
		public static void OpenWindow()
		{
			ToDoManager window = GetWindow<ToDoManager>("//TODO");
			window.minSize = new Vector2(400, 250);
			window.wantsMouseMove = true;
			window.Show();
		}
		#endregion

		#region Methods
		void OnEnable()
		{
			Initialize();
		}

		void OnGUI()
		{
			Draw();

			if(Event.current.type == EventType.MouseMove)
			{
				Repaint();
			}
		}

		private void Initialize()
		{
			NoteList[] noteLists = Resources.FindObjectsOfTypeAll<NoteList>();
			if(noteLists != null && noteLists.Length > 0)
			{
				noteList = noteLists[0];
			}
			
			directory = new DirectoryInfo(Application.dataPath);

			watcher = new FileSystemWatcher(Application.dataPath, fileExtension);
			watcher.Created += OnCreated;
			watcher.Changed += OnChanged;
			watcher.Renamed += OnRenamed;
			watcher.Deleted += OnDeleted;

			watcher.EnableRaisingEvents = true;
			watcher.IncludeSubdirectories = true;

			ScanAllFiles();
		}

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
			EditorApplication.delayCall += () => noteList.Notes.RemoveAll(note => note.FilePath == e.FullPath);
		}

		private void Draw()
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
				DrawSideBar();
				//TODO: use tag instead SearchString
				noteList.Draw(SearchString);
			}

			EditorUtility.SetDirty(noteList);
		}

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

		private void DrawSideBar()
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

		private void DrawTagField(int index)
		{
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
			Event e = Event.current;
			if(e.isMouse && e.type == EventType.MouseDown && GUILayoutUtility.GetLastRect().Contains(e.mousePosition))
			{
				SetCurrentTag();
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

		private void ScanAllFiles()
		{
			foreach(FileInfo file in directory.GetFiles(fileExtension, SearchOption.AllDirectories))
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

			noteList.Notes.RemoveAll(note => note.FilePath == filePath);
			noteList.Notes.AddRange(Note.Parse(filePath, noteList.Tags));
		}

		private void SetCurrentTag()
		{
			EditorApplication.delayCall += Repaint;
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
	}
}