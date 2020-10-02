namespace Todo
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	[CreateAssetMenu(menuName = "TODO Manager/NoteList")]
	[Serializable]
	public class TodoData : ScriptableObject
	{
		#region Fields
		public List<TodoEntry> Entries = new List<TodoEntry>();
		public List<string> Tags = new List<string> { "TODO", "BUG" };
		#endregion

		#region Properties

		#endregion

		#region Constructors

		#endregion

		#region Methods
		public int GetCountByTag(int tagIndex)
		{
			return tagIndex == -1 ? Entries.Count : Entries.Count(entry => entry.Tag == Tags[tagIndex]);
		}

		public TodoEntry GetEntryAt(int index)
		{
			return Entries[index];
		}

		public void AddTag(string tag)
		{
			if(Tags.Contains(tag) || string.IsNullOrEmpty(tag))
			{
				return;
			}
			Tags.Add(tag);
		}

		public void RemoveTag(int index)
		{
			if(Tags.Count >= (index + 1))
			{
				Tags.RemoveAt(index);
			}
		}
		#endregion
	}
}