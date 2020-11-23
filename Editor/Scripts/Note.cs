namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
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
		private int line = 0;
		[SerializeField]
		private Tag tag = null;
		[SerializeField]
		private string text = null;
		
		[SerializeField]
		private GUIContent lineButtonContent = null;
		#endregion

		#region Properties
		public int Line
		{ get { return line; } }

		public Tag Tag
		{ get { return tag; } }

		public string Text
		{ get { return text; } }
		#endregion

		#region Constructors
		public Note(int line, Tag tag, string text)
		{
			this.text = text;
			this.tag = tag;
			this.line = line;

			lineButtonContent = new GUIContent(line.ToString(), "Go to line");
		}
		#endregion

		#region Methods
		public void Draw(NoteList noteList)
		{
			if(!tag.IsEnabled)
			{
				return;
			}

			using(new GUILayout.HorizontalScope(EditorStyles.helpBox))
			{
				if(GUILayout.Button(lineButtonContent, buttonWidth))
				{
					noteList.OpenScript(line);
				}
				using(new GUIColorScope(tag.Color))
				{
					GUILayout.Label(tag.Name, EditorStyles.helpBox, tagWidth);
				}

				GUIStyle textStyle = new GUIStyle(EditorStyles.label){ wordWrap = true };
				GUILayout.Label(text, textStyle);
			}
		}
		#endregion
	}
}