namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;

	[CreateAssetMenu(menuName = "NoteFinder/TagList")]
	public class TagList : ScriptableObject, IDrawable
	{
		#region Fields
		[SerializeField]
		private List<Tag> tags = new List<Tag>();
		[NonSerialized]
		private Vector2 scrollPosition = Vector2.zero;
		[NonSerialized]
		private string newTagName = string.Empty;
		#endregion

		#region Properties
		public List<Tag> Tags
		{ get { return tags; } }
		#endregion

		#region Constructors
		
		#endregion

		#region Methods
		public void Draw()
		{
			GUILayout.Label("Tags");
			using(GUILayout.ScrollViewScope scrollViewScrope = new GUILayout.ScrollViewScope(scrollPosition))
			{
				scrollPosition = scrollViewScrope.scrollPosition;

				for(int i = 0; i < tags.Count; i++)
				{
					using(new GUILayout.HorizontalScope(GUI.skin.box))
					{
						tags[i].Draw(this);
						GUILayout.FlexibleSpace();
						if(GUILayout.Button("-"))
						{
							tags.RemoveAt(i);
						}
					}
				}
			}
			AddTagField();
		}

		private void AddTagField()
		{
			using(new GUILayout.HorizontalScope(EditorStyles.helpBox))
			{
				newTagName = EditorGUILayout.TextField(newTagName);
				if(GUILayout.Button("Add", GUILayout.ExpandWidth(false)))
				{
					tags.Add(new Tag(newTagName));
					newTagName = string.Empty;
					GUI.FocusControl(null);
				}
			}
		}

		public void DrawMenu(NoteFinder noteFinder)
		{
			GenericMenu menu = new GenericMenu();

			foreach(Tag tag in tags)
			{
				menu.AddItem(string.Format("{0} ({1})", tag.Name, noteFinder.NoteList.Notes
					.Count(note => note.Tag == tag)), tag.IsEnabled, tag.Toggle);
			}
			menu.AddSeparator();
			menu.AddItem("Edit...", false, () => noteFinder.IDrawable = this);
			menu.ShowAsContext();
		}

		public int GetCountByTag(List<Note> notes, Tag tag)
		{
			return notes.Count(note => note.Tag == tag);
		}
		#endregion
	}
}