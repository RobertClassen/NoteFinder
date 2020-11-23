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
		private NoteListCollection noteListCollection = new NoteListCollection();

		[SerializeField]
		private Vector2 mainAreaScrollPosition = Vector2.zero;
		#endregion

		#region Properties
		public TagList TagList
		{ get { return tagList; } }

		public NoteListCollection NoteListCollection
		{ get { return noteListCollection; } }
		#endregion

		#region Constructors
		[MenuItem("Tools/NoteFinder")]
		public static void OpenWindow()
		{
			NoteFinder window = GetWindow<NoteFinder>();
			window.minSize = new Vector2(250f, 250f);
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
			TagList[] tagLists = Resources.FindObjectsOfTypeAll<TagList>();
			if(tagLists?.Length > 0)
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

			Undo.undoRedoPerformed -= Repaint;
			Undo.undoRedoPerformed += Repaint;

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
			EditorApplication.delayCall += () => noteListCollection.NoteLists
				.RemoveAll(note => note.RelativePath == GetRelativePath(e.FullPath));
		}

		public void Draw()
		{
			if(noteListCollection == null)
			{
				GUILayout.Label("No Notes loaded", EditorStyles.centeredGreyMiniLabel);
				return;
			}

			menuBar.Draw();
			using(GUILayout.ScrollViewScope scrollViewScrope = new GUILayout.ScrollViewScope(mainAreaScrollPosition))
			{
				mainAreaScrollPosition = scrollViewScrope.scrollPosition;
				noteListCollection.Draw(menuBar.SearchString);
			}
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
			if(!File.Exists(filePath))
			{
				return;
			}

			string relativePath = GetRelativePath(filePath);
			noteListCollection.NoteLists.RemoveAll(noteList => noteList.RelativePath == relativePath);
			Debug.Log(noteListCollection.NoteLists.Count);
			noteListCollection.NoteLists.Add(NoteList.Parse(filePath, relativePath, tagList.Tags));
		}

		private static string GetRelativePath(string path)
		{
			return path.Remove(0, Directory.GetParent(Application.dataPath).FullName.Length + 1);
		}
		#endregion
	}
}