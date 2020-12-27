namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	internal static class IntExtensions
	{
		#region Fields
		
		#endregion

		#region Properties
		
		#endregion

		#region Constructors
		
		#endregion

		#region Methods
		public static bool IsClamped(this int value, int min, int max, bool isInclusive = true)
		{
			return isInclusive ? min <= value && value <= max : min < value && value < max;
		}
		#endregion
	}
}