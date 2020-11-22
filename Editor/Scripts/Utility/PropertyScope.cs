namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	public struct PropertyScope : IDisposable
	{
		#region Fields

		#endregion

		#region Properties
		
		#endregion

		#region Constructors
		public PropertyScope(Rect rect, GUIContent label, SerializedProperty property)
		{
			GUIContent content = EditorGUI.BeginProperty(rect, label, property);
			label.image = content.image;
			label.text = content.text;
			label.tooltip = content.tooltip;
		}
		#endregion

		#region Methods
		public void Dispose()
		{
			EditorGUI.EndProperty();
		}
		#endregion
	}
}