namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	[Serializable]
	public class SearchField
	{
		#region Constants
		private const string endCap = "ToolbarSeachCancelButtonEmpty";
		private const string endCapX = "ToolbarSeachCancelButton";
		#endregion

		#region Fields
		[SerializeField]
		private string text = string.Empty;
		[SerializeField]
		private string lowerText = string.Empty;
		#endregion

		#region Properties
		public string Text
		{
			get { return text; }
			set
			{
				if(text != value)
				{
					text = value;
					lowerText = text.ToLowerInvariant();
				}
			}
		}

		public string LowerText
		{ get { return lowerText; } }
		#endregion

		#region Constructors
		
		#endregion

		#region Methods
		public void Draw(Action onClear, float maxWidth = 300f)
		{
			bool isEmpty = string.IsNullOrEmpty(text);
			Text = GUILayout.TextField(text, EditorStyles.toolbarSearchField, GUILayout.MaxWidth(maxWidth));
			if(DrawEndCap(isEmpty) || IsEscapePressed())
			{
				Text = string.Empty;
				GUI.FocusControl(null);
				onClear?.Invoke();
				GUIUtility.ExitGUI();
			}
		}

		private static bool DrawEndCap(bool isEmpty)
		{
			return GUILayout.Button(string.Empty, isEmpty ? endCap : endCapX);
		}

		private static bool IsEscapePressed()
		{
			return Event.current.rawType == EventType.KeyUp && Event.current.keyCode == KeyCode.Escape;
		}
		#endregion
	}
}