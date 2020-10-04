namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;

	[CreateAssetMenu(menuName = "TODO Manager/TagList")]
	public class TagList : ScriptableObject
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
			using(new GUILayout.VerticalScope(GUI.skin.box))
			{
				using(GUILayout.ScrollViewScope scrollViewScrope = new GUILayout.ScrollViewScope(scrollPosition))
				{
					scrollPosition = scrollViewScrope.scrollPosition;
				}
				AddTagField();
			}

			foreach(Tag tag in tags)
			{
				using(new GUILayout.HorizontalScope(EditorStyles.helpBox))
				{
					GUILayout.Label(tag.Name);
					GUILayout.FlexibleSpace();
					if(GUILayout.Button("-", EditorStyles.miniButton))
					{
						tags.Remove(tag);
					}
				}
			}
		}

		private void AddTagField()
		{
			using(new GUILayout.HorizontalScope(EditorStyles.helpBox))
			{
				newTagName = EditorGUILayout.TextField(newTagName);
				if(GUILayout.Button("Add", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
				{
					tags.Add(new Tag(newTagName));
					newTagName = string.Empty;
					GUI.FocusControl(null);
				}
			}
		}

		public void DrawToggles()
		{
			foreach(Tag tag in tags)
			{
				tag.Draw();
			}
		}

		public int GetCountByTag(List<Note> notes, Tag tag)
		{
			return notes.Count(note => note.Tag == tag);
		}
		#endregion
	}
}