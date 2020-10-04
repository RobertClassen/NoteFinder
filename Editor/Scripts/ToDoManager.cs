namespace Notes
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
		[SerializeField]
		private MenuBar menuBar = null;

		private DirectoryInfo directory = null;
		private FileSystemWatcher watcher = null;

		[SerializeField]
		private TagList tagList = null;
		[SerializeField]
		private NoteList noteList = null;
		#endregion

		#region Properties
		public TagList TagList
		{ get { return tagList; } }
		#endregion

		#region Constructors
		[MenuItem("Tools/NoteFinder")]
		public static void OpenWindow()
		{
			ToDoManager window = GetWindow<ToDoManager>();
			window.minSize = new Vector2(400, 250);
			window.wantsMouseMove = true;
			window.titleContent = new GUIContent("Notes", EditorGUIUtility.IconContent("d_UnityEditor.ConsoleWindow").image);
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
			TagList[] tagLists = Resources.FindObjectsOfTypeAll<TagList>();
			if(tagLists != null && tagLists.Length > 0)
			{
				tagList = tagLists[0];
			}

			if(menuBar == null)
			{
				menuBar = new MenuBar(this);
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

			menuBar.Draw();
			using(new GUILayout.HorizontalScope())
			{
				//DrawSideBar();
				noteList.Draw(menuBar.SearchString);
			}

			EditorUtility.SetDirty(noteList);
		}

		public void ScanAllFiles()
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
			noteList.Notes.AddRange(Note.Parse(filePath, tagList.Tags));
		}
		#endregion
	}
}