namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	internal static class SearchField
	{
		#region Constants
		private const string endCap = "ToolbarSeachCancelButtonEmpty";
		private const string endCapX = "ToolbarSeachCancelButton";
		#endregion

		#region Fields
		
		#endregion

		#region Properties
		
		#endregion

		#region Constructors
		
		#endregion

		#region Methods
		public static void Draw(ref string searchString, Action onClear, float maxWidth = 300f)
		{
			searchString = GUILayout.TextField(searchString, EditorStyles.toolbarSearchField, GUILayout.MaxWidth(maxWidth));
			if(DrawEndCap(searchString) || IsEscapePressed())
			{
				searchString = string.Empty;
				GUI.FocusControl(null);
				onClear?.Invoke();
				GUIUtility.ExitGUI();
			}
		}

		private static bool DrawEndCap(string searchString)
		{
			return GUILayout.Button(string.Empty, string.IsNullOrEmpty(searchString) ? endCap : endCapX);
		}

		private static bool IsEscapePressed()
		{
			return Event.current.rawType == EventType.KeyUp && Event.current.keyCode == KeyCode.Escape;
		}
		#endregion
	}
}