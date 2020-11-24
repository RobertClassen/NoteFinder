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
		#region Constants
		private static readonly GUIContent filterButtonContent = EditorGUIUtility.TrIconContent("FilterByLabel", "Filter");
		private static readonly GUILayoutOption filterButtonWidth = GUILayout.Width(EditorGUIUtility.singleLineHeight);
		private static readonly GUILayoutOption filterButtonHeight = GUILayout.Height(EditorGUIUtility.singleLineHeight);
		#endregion

		#region Fields
		[SerializeField]
		private string relativePath = null;
		[SerializeField]
		private string lowerRelativePath = null;
		[SerializeField]
		private bool isExpanded = true;
		[SerializeField]
		private List<Note> notes = new List<Note>();

		[SerializeField]
		private string foldoutTitle = null;

		private static GUIStyle filterButtonStyle = null;
		#endregion

		#region Properties
		public string RelativePath
		{ get { return relativePath; } }

		public List<Note> Notes
		{ get { return notes; } }

		private static GUIStyle FilterButtonStyle
		{
			get
			{
				filterButtonStyle = filterButtonStyle ?? new GUIStyle(GUI.skin.button) { padding = new RectOffset() };
				return filterButtonStyle;
			}
		}
		#endregion

		#region Constructors
		private NoteList(string relativePath, List<Note> notes)
		{
			this.relativePath = relativePath;
			lowerRelativePath = relativePath.ToLowerInvariant();
			this.notes = notes;
			foldoutTitle = string.Format("{0} ({1})", relativePath, notes.Count);
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
			int line = 1;
			for(int i = 0; i < index; i++)
			{
				if(text[i] == '\n')
				{
					line++;
				}
			}
			return line;
		}

		public void Draw(string searchString, Action<string> onFilter)
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
				isExpanded = EditorGUILayout.Foldout(isExpanded, foldoutTitle, true);
				if(GUILayout.Button(filterButtonContent, FilterButtonStyle, filterButtonWidth, filterButtonHeight))
				{
					onFilter?.Invoke(relativePath);
				}
			}
			if(!isExpanded)
			{
				return;
			}

			using(new LayoutGroup.Scope(LayoutGroup.Direction.Horizontal))
			{
				GUILayout.Space(EditorGUIUtility.singleLineHeight);
				using(new LayoutGroup.Scope(LayoutGroup.Direction.Vertical))
				{
					foreach(Note note in notes)
					{
						if(!isSearched && isFiltered && !note.Text.Contains(searchString))
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