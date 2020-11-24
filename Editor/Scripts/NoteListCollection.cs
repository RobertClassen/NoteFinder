namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	[Serializable]
	public class NoteListCollection
	{
		#region Fields
		[SerializeField]
		private List<NoteList> noteLists = new List<NoteList>();
		#endregion

		#region Properties
		public List<NoteList> NoteLists
		{ get { return noteLists; } }
		#endregion

		#region Constructors
		
		#endregion

		#region Methods
		public void Draw(string searchString, Action<string> onFilter)
		{
			foreach(NoteList noteList in noteLists)
			{
				noteList.Draw(searchString, onFilter);
			}
		}

		public int GetTagCount(Tag tag)
		{
			return noteLists.Sum(noteList => noteList.Notes.Count(note => note.Tag == tag));
		}
		#endregion
	}
}