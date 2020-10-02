namespace Todo
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text.RegularExpressions;
	using UnityEngine;

	public class ScriptsParser
	{
		#region Fields
		private string filePath;
		private string text;
		private string[] tags;
		#endregion

		#region Properties

		#endregion

		#region Constructors
		public ScriptsParser(string filePath, string[] tags = null)
		{
			this.filePath = filePath;
			FileInfo file = new FileInfo(filePath);
			if(file.Exists)
			{
				text = File.ReadAllText(filePath);
			}
			this.tags = tags;
		}
		#endregion

		#region Methods
		public Note[] Parse()
		{
			FileInfo file = new FileInfo(filePath);
			if(!file.Exists)
			{
				return null;
			}

			List<Note> entries = new List<Note>();
			for(int i = 0; i < tags.Length; i++)
			{
				entries.AddRange(Regex.Matches(text, string.Format(@"(?<=\W|^)//(\s?{0})(.*)", tags[i]))
					.Cast<Match>()
					.Select(match => new Note(match.Groups[2].Value, tags[i], filePath, GetLine(match.Index))));
			}
			return entries.ToArray();
		}

		private int GetLine(int index)
		{
			return text.Take(index).Count(c => c == '\n') + 1;
		}
		#endregion
	}
}