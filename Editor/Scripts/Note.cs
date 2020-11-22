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
		private string PathToShow = null;
		#endregion

		#region Properties
		public string FilePath
		{ get { return filePath; } }

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

			PathToShow = string.Format("{0} ({1})", filePath.Remove(0, Application.dataPath.Length - 6).Replace("\\", "/"), line);
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

			using(new GUIColorScope(tag.Color))
			{
				using(new GUILayout.VerticalScope(GUI.skin.box))
				{
					using(new GUILayout.HorizontalScope())
					{
						GUILayout.Label(tag.Name, EditorStyles.boldLabel);
						GUILayout.FlexibleSpace();
						GUIStyle pathStyle = new GUIStyle(EditorStyles.miniBoldLabel){ wordWrap = true };
						GUILayout.Label(PathToShow, pathStyle);
					}
					//GUILayout.Space(5f);
					GUIStyle textStyle = new GUIStyle(EditorStyles.largeLabel){ wordWrap = true };
					GUILayout.Label(text, textStyle);
				}
			}

			Event e = Event.current;
			Rect rect = GUILayoutUtility.GetLastRect();
			if(rect.Contains(e.mousePosition))
			{
				EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
				if(e.isMouse && e.type == EventType.MouseUp /*&& e.clickCount == 2*/)
				{
					OpenScript();
				}
			}
		}

		private void OpenScript()
		{
			EditorApplication.delayCall += () => InternalEditorUtility.OpenFileAtLineExternal(filePath, line);
		}

		public override bool Equals(object obj)
		{
			Note other = obj as Note;
			return ReferenceEquals(this, other) ||
			!ReferenceEquals(this, null) && !ReferenceEquals(other, null) && GetType() == other.GetType() &&
			text == other.text && tag == other.tag && filePath == other.filePath && line == other.line;
		}

		public override int GetHashCode()
		{
			int hashCode = (Text != null ? Text.GetHashCode() : 0);
			hashCode = (hashCode * 397) ^ (tag != null ? tag.GetHashCode() : 0);
			hashCode = (hashCode * 397) ^ (filePath != null ? filePath.GetHashCode() : 0);
			hashCode = (hashCode * 397) ^ line;
			return hashCode;
		}
		#endregion
	}
}