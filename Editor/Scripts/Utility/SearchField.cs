namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	public static class SearchField
	{
		#region Constants
		private const string endCap = "ToolbarSeachCancelButton";
		private const string endCapEmpty = "ToolbarSeachCancelButtonEmpty";
		#endregion

		#region Fields
		
		#endregion

		#region Properties
		
		#endregion

		#region Constructors
		
		#endregion

		#region Methods
		public static string Draw(string searchString, Action onClear, float maxWidth = 300f)
		{
			searchString = GUILayout.TextField(searchString, EditorStyles.toolbarSearchField, GUILayout.MaxWidth(maxWidth));
			if(DrawEndCap(searchString) || IsEscapePressed())
			{
				GUI.FocusControl(null);
				onClear.Invoke();
				GUIUtility.ExitGUI();
				searchString = string.Empty;
			}
			return searchString;
		}

		private static bool DrawEndCap(string searchString)
		{
			return GUILayout.Button(string.Empty, string.IsNullOrEmpty(searchString) ? endCapEmpty : endCap);
		}

		private static bool IsEscapePressed()
		{
			return Event.current.rawType == EventType.KeyUp && Event.current.keyCode == KeyCode.Escape;
		}
		#endregion
	}
}