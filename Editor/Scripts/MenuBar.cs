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
		[SerializeField]
		private string lowerSearchString = string.Empty;

		[SerializeField]
		private Rect rect;
		[SerializeField]
		private Rect tagMenuRect;
		#endregion

		#region Properties
		public string LowerSearchString
		{ get { return lowerSearchString; } }
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
			using(new LayoutGroup.Scope(LayoutGroup.Direction.Horizontal, EditorStyles.toolbar))
			{
				if(GUILayout.Button("Scan", EditorStyles.toolbarButton))
				{
					noteFinder.ScanAllFiles();
				}

				DrawTagMenu();

				GUILayout.FlexibleSpace();
				SearchField.Draw(ref searchString, noteFinder.Repaint);
				lowerSearchString = searchString.ToLowerInvariant();
			}

			rect = GUILayoutUtility.GetLastRect();
		}

		private void DrawTagMenu()
		{
			if(GUILayout.Button("Tags", EditorStyles.toolbarButton))
			{
				GenericMenu menu = new GenericMenu();
				foreach(Tag tag in noteFinder.TagList.Tags)
				{
					menu.AddItem(string.Format("{0} ({1})", tag.Name, noteFinder.NoteListCollection.GetTagCount(tag)), 
						tag.IsEnabled, tag.Toggle);
				}
				menu.AddSeparator();
				menu.AddItem("Edit...", false, () => noteFinder.IDrawable = noteFinder.TagList);
				menu.DropDown(rect, tagMenuRect);
			}

			if(Event.current.type == EventType.Layout || Event.current.type == EventType.Used)
			{
				return;
			}
			tagMenuRect = GUILayoutUtility.GetLastRect();
		}

		public void SetFilter(string filter)
		{
			searchString = filter;
			lowerSearchString = searchString.ToLowerInvariant();
			noteFinder.Repaint();
		}
		#endregion
	}
}