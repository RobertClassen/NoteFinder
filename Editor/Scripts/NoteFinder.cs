namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using UnityEditor;
	using UnityEngine;

	public class NoteFinder : EditorWindow, IDrawable
	{
		#region Constants
		private const string fileExtension = "*.cs";
		private const float sidebarWidth = 150f;
		#endregion

		#region Fields
		[NonSerialized]
		public IDrawable IDrawable = null;

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

		public NoteList NoteList
		{ get { return noteList; } }
		#endregion

		#region Constructors
		[MenuItem("Tools/NoteFinder")]
		public static void OpenWindow()
		{
			NoteFinder window = GetWindow<NoteFinder>();
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

		public void Draw()
		{
			if(noteList == null)
			{
				GUILayout.Label("No data loaded", EditorStyles.centeredGreyMiniLabel);
				return;
			}

			menuBar.Draw();
			noteList.Draw(menuBar.SearchString);
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