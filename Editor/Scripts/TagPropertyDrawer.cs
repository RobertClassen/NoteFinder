namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(Tag))]
	public class TagPropertyDrawer : PropertyDrawer
	{
		#region Constants
		private const string nameFieldName = "name";
		private const string enabledFieldName = "isEnabled";
		private const string colorFieldName = "color";
		private const float nameWidth = 100f;
		private const float enabledWidth = 30f;
		private const float colorWidth = 100f;
		#endregion

		#region Fields
		
		#endregion

		#region Properties
		
		#endregion

		#region Constructors
		
		#endregion

		#region Methods
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			using(new PropertyScope(position, label, property))
			{
				SerializedProperty name = property.FindPropertyRelative(nameFieldName);
				SerializedProperty isEnabled = property.FindPropertyRelative(enabledFieldName);
				SerializedProperty color = property.FindPropertyRelative(colorFieldName);

				Rect enabledRect = position.SetWidth(enabledWidth);
				Rect nameRect = position.SetWidth(nameWidth).SetX(enabledRect.xMax);
				Rect colorRect = position.SetWidth(colorWidth).SetX(nameRect.xMax);

				EditorGUI.PropertyField(enabledRect, isEnabled, GUIContent.none, true);
				EditorGUI.PropertyField(nameRect, name, GUIContent.none);
				EditorGUI.PropertyField(colorRect, color, GUIContent.none);
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}
		#endregion
	}
}