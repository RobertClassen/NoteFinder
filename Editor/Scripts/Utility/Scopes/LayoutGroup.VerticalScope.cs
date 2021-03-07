namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	internal static partial class LayoutGroup
	{
		public struct VerticalScope : IDisposable
		{
			#region Fields
			private bool isActive;
			#endregion

			#region Properties

			#endregion

			#region Constructors
			private VerticalScope(bool isActive)
			{
				this.isActive = isActive;
			}

			public VerticalScope(bool isActive, 
				params GUILayoutOption[] options) : this(isActive)
			{
				GUILayout.BeginVertical(options);
			}

			public VerticalScope(GUIStyle style, bool isActive = true, 
				params GUILayoutOption[] options) : this(isActive)
			{
				GUILayout.BeginVertical(style, options);
			}

			public VerticalScope(string text, GUIStyle style, bool isActive = true, 
				params GUILayoutOption[] options) : this(isActive)
			{
				GUILayout.BeginVertical(text, style, options);
			}

			public VerticalScope(Texture image, GUIStyle style, bool isActive = true, 
				params GUILayoutOption[] options) : this(isActive)
			{
				GUILayout.BeginVertical(image, style, options);
			}

			public VerticalScope(GUIContent content, GUIStyle style, bool isActive = true, 
				params GUILayoutOption[] options) : this(isActive)
			{
				GUILayout.BeginVertical(content, style, options);
			}
			#endregion

			#region Methods
			public void Dispose()
			{
				if(isActive)
				{
					GUILayout.EndVertical();
				}
			}
			#endregion
		}
	}
}