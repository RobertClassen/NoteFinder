namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(menuName = "TODO Manager/NoteList")]
	[Serializable]
	public class NoteList : ScriptableObject
	{
		#region Fields
		[NonSerialized]
		private List<Note> notes = new List<Note>();

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
		public Note GetEntryAt(int index)
		{
			return notes[index];
		}

		public void Draw(string searchString)
		{
			using(GUILayout.ScrollViewScope scrollViewScrope = new GUILayout.ScrollViewScope(mainAreaScrollPosition))
			{
				mainAreaScrollPosition = scrollViewScrope.scrollPosition;
				if(string.IsNullOrEmpty(searchString))
				{
					foreach(Note note in notes)
					{
						note.Draw();
					}	
				}
				else
				{
					foreach(Note note in notes)
					{
						if(!note.Text.Contains(searchString))
						{
							continue;
						}
						note.Draw();
					}
				}
			}
		}
		#endregion
	}
}