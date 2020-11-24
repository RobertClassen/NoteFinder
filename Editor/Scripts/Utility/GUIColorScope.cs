namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	internal struct GUIColorScope : IDisposable
	{
		#region Fields
		private Color color;
		#endregion

		#region Properties

		#endregion

		#region Constructors
		public GUIColorScope(Color color)
		{
			this.color = GUI.color;
			GUI.color = color;
		}
		#endregion

		#region Methods
		public void Dispose()
		{
			GUI.color = color;
		}
		#endregion
	}
}