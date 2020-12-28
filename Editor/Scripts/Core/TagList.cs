namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	[CreateAssetMenu(menuName = "NoteFinder/TagList")]
	[Serializable]
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
			using(new ScrollViewScope(ref scrollPosition))
			{
				for(int i = 0; i < tags.Count; i++)
				{
					using(new LayoutGroup.Scope(LayoutGroup.Direction.Horizontal, GUI.skin.box))
					{
						tags[i].DrawSettings(this);
						GUILayout.FlexibleSpace();
						if(GUILayout.Button("-"))
						{
							tags.RemoveAt(i);
						}
					}
				}
			}
			DrawAddTagField();
		}

		private void DrawAddTagField()
		{
			using(new LayoutGroup.Scope(LayoutGroup.Direction.Horizontal, EditorStyles.helpBox))
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
		#endregion
	}
}