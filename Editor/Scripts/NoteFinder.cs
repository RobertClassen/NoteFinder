namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;

	public class NoteFinder : EditorWindow, IDrawable
	{
		#region Constants
		private const string fileExtension = "*.cs";
		private const NotifyFilters notifyFilters = 
			NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Attributes |
			NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.Security;
		private const bool defaultExpansionState = true;
		private static readonly float indentWith = EditorGUIUtility.singleLineHeight;
		private static readonly GUILayoutOption smallButtonWidth = GUILayout.Width(indentWith);
		private static readonly GUILayoutOption smallButtonHeight = GUILayout.Height(indentWith);
		#endregion

		#region Fields
		[NonSerialized]
		public IDrawable IDrawable = null;

		[SerializeField]
		private MenuBar menuBar = null;

		[NonSerialized]
		private FileSystemWatcher watcher = null;

		[SerializeField]
		private TagList tagList = null;
		[SerializeField]
		private List<NoteList> noteLists = new List<NoteList>();

		[SerializeField]
		private IntBoolDictionary pathExpansionStates = new IntBoolDictionary();

		[SerializeField]
		private Vector2 mainAreaScrollPosition = Vector2.zero;

		private static GUIContent filterButtonContent = null;
		private static GUIContent highlightButtonContent = null;
		private static GUIStyle smallButtonStyle = null;
		#endregion

		#region Properties
		public TagList TagList
		{ get { return tagList; } }

		private static GUIStyle SmallButtonStyle
		{
			get
			{
				smallButtonStyle = smallButtonStyle ?? new GUIStyle(GUI.skin.button) { padding = new RectOffset() };
				return smallButtonStyle;
			}
		}
		#endregion

		#region Constructors
		[MenuItem("Tools/NoteFinder")]
		public static void OpenWindow()
		{
			NoteFinder noteFinder = GetWindow<NoteFinder>();
			noteFinder.minSize = new Vector2(250f, 250f);
			noteFinder.wantsMouseMove = true;
			noteFinder.titleContent = EditorGUIUtility.TrTextContentWithIcon("Notes", "d_UnityEditor.ConsoleWindow");
			noteFinder.Initialize();
			noteFinder.Show();
			noteFinder.ParseAll();
		}
		#endregion

		#region Methods
		void OnEnable()
		{
			Initialize();
		}

		void OnGUI()
		{
			if(IDrawable == null)
			{
				Draw();
			}
			else
			{
				IDrawable.Draw();
				if(GUILayout.Button("Exit"))
				{
					IDrawable = null;
				}
			}
		}

		private void Initialize()
		{
			tagList = Resources.Load<TagList>("TagList");

			if(menuBar == null)
			{
				menuBar = new MenuBar(this);
			}

			watcher = new FileSystemWatcher(Application.dataPath, fileExtension);
			watcher.NotifyFilter = notifyFilters;
			watcher.Created += QueueFileUpdate;
			watcher.Changed += QueueFileUpdate;
			watcher.Renamed += QueueFileUpdate;
			watcher.Deleted += QueueFileUpdate;
			watcher.EnableRaisingEvents = true;
			watcher.IncludeSubdirectories = true;

			Undo.undoRedoPerformed -= Repaint;
			Undo.undoRedoPerformed += Repaint;

			// For icon references see https://github.com/halak/unity-editor-icons
			//filterButtonContent = EditorGUIUtility.TrIconContent("ViewToolZoom", "Filter");
			filterButtonContent = EditorGUIUtility.TrIconContent("Search Icon", "Filter");
			highlightButtonContent = EditorGUIUtility.TrIconContent("cs Script Icon", "Show file");
		}

		private void QueueFileUpdate(object obj, FileSystemEventArgs e)
		{
			EditorApplication.delayCall += () => Parse(e.FullPath);
		}

		public void ParseAll()
		{
			ValidateNoteLists();
			foreach(string file in Directory.GetFiles(Application.dataPath, fileExtension, SearchOption.AllDirectories))
			{
				Parse(file);
			}
		}

		/// <summary>
		/// Removes <see cref="NoteList"/>s of deleted script files. 
		/// Workaround because the <see cref="FileSystemWatcher.Deleted"/> event is not called correctly.
		/// </summary>
		private void ValidateNoteLists()
		{
			for(int i = 0; i < noteLists.Count; i++)
			{
				if(!File.Exists(noteLists[i].GetFullPath()))
				{
					noteLists.RemoveAt(i);
				}
			}
		}

		private void Parse(string filePath)
		{
			string relativePath = GetRelativePath(filePath);
			NoteList newNoteList = NoteList.Parse(filePath, relativePath, tagList.Tags);
			if(newNoteList == null)
			{
				ValidateNoteLists();
				return;
			}
			noteLists.SetOrAddSorted(noteList => noteList.RelativePath == relativePath, newNoteList);
		}

		private static string GetRelativePath(string path)
		{
			return path.Remove(0, Application.dataPath.Length + 1);
		}

		public void Draw()
		{
			if(noteLists == null)
			{
				return;
			}

			menuBar.Draw();
			DrawNoteLists();
			DrawExpansionMenu(GUILayoutUtility.GetLastRect());
		}

		private void DrawNoteLists()
		{
			using(new ScrollViewScope(ref mainAreaScrollPosition))
			{
				NoteList previousNoteList = null;
				foreach(NoteList noteList in noteLists)
				{
					if(noteList.Notes.Count == 0)
					{
						continue;
					}

					DrawNoteList(noteList, previousNoteList);
					previousNoteList = noteList;
				}
			}
		}

		private void DrawNoteList(NoteList noteList, NoteList previousNoteList)
		{
			int depth = GetMinHierarchyDepth(noteList, previousNoteList);
			bool isExpanded = IsParentHierarchyExpanded(noteList, depth);
			if(!isExpanded)
			{
				return;
			}

			for(int i = depth; i < noteList.RelativePathHashes.Length; i++)
			{
				isExpanded = DrawFoldout(noteList, i);
				if(!isExpanded)
				{
					break;
				}
			}

			if(isExpanded)
			{
				noteList.Draw(menuBar.LowerSearchString, noteList.RelativePathHashes.Length, tagList);
			}
		}

		private int GetMinHierarchyDepth(NoteList noteList, NoteList previousNoteList)
		{
			if(previousNoteList == null)
			{
				return 0;
			}

			int minHierarchyDepth = 0;
			for(int i = 0; i < noteList.RelativePathHashes.Length; i++)
			{
				if(previousNoteList.RelativePathHashes.Length <= i ||
				   noteList.RelativePathHashes[i] != previousNoteList.RelativePathHashes[i])
				{
					break;
				}
				minHierarchyDepth++;
			}
			return minHierarchyDepth;
		}

		private bool IsParentHierarchyExpanded(NoteList noteList, int maxDepth)
		{
			if(maxDepth == 0)
			{
				return true;
			}

			bool isExpanded = false;
			for(int depth = 0; depth < maxDepth; depth++)
			{
				if(pathExpansionStates.TryGetValue(noteList.RelativePathHashes[depth], out isExpanded))
				{
					if(!isExpanded)
					{
						return false;
					}
				}
				else
				{
					pathExpansionStates[noteList.RelativePathHashes[depth]] = defaultExpansionState;
					return defaultExpansionState;
				}
			}
			return isExpanded;
		}

		private bool DrawFoldout(NoteList noteList, int depth)
		{
			bool isExpanded;
			if(!pathExpansionStates.TryGetValue(noteList.RelativePathHashes[depth], out isExpanded))
			{
				isExpanded = defaultExpansionState;
			}
			using(new EditorGUILayout.HorizontalScope())
			{
				GUILayout.Space(indentWith * depth);
				isExpanded = EditorGUILayout.Foldout(isExpanded, noteList.RelativeDirectoryContents[depth], true);
				if(depth == noteList.RelativePathHashes.Length - 1)
				{
					DrawHighlightButton(noteList);
				}
				DrawFilterButton(noteList, depth);
			}
			pathExpansionStates[noteList.RelativePathHashes[depth]] = isExpanded;
			return isExpanded;
		}

		private void DrawHighlightButton(NoteList noteList)
		{
			if(GUILayout.Button(highlightButtonContent, SmallButtonStyle, smallButtonWidth, smallButtonHeight))
			{
				EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<TextAsset>(
						string.Format("Assets/{0}", noteList.RelativePath
						.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))));
			}
		}

		private void DrawFilterButton(NoteList noteList, int depth)
		{
			if(GUILayout.Button(filterButtonContent, SmallButtonStyle, smallButtonWidth, smallButtonHeight))
			{
				menuBar.SetFilter(noteList.RelativeDirectoryContents[depth].text);
			}
		}

		private void DrawExpansionMenu(Rect rect)
		{
			if(!(Event.current.isMouse && Event.current.rawType == EventType.MouseDown && Event.current.button == 1))
			{
				return;
			}

			if(!rect.Contains(Event.current.mousePosition))
			{
				return;
			}

			GenericMenu menu = new GenericMenu();
			menu.AddItem("Collapse all", false, SetExpansionStates, false);
			menu.AddItem("Expand all", false, SetExpansionStates, true);
			menu.ShowAsContext();
		}

		private void SetExpansionStates(bool isExpanded)
		{
			foreach(int pathHash in noteLists.SelectMany(noteList => noteList.RelativePathHashes))
			{
				pathExpansionStates[pathHash] = isExpanded;
			}
		}

		private void SetExpansionStates(object isExpanded)
		{
			SetExpansionStates((bool)isExpanded);
		}

		public int GetTagCount(int index)
		{
			return noteLists.Sum(noteList => noteList.Notes.Count(note => note.TagIndex == index));
		}
		#endregion
	}
}