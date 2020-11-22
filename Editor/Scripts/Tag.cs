namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	[Serializable]
	public class Tag
	{
		#region Fields
		[SerializeField]
		private string name = null;
		[SerializeField]
		private bool isEnabled = true;
		[SerializeField]
		private Color color = Color.white;
		#endregion

		#region Properties
		public string Name
		{ get { return name; } }

		public bool IsEnabled
		{ get { return isEnabled; } }

		public Color Color
		{ get { return color; } }
		#endregion

		#region Constructors
		public Tag(string name)
		{
			this.name = name;
		}
		#endregion

		#region Methods
		public void Draw(TagList tagList)
		{
			string newName = GUILayout.TextField(name, GUILayout.Width(100f));
			if(newName != name)
			{
				Undo.RegisterCompleteObjectUndo(tagList, "Changed Tag Name");
				name = newName;
				EditorUtility.SetDirty(tagList);
			}
			Color newColor = EditorGUILayout.ColorField(color);
			if(newColor != color)
			{
				Undo.RegisterCompleteObjectUndo(tagList, "Changed Tag Color");
				color = newColor;
				EditorUtility.SetDirty(tagList);
			}
		}

		public void Toggle()
		{
			isEnabled = !isEnabled;
		}
		#endregion
	}
}