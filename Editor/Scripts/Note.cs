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
	public class Note
	{
		#region Constants
		private static readonly GUILayoutOption buttonWidth = GUILayout.Width(50f);
		private static readonly GUILayoutOption tagWidth = GUILayout.Width(50f);
		private static readonly GUILayoutOption tagHeight = GUILayout.Height(EditorGUIUtility.singleLineHeight);
		#endregion

		#region Fields
		[SerializeField]
		private string filePath = null;
		[SerializeField]
		private int line = 0;
		[SerializeField]
		private Tag tag = null;
		[SerializeField]
		private string text = null;
		
		[SerializeField]
		private string relativeFilePath = null;
		[SerializeField]
		private GUIContent lineContent = null;
		#endregion

		#region Properties
		public string FilePath
		{ get { return filePath; } }

		public string RelativeFilePath
		{ get { return relativeFilePath; } }

		public Tag Tag
		{ get { return tag; } }

		public string Text
		{ get { return text; } }
		#endregion

		#region Constructors
		private Note(string filePath, int line, Tag tag, string text)
		{
			this.text = text;
			this.tag = tag;
			this.filePath = filePath;
			this.line = line;

			relativeFilePath = filePath.Remove(0, Application.dataPath.Length - 6).Replace("\\", "/");
			lineContent = new GUIContent(line.ToString(), "Go to line");
		}
		#endregion

		#region Methods
		public static Note[] Parse(string filePath, List<Tag> tags)
		{
			if(!File.Exists(filePath))
			{
				return null;
			}

			string text = File.ReadAllText(filePath);
			List<Note> entries = new List<Note>();
			for(int i = 0; i < tags.Count; i++)
			{
				entries.AddRange(Regex.Matches(text, string.Format(@"(?<=\W|^)\/\/(\s?(?i){0}(?-i))(:?)(.*)", tags[i].Name))
					.Cast<Match>()
					.Select(match => new Note(filePath, GetLine(text, match.Index), tags[i], match.Groups[3].Value.Trim())));
			}
			return entries.ToArray();
		}

		private static int GetLine(string text, int index)
		{
			return text.Take(index).Count(c => c == '\n') + 1;
		}

		public void Draw()
		{
			if(!tag.IsEnabled)
			{
				return;
			}

			using(new GUILayout.HorizontalScope())
			{
				GUILayout.Space(EditorGUIUtility.singleLineHeight);
				using(new GUILayout.HorizontalScope(EditorStyles.helpBox))
				{
					if(GUILayout.Button(lineContent, buttonWidth))
					{
						OpenScript();
					}
					using(new GUIColorScope(tag.Color))
					{
						GUILayout.Label(tag.Name, EditorStyles.helpBox, tagWidth);
					}

					GUIStyle textStyle = new GUIStyle(EditorStyles.label){ wordWrap = true };
					GUILayout.Label(text, textStyle);
				}
			}
		}

		private void OpenScript()
		{
			InternalEditorUtility.OpenFileAtLineExternal(filePath, line);
		}
		#endregion
	}
}