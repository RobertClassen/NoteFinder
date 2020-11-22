namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	[CreateAssetMenu(menuName = "NoteFinder/NoteList")]
	[Serializable]
	public class NoteList : ScriptableObject
	{
		#region Fields
		[NonSerialized]
		private List<Note> notes = new List<Note>();

		[NonSerialized]
		private Dictionary<string, bool> isFilePathExpanded = new Dictionary<string, bool>();

		[NonSerialized]
		private Vector2 mainAreaScrollPosition = Vector2.zero;
		#endregion

		#region Properties
		public List<Note> Notes
		{ get { return notes; } }
		#endregion

		#region Constructors

		#endregion

		#region Methods
		public void Draw(string searchString)
		{
			using(GUILayout.ScrollViewScope scrollViewScrope = new GUILayout.ScrollViewScope(mainAreaScrollPosition))
			{
				mainAreaScrollPosition = scrollViewScrope.scrollPosition;
				string previousFilePath = string.Empty;
				bool isExpanded = true;
				foreach(Note note in notes)
				{
					if(!string.IsNullOrEmpty(searchString) && !note.Text.Contains(searchString))
					{
						continue;
					}

					if(note.RelativeFilePath != previousFilePath)
					{
						if(!isFilePathExpanded.TryGetValue(note.RelativeFilePath, out isExpanded))
						{
							isExpanded = true;
						}
						isFilePathExpanded[note.RelativeFilePath] = EditorGUILayout.Foldout(isExpanded, 
							note.RelativeFilePath, true);
					}

					if(isExpanded)
					{
						note.Draw();
					}

					previousFilePath = note.RelativeFilePath;
				}
			}
		}
		#endregion
	}
}