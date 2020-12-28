namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	internal struct ScrollViewScope : IDisposable
	{
		#region Fields
		
		#endregion

		#region Properties
		
		#endregion

		#region Constructors
		public ScrollViewScope(ref Vector2 scrollPosition, params GUILayoutOption[] options)
		{
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, options);
		}

		public ScrollViewScope(ref Vector2 scrollPosition, GUIStyle style, params GUILayoutOption[] options)
		{
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, style, options);
		}

		public ScrollViewScope(ref Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, 
			params GUILayoutOption[] options)
		{
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, options);
		}

		public ScrollViewScope(ref Vector2 scrollPosition, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, 
			params GUILayoutOption[] options)
		{
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, horizontalScrollbar, verticalScrollbar, options);
		}

		public ScrollViewScope(ref Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, 
			GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, params GUILayoutOption[] options)
		{
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, 
				horizontalScrollbar, verticalScrollbar, options);
		}

		public ScrollViewScope(ref Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, 
			GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background, params GUILayoutOption[] options)
		{
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, 
				horizontalScrollbar, verticalScrollbar, background, options);
		}
		#endregion

		#region Methods
		public void Dispose()
		{
			GUILayout.EndScrollView();
		}
		#endregion
	}
}