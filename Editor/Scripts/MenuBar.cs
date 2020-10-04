namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	[Serializable]
	public class MenuBar
	{
		#region Fields
		[SerializeField]
		ToDoManager toDoManager = null;
		[SerializeField]
		private string searchString = string.Empty;
		#endregion

		#region Properties
		public string SearchString
		{
			get { return searchString; }
			set
			{
				if(value != searchString)
				{
					searchString = value;
				}
			}
		}
		#endregion

		#region Constructors
		public MenuBar(ToDoManager toDoManager)
		{
			this.toDoManager = toDoManager;
		}
		#endregion

		#region Methods
		public void Draw()
		{
			using(new GUILayout.HorizontalScope(EditorStyles.toolbar))
			{
				if(GUILayout.Button("Scan", EditorStyles.toolbarButton))
				{
					toDoManager.ScanAllFiles();
				}

				toDoManager.TagList.DrawToggles();

				GUILayout.FlexibleSpace();
				SearchString = DrawSearchField(SearchString);
			}
		}

		private string DrawSearchField(string searchString)
		{
			searchString = GUILayout.TextField(searchString, EditorStyles.toolbarSearchField, GUILayout.Width(250));
			if(GUILayout.Button(string.Empty, string.IsNullOrEmpty(searchString) ? 
				"ToolbarSeachCancelButtonEmpty" : "ToolbarSeachCancelButton"))
			{
				searchString = string.Empty;
				GUI.FocusControl(null);
			}
			return searchString;
		}
		#endregion
	}
}