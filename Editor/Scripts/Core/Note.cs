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
		private static readonly GUILayoutOption lineButtonWidth = GUILayout.Width(50f);
		#endregion

		#region Fields
		[SerializeField]
		private int line = 0;
		[SerializeField]
		private int tagIndex = 0;
		[SerializeField]
		private string text = null;
		[SerializeField]
		private string lowerText = null;
		
		[SerializeField]
		private GUIContent lineButtonContent = null;

		[NonSerialized]
		private static GUIStyle textStyle = null;
		#endregion

		#region Properties
		public int Line
		{ get { return line; } }

		public int TagIndex
		{ get { return tagIndex; } }

		public string LowerText
		{ get { return lowerText; } }

		private static GUIStyle TextStyle
		{ get { return textStyle ?? (textStyle = new GUIStyle(EditorStyles.label) { wordWrap = true }); } }
		#endregion

		#region Constructors
		public Note(int line, int tagIndex, string text)
		{
			this.line = line;
			this.tagIndex = tagIndex;
			this.text = text;
			lowerText = text.ToLowerInvariant();

			lineButtonContent = new GUIContent(line.ToString(), "Go to line");
		}
		#endregion

		#region Methods
		public void Draw(NoteList noteList, Tag tag)
		{
			if(!tag.IsEnabled)
			{
				return;
			}

			using(new LayoutGroup.Scope(LayoutGroup.Direction.Horizontal, GUI.skin.box))
			{
				if(GUILayout.Button(lineButtonContent, lineButtonWidth))
				{
					noteList.OpenScript(line);
				}
				tag.Draw();

				GUILayout.Label(text, TextStyle);
			}
		}
		#endregion
	}
}