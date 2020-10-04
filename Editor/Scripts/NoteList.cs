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
		public List<Note> Notes = new List<Note>();

		[NonSerialized]
		private Vector2 mainAreaScrollPosition = Vector2.zero;
		#endregion

		#region Properties

		#endregion

		#region Constructors

		#endregion

		#region Methods
		public Note GetEntryAt(int index)
		{
			return Notes[index];
		}

		public void Draw(string searchString)
		{
			using(GUILayout.ScrollViewScope scrollViewScrope = new GUILayout.ScrollViewScope(mainAreaScrollPosition))
			{
				mainAreaScrollPosition = scrollViewScrope.scrollPosition;
				if(string.IsNullOrEmpty(searchString))
				{
					foreach(Note note in Notes)
					{
						note.Draw();
					}	
				}
				else
				{
					foreach(Note note in Notes)
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