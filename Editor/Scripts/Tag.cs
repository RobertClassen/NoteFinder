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
		public Color Color = Color.white;
		#endregion

		#region Properties
		public string Name
		{ get { return name; } }

		public bool IsEnabled
		{ get { return isEnabled; } }
		#endregion

		#region Constructors
		public Tag(string name)
		{
			this.name = name;
		}
		#endregion

		#region Methods
		public void Draw()
		{
			name = GUILayout.TextField(name, GUILayout.Width(100f));
			Color = EditorGUILayout.ColorField(Color);
		}

		public void Toggle()
		{
			isEnabled = !isEnabled;
		}
		#endregion
	}
}