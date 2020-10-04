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
		public bool IsEnabled = true;
		[SerializeField]
		public Color Color = Color.white;
		#endregion

		#region Properties
		public string Name
		{ get { return name; } }
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
			IsEnabled = GUILayout.Toggle(IsEnabled, name, EditorStyles.toolbarButton);
		}
		#endregion
	}
}