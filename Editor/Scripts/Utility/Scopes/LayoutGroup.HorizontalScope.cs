namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	internal static partial class LayoutGroup
	{
		public struct HorizontalScope : IDisposable
		{
			#region Fields
			private bool isActive;
			#endregion

			#region Properties

			#endregion

			#region Constructors
			private HorizontalScope(bool isActive)
			{
				this.isActive = isActive;
			}

			public HorizontalScope(bool isActive, 
				params GUILayoutOption[] options) : this(isActive)
			{
				GUILayout.BeginHorizontal(options);
			}

			public HorizontalScope(GUIStyle style, bool isActive = true, 
				params GUILayoutOption[] options) : this(isActive)
			{
				GUILayout.BeginHorizontal(style, options);
			}

			public HorizontalScope(string text, GUIStyle style, bool isActive = true, 
				params GUILayoutOption[] options) : this(isActive)
			{
				GUILayout.BeginHorizontal(text, style, options);
			}

			public HorizontalScope(Texture image, GUIStyle style, bool isActive = true, 
				params GUILayoutOption[] options) : this(isActive)
			{
				GUILayout.BeginHorizontal(image, style, options);
			}

			public HorizontalScope(GUIContent content, GUIStyle style, bool isActive = true, 
				params GUILayoutOption[] options) : this(isActive)
			{
				GUILayout.BeginHorizontal(content, style, options);
			}
			#endregion

			#region Methods
			public void Dispose()
			{
				if(isActive)
				{
					GUILayout.EndHorizontal();
				}
			}
			#endregion
		}
	}
}