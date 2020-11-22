namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public static class RectExtensions
	{
		#region Fields
		
		#endregion

		#region Properties
		
		#endregion

		#region Constructors
		
		#endregion

		#region Methods
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
			return new Rect(rect.x, rect.y, width, rect.height);
		}

		public static Rect SetX(this Rect rect, float x)
		{
			return new Rect(x, rect.y, rect.width, rect.height);
		}
		#endregion
	}
}