namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public static class StringExtensions
	{
		#region Fields
		
		#endregion

		#region Properties
		
		#endregion

		#region Constructors
		
		#endregion

		#region Methods
		public static string[] Split(this string text, char separator)
		{
			return text.Split(new[]{ separator }, StringSplitOptions.RemoveEmptyEntries);
		}
		#endregion
	}
}