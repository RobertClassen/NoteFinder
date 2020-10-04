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

				if(GUILayout.Button("Tags", EditorStyles.toolbarButton))
				{
					toDoManager.TagList.DrawMenu(toDoManager);
				}

				GUILayout.FlexibleSpace();
				DrawSearchField();
			}
		}

		private void DrawSearchField()
		{
			searchString = GUILayout.TextField(searchString, EditorStyles.toolbarSearchField, GUILayout.Width(250));
			if(GUILayout.Button(string.Empty, string.IsNullOrEmpty(searchString) ? 
				"ToolbarSeachCancelButtonEmpty" : "ToolbarSeachCancelButton") ||
			   Event.current.rawType == EventType.KeyUp && Event.current.keyCode == KeyCode.Escape)
			{
				searchString = string.Empty;
				GUI.FocusControl(null);
				EditorApplication.delayCall += toDoManager.Repaint;
				GUIUtility.ExitGUI();
			}
		}
		#endregion
	}
}