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
		NoteFinder noteFinder = null;
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
		public MenuBar(NoteFinder noteFinder)
		{
			this.noteFinder = noteFinder;
		}
		#endregion

		#region Methods
		public void Draw()
		{
			using(new GUILayout.HorizontalScope(EditorStyles.toolbar))
			{
				if(GUILayout.Button("Scan", EditorStyles.toolbarButton))
				{
					noteFinder.ScanAllFiles();
				}

				if(GUILayout.Button("Tags", EditorStyles.toolbarButton))
				{
					noteFinder.TagList.DrawMenu(noteFinder);
				}

				GUILayout.FlexibleSpace();
				searchString = SearchField.Draw(searchString, noteFinder.Repaint);
			}
		}
		#endregion
	}
}