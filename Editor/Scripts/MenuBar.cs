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
		private NoteFinder noteFinder = null;
		[SerializeField]
		private string searchString = string.Empty;
		#endregion

		#region Properties
		public string SearchString
		{ get { return searchString; } }
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
					DrawTagMenu(noteFinder);
				}

				GUILayout.FlexibleSpace();
				searchString = SearchField.Draw(searchString, noteFinder.Repaint);
			}
		}

		private void DrawTagMenu(NoteFinder noteFinder)
		{
			GenericMenu menu = new GenericMenu();

			foreach(Tag tag in noteFinder.TagList.Tags)
			{
				menu.AddItem(string.Format("{0} ({1})", tag.Name, noteFinder.NoteListCollection.GetTagCount(tag)), 
					tag.IsEnabled, tag.Toggle);
			}
			menu.AddSeparator();
			menu.AddItem("Edit...", false, () => noteFinder.IDrawable = noteFinder.TagList);
			menu.ShowAsContext();
		}
		#endregion
	}
}