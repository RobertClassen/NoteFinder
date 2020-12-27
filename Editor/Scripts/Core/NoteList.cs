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

	/// <summary>
	/// Contains all <see cref="Note"/>s of a single script file.
	/// </summary>
	[Serializable]
	public class NoteList : IComparable<NoteList>
	{
		#region Constants
		/// <summary>
		/// Used instead of <see cref="Environment.NewLine"/> 
		/// because script files do not necessarily use the correct line endings depending on the current platform.
		/// </summary>
		private const char newLine = '\n';
		#endregion

		#region Fields
		[SerializeField]
		private string relativePath = null;
		[SerializeField]
		private string lowerRelativePath = null;

		[SerializeField]
		private FoldoutInfo[] foldoutInfos = null;
		[SerializeField]
		private List<Note> notes = new List<Note>();

		[SerializeField]
		private string foldoutTitle = null;
		#endregion

		#region Properties
		public string RelativePath
		{ get { return relativePath; } }

		public FoldoutInfo[] FoldoutInfos
		{ get { return foldoutInfos; } }

		public List<Note> Notes
		{ get { return notes; } }
		#endregion

		#region Constructors
		private NoteList(string relativePath, List<Note> notes)
		{
			this.relativePath = relativePath;
			lowerRelativePath = relativePath.ToLowerInvariant();
			this.notes = notes;
			foldoutTitle = string.Format("{0} ({1})", relativePath, notes.Count);

			string[] relativePathNames = relativePath.Split(Path.DirectorySeparatorChar);
			int count = relativePathNames.Length;
			foldoutInfos = new FoldoutInfo[count];
			string combinedPath = relativePathNames[0];
			for(int i = 0; i < count; i++)
			{
				if(i > 0)
				{
					combinedPath = string.Format("{0}{1}{2}", combinedPath, Path.DirectorySeparatorChar, relativePathNames[i]);
				}

				foldoutInfos[i] = new FoldoutInfo(combinedPath.GetHashCode(), relativePathNames[i], i < count - 1);
			}
		}
		#endregion

		#region Methods
		public static NoteList Parse(string filePath, string relativePath, List<Tag> tags)
		{
			if(!File.Exists(filePath))
			{
				return null;
			}

			string text = File.ReadAllText(filePath);
			List<Note> notes = new List<Note>();
			for(int i = 0; i < tags.Count; i++)
			{
				notes.AddRange(Regex.Matches(text, string.Format(@"(?<=\W|^)\/\/(\s?(?i){0}(?-i))(:?)(.*)", tags[i].Name))
					.Cast<Match>()
					.Select(match => new Note(GetLine(text, match.Index), i, match.Groups[3].Value.Trim())));
			}
			return new NoteList(relativePath, notes.OrderBy(note => note.Line).ToList());
		}

		private static int GetLine(string text, int index)
		{
			int line = 1;
			for(int i = 0; i < index; i++)
			{
				if(text[i] == newLine)
				{
					line++;
				}
			}
			return line;
		}

		public GUIContent GetLabel(bool isExpanded, int depth)
		{
			return foldoutInfos[depth].GetLabel(isExpanded);
		}

		public void Draw(string searchString, int indentation, TagList tagList)
		{
			if(notes.Count == 0)
			{
				return;
			}

			bool isFiltered = !string.IsNullOrEmpty(searchString);
			bool isSearched = isFiltered && lowerRelativePath.Contains(searchString);
			if(!(isSearched || notes.Any(note => note.LowerText.Contains(searchString))))
			{
				return;
			}

			using(new LayoutGroup.Scope(LayoutGroup.Direction.Horizontal))
			{
				GUILayout.Space(EditorGUIUtility.singleLineHeight * indentation);
				using(new LayoutGroup.Scope(LayoutGroup.Direction.Vertical))
				{
					foreach(Note note in notes)
					{
						if(!isSearched && isFiltered && !note.Text.Contains(searchString))
						{
							continue;
						}

						note.Draw(this, tagList.Tags[note.TagIndex]);
					}
				}
			}
		}

		public void OpenScript(int line)
		{
			InternalEditorUtility.OpenFileAtLineExternal(GetFullPath(), line);
		}

		public string GetFullPath()
		{
			return Path.Combine(Application.dataPath, relativePath)
				.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
		}

		public int CompareTo(NoteList other)
		{
			return relativePath.CompareTo(other.relativePath);
		}
		#endregion
	}
}