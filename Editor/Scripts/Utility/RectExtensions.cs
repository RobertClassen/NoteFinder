namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	internal static class RectExtensions
	{
		#region Fields
		
		#endregion

		#region Properties
		
		#endregion

		#region Constructors
		
		#endregion

		#region Methods
		public static Rect SetPosition(this Rect rect, Vector2 size)
		{
			return new Rect(rect.position, size);
		}

		public static Rect SetPosition(this Rect rect, float x, float y)
		{
			return rect.SetSize(new Vector2(x, y));
		}

		public static Rect SetX(this Rect rect, float x)
		{
			return rect.SetPosition(x, rect.y);
		}

		public static Rect SetY(this Rect rect, float y)
		{
			return rect.SetPosition(rect.x, y);
		}

		public static Rect SetSize(this Rect rect, Vector2 size)
		{
			return new Rect(rect.position, size);
		}

		public static Rect SetSize(this Rect rect, float width, float height)
		{
			return rect.SetSize(new Vector2(width, height));
		}

		public static Rect SetWidth(this Rect rect, float width)
		{
			return rect.SetSize(width, rect.height);
		}

		public static Rect SetHeight(this Rect rect, float height)
		{
			return rect.SetSize(rect.width, height);
		}
		#endregion
	}
}