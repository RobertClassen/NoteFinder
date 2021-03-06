namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;
	using FileSystemWatcher = IO.FileSystemWatcher;

	public class NoteFinder : EditorWindow
	{
		#region Constants
		private const string fileExtension = "*.cs";
		private const bool defaultExpansionState = true;
		private const string titleIconName = "UnityEditor.ConsoleWindow";
		public const string ScriptIconName = "cs Script Icon";
		private const string searchIconName = "Search Icon";
		private static readonly float indentWith = EditorGUIUtility.singleLineHeight;
		private static readonly GUILayoutOption smallButtonWidth = GUILayout.Width(indentWith);
		private static readonly GUILayoutOption smallButtonHeight = GUILayout.Height(indentWith);
		#endregion

		#region Fields
		[NonSerialized]
		public IDrawable IDrawable = null;

		[SerializeField]
		private MenuBar menuBar = null;

		[SerializeField]
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
		{ get { return smallButtonStyle ?? (smallButtonStyle = new GUIStyle(GUI.skin.button) { padding = new RectOffset() }); } }
		#endregion

		#region Constructors
		[MenuItem("Tools/NoteFinder")]
		public static void OpenWindow()
		{
			NoteFinder noteFinder = GetWindow<NoteFinder>();
			noteFinder.minSize = new Vector2(250f, 250f);
			noteFinder.wantsMouseMove = true;
			noteFinder.Initialize();
			noteFinder.Show();
			noteFinder.ParseAll();
		}
		#endregion

		#region Methods
		void OnEnable()
		{
			Initialize();
			watcher.ValidateFiles(Parse);
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
			titleContent = EditorGUIUtility.TrTextContentWithIcon("Notes", titleIconName);
			tagList = Resources.Load<TagList>("TagList");

			watcher = watcher ?? new FileSystemWatcher(Application.dataPath, fileExtension, Parse);
			menuBar = menuBar ?? new MenuBar(this);

			Undo.undoRedoPerformed -= Repaint;
			Undo.undoRedoPerformed += Repaint;

			filterButtonContent = EditorGUIUtility.TrIconContent(searchIconName, "Filter");
			highlightButtonContent = EditorGUIUtility.TrIconContent(ScriptIconName, "Show file");
		}

		public void ParseAll()
		{
			foreach(string filePath in Directory.EnumerateFiles(Application.dataPath, fileExtension, SearchOption.AllDirectories))
			{
				Parse(filePath);
			}
		}

		private void Parse(string filePath)
		{
			filePath = filePath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			string relativePath = GetRelativePath(filePath);
			if(File.Exists(filePath))
			{
				noteLists.SetOrAddSorted(noteList => noteList.RelativePath == relativePath, 
					NoteList.Parse(filePath, relativePath, tagList.Tags));
			}
			else
			{
				noteLists.TryRemove(noteList => noteList.RelativePath == relativePath);
			}
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

			for(int i = depth; i < noteList.FoldoutInfos.Length; i++)
			{
				isExpanded = DrawFoldout(noteList, i);
				if(!isExpanded)
				{
					break;
				}
			}

			if(isExpanded)
			{
				noteList.Draw(menuBar.SearchField.LowerText, noteList.FoldoutInfos.Length, tagList);
			}
		}

		private int GetMinHierarchyDepth(NoteList noteList, NoteList previousNoteList)
		{
			if(previousNoteList == null)
			{
				return 0;
			}

			int minHierarchyDepth = 0;
			for(int i = 0; i < noteList.FoldoutInfos.Length; i++)
			{
				if(previousNoteList.FoldoutInfos.Length <= i ||
				   noteList.FoldoutInfos[i].Hash != previousNoteList.FoldoutInfos[i].Hash)
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
				if(pathExpansionStates.TryGetValue(noteList.FoldoutInfos[depth].Hash, out isExpanded))
				{
					if(!isExpanded)
					{
						return false;
					}
				}
				else
				{
					pathExpansionStates[noteList.FoldoutInfos[depth].Hash] = defaultExpansionState;
					return defaultExpansionState;
				}
			}
			return isExpanded;
		}

		private bool DrawFoldout(NoteList noteList, int depth)
		{
			bool isExpanded;
			if(!pathExpansionStates.TryGetValue(noteList.FoldoutInfos[depth].Hash, out isExpanded))
			{
				isExpanded = defaultExpansionState;
			}
			using(new EditorGUILayout.HorizontalScope())
			{
				GUILayout.Space(indentWith * depth);
				GUIContent label = noteList.GetLabel(isExpanded, depth);
				isExpanded = EditorGUILayout.Foldout(isExpanded, label, true);
				if(depth == noteList.FoldoutInfos.Length - 1)
				{
					DrawHighlightButton(noteList);
				}
				DrawFilterButton(label);
			}
			pathExpansionStates[noteList.FoldoutInfos[depth].Hash] = isExpanded;
			return isExpanded;
		}

		private void DrawHighlightButton(NoteList noteList)
		{
			if(GUILayout.Button(highlightButtonContent, SmallButtonStyle, smallButtonWidth, smallButtonHeight))
			{
				EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<TextAsset>(
						string.Format("Assets/{0}", noteList.RelativePath)));
			}
		}

		private void DrawFilterButton(GUIContent label)
		{
			if(GUILayout.Button(filterButtonContent, SmallButtonStyle, smallButtonWidth, smallButtonHeight))
			{
				menuBar.SetFilter(label.text);
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
			foreach(int pathHash in noteLists.SelectMany(noteList => noteList.FoldoutInfos
				.Select(foldoutInfo=>foldoutInfo.Hash)))
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