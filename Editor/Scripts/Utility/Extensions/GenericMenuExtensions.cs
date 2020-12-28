namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	internal static class GenericMenuExtensions
	{
		#region Fields

		#endregion

		#region Properties

		#endregion

		#region Constructors

		#endregion

		#region Methods
		public static void AddItem(this GenericMenu genericMenu, GUIContent content, bool isTicked, 
			GenericMenu.MenuFunction func, bool isEnabled = true)
		{
			if(isEnabled)
			{
				genericMenu.AddItem(content, isTicked, func);
			}
			else
			{
				genericMenu.AddDisabledItem(content);
			}
		}

		public static void AddItem(this GenericMenu genericMenu, string content, bool isTicked, 
			GenericMenu.MenuFunction func, bool isEnabled = true)
		{
			AddItem(genericMenu, new GUIContent(content), isTicked, func, isEnabled);
		}

		public static void AddItem(this GenericMenu genericMenu, GUIContent content, bool isTicked, 
			GenericMenu.MenuFunction2 func, object userData, bool isEnabled = true)
		{
			if(isEnabled)
			{
				genericMenu.AddItem(content, isTicked, func, userData);
			}
			else
			{
				genericMenu.AddDisabledItem(content);
			}
		}

		public static void AddItem(this GenericMenu genericMenu, string content, bool isTicked, 
			GenericMenu.MenuFunction2 func, object userData, bool isEnabled = true)
		{
			AddItem(genericMenu, new GUIContent(content), isTicked, func, userData, isEnabled);
		}

		public static void AddSeparator(this GenericMenu genericMenu)
		{
			genericMenu.AddSeparator(string.Empty);
		}

		public static void DropDown(this GenericMenu genericMenu, Rect menuBarRect, Rect menuRect)
		{
			genericMenu.DropDown(
				new Rect(menuBarRect.x + menuRect.x, menuBarRect.yMax, menuRect.width, menuRect.height));
		}
		#endregion
	}
}