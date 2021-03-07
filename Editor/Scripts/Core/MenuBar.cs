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
		private SearchField searchField = new SearchField();

		[SerializeField]
		private Rect rect;
		[SerializeField]
		private Rect tagMenuRect;
		#endregion

		#region Properties
		public SearchField SearchField
		{ get { return searchField; } }
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
			using(new LayoutGroup.HorizontalScope(EditorStyles.toolbar))
			{
				DrawUpdateButton();
				DrawTagMenu();
				GUILayout.FlexibleSpace();
				SearchField.Draw(noteFinder.Repaint);
			}
			rect = GUILayoutUtility.GetLastRect();
		}

		private void DrawUpdateButton()
		{
			if(GUILayout.Button("Update", EditorStyles.toolbarButton))
			{
				noteFinder.ParseAll();
			}
		}

		private void DrawTagMenu()
		{
			if(GUILayout.Button("Tags", EditorStyles.toolbarButton))
			{
				GenericMenu menu = new GenericMenu();
				for(int i = 0; i < noteFinder.TagList.Tags.Count; i++)
				{
					Tag tag = noteFinder.TagList.Tags[i];
					menu.AddItem(string.Format("{0} ({1})", tag.Name, noteFinder.GetTagCount(i)), 
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
			searchField.Text = filter;
			noteFinder.Repaint();
		}
		#endregion
	}
}