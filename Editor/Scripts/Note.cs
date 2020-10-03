namespace Todo
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
		#region Fields
		public string Text = null;
		public string Tag = null;
		public string FilePath = null;
		public int Line = 0;

		public string PathToShow = null;
		#endregion

		#region Properties

		#endregion

		#region Constructors
		private Note(string text, string tag, string filePath, int line)
		{
			Text = text;
			Tag = tag;
			FilePath = filePath;
			Line = line;

			PathToShow = string.Format("{0}({1})", FilePath.Remove(0, Application.dataPath.Length - 6).Replace("\\", "/"), Line);
		}
		#endregion

		#region Methods
		public static Note[] Parse(string filePath, List<string> tags)
		{
			if(!File.Exists(filePath))
			{
				return null;
			}

			string text = File.ReadAllText(filePath);
			List<Note> entries = new List<Note>();
			for(int i = 0; i < tags.Count; i++)
			{
				entries.AddRange(Regex.Matches(text, string.Format(@"(?<=\W|^)\/\/(\s?(?i){0}(?-i))(:?)(.*)", tags[i]))
					.Cast<Match>()
					.Select(match => new Note(match.Groups[3].Value.Trim(), tags[i], filePath, GetLine(text, match.Index))));
			}
			return entries.ToArray();
		}

		private static int GetLine(string text, int index)
		{
			return text.Take(index).Count(c => c == '\n') + 1;
		}

		public void Draw()
		{
			using(new GUILayout.VerticalScope(EditorStyles.helpBox))
			{
				using(new GUILayout.HorizontalScope())
				{
					GUILayout.Label(Tag, EditorStyles.boldLabel);
					GUILayout.FlexibleSpace();
					GUILayout.Label(PathToShow, EditorStyles.miniBoldLabel);
				}
				//GUILayout.Space(5f);
				GUIStyle textStyle = new GUIStyle(EditorStyles.largeLabel){ wordWrap = true };
				GUILayout.Label(Text, textStyle);
			}
			Event e = Event.current;
			Rect rect = GUILayoutUtility.GetLastRect();
			if(rect.Contains(e.mousePosition))
			{
				EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
				if(e.isMouse && e.type == EventType.MouseDown /*&& e.clickCount == 2*/)
				{
					OpenScript();
				}
			}
		}

		private void OpenScript()
		{
			EditorApplication.delayCall += () => InternalEditorUtility.OpenFileAtLineExternal(FilePath, Line);
		}

		public override bool Equals(object obj)
		{
			Note other = obj as Note;
			return ReferenceEquals(this, other) ||
			!ReferenceEquals(this, null) && !ReferenceEquals(other, null) && GetType() == other.GetType() &&
			Text == other.Text && Tag == other.Tag && FilePath == other.FilePath && Line == other.Line;
		}

		public override int GetHashCode()
		{
			int hashCode = (Text != null ? Text.GetHashCode() : 0);
			hashCode = (hashCode * 397) ^ (Tag != null ? Tag.GetHashCode() : 0);
			hashCode = (hashCode * 397) ^ (FilePath != null ? FilePath.GetHashCode() : 0);
			hashCode = (hashCode * 397) ^ Line;
			return hashCode;
		}
		#endregion
	}
}