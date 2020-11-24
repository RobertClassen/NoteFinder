namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	internal static class LayoutGroup
	{
		#region Enums
		public enum Direction
		{
			Horizontal,
			Vertical,
		}
		#endregion

		public struct Scope : IDisposable
		{
			#region Fields
			private LayoutGroup.Direction direction;
			#endregion

			#region Properties

			#endregion

			#region Constructors
			private Scope(Direction direction)
			{
				this.direction = direction;
			}

			public Scope(Direction direction, params GUILayoutOption[] options) :
				this(direction)
			{
				if(direction == Direction.Horizontal)
				{
					GUILayout.BeginHorizontal(options);
				}
				else
				{
					GUILayout.BeginVertical(options);
				}
			}

			public Scope(Direction direction, GUIStyle style, params GUILayoutOption[] options) :
				this(direction)
			{
				if(direction == Direction.Horizontal)
				{
					GUILayout.BeginHorizontal(style, options);
				}
				else
				{
					GUILayout.BeginVertical(style, options);
				}
			}

			public Scope(Direction direction, string text, GUIStyle style, params GUILayoutOption[] options) :
				this(direction)
			{
				if(direction == Direction.Horizontal)
				{
					GUILayout.BeginHorizontal(text, style, options);
				}
				else
				{
					GUILayout.BeginVertical(text, style, options);
				}
			}

			public Scope(Direction direction, Texture image, GUIStyle style, params GUILayoutOption[] options) :
				this(direction)
			{
				if(direction == Direction.Horizontal)
				{
					GUILayout.BeginHorizontal(image, style, options);
				}
				else
				{
					GUILayout.BeginVertical(image, style, options);
				}
			}

			public Scope(Direction direction, GUIContent content, GUIStyle style, params GUILayoutOption[] options) :
				this(direction)
			{
				if(direction == Direction.Horizontal)
				{
					GUILayout.BeginHorizontal(content, style, options);
				}
				else
				{
					GUILayout.BeginVertical(content, style, options);
				}
			}
			#endregion

			#region Methods
			public void Dispose()
			{
				if(direction == Direction.Horizontal)
				{
					GUILayout.EndHorizontal();
				}
				else
				{
					GUILayout.EndVertical();
				}
			}
			#endregion
		}
	}
}