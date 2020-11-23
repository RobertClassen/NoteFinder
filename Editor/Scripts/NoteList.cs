namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text.RegularExpressions;
	using UnityEditor;
	using UnityEditorInternal;
	using UnityEngine;

	[Serializable]
	public class NoteList
	{
		#region Fields
		[SerializeField]
		private string relativePath = null;
		[SerializeField]
		private bool isExpanded = true;
		[SerializeField]
		private List<Note> notes = new List<Note>();
		#endregion

		#region Properties
		public string RelativePath
		{ get { return relativePath; } }

		public List<Note> Notes
		{ get { return notes; } }
		#endregion

		#region Constructors
		private NoteList(string relativePath, List<Note> notes)
		{
			this.relativePath = relativePath;
			this.notes = notes;
		}
		#endregion

		#region Methods
		public static NoteList Parse(string filePath, string relativePath, List<Tag> tags)
		{
			string text = File.ReadAllText(filePath);
			List<Note> notes = new List<Note>();
			foreach(Tag tag in tags)
			{
				notes.AddRange(Regex.Matches(text, string.Format(@"(?<=\W|^)\/\/(\s?(?i){0}(?-i))(:?)(.*)", tag.Name))
					.Cast<Match>()
					.Select(match => new Note(GetLine(text, match.Index), tag, match.Groups[3].Value.Trim())));
			}
			return new NoteList(relativePath, notes.OrderBy(note => note.Line).ToList());
		}

		private static int GetLine(string text, int index)
		{
			return text.Take(index).Count(c => c == '\n') + 1;
		}

		public void Draw(string searchString)
		{
			isExpanded = EditorGUILayout.Foldout(isExpanded, RelativePath, true);
			if(!isExpanded)
			{
				return;
			}

			using(new GUILayout.HorizontalScope())
			{
				GUILayout.Space(EditorGUIUtility.singleLineHeight);
				using(new GUILayout.VerticalScope())
				{
					foreach(Note note in notes)
					{
						if(!string.IsNullOrEmpty(searchString) && !note.Text.Contains(searchString))
						{
							continue;
						}

						note.Draw(this);
					}
				}
			}
		}

		public void OpenScript(int line)
		{
			InternalEditorUtility.OpenFileAtLineExternal(GetFullPath(relativePath), line);
		}

		private static string GetFullPath(string relativePath)
		{
			return Path.Combine(Directory.GetParent(Application.dataPath).FullName, relativePath);
		}
		#endregion
	}
}