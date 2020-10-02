namespace Todo
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[Serializable]
	public class TodoEntry
	{
		#region Fields
		public string Text = null;
		public string Note = null;
		public string Tag = null;
		public string File = null;
		public int Line = 0;

		public string PathToShow = null;
		#endregion

		#region Properties

		#endregion

		#region Constructors
		public TodoEntry(string text, string note, string tag, string file, int line)
		{
			Text = text;
			Note = note;
			Tag = tag;
			File = file;
			Line = line;

			PathToShow = string.Format("{0}({1})", File.Remove(0, Application.dataPath.Length - 6).Replace("\\", "/"), Line);
		}
		#endregion

		#region Methods
		public override bool Equals(object obj)
		{
			TodoEntry x = this;
			TodoEntry y = (TodoEntry)obj;
			if(ReferenceEquals(x, y))
			{
				return true;
			}
			if(ReferenceEquals(x, null))
			{
				return false;
			}
			if(ReferenceEquals(y, null))
			{
				return false;
			}
			if(x.GetType() != y.GetType())
			{
				return false;
			}
			return string.Equals(x.Text, y.Text) && string.Equals(x.Note, y.Note) && string.Equals(x.Tag, y.Tag) && string.Equals(x.File, y.File) && x.Line == y.Line;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var obj = this;
				var hashCode = (obj.Text != null ? obj.Text.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (obj.Note != null ? obj.Note.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (obj.Tag != null ? obj.Tag.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (obj.File != null ? obj.File.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ obj.Line;
				return hashCode;
			}
		}
		#endregion
	}
}