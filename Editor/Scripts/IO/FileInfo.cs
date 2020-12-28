namespace Notes.IO
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using UnityEngine;

	[Serializable]
	public class FileInfo
	{
		#region Fields
		[SerializeField]
		private string path = null;
		[SerializeField]
		private long length = 0L;
		[SerializeField]
		private long lastWriteTime = 0L;
		#endregion

		#region Properties
		public string Path
		{ get { return path; } }

		public bool Exists
		{ get { return File.Exists(path); } }

		public bool IsModified
		{
			get
			{
				long oldLength = length;
				long oldLastWriteTime = lastWriteTime;
				Update();
				return oldLength != length || oldLastWriteTime != lastWriteTime;
			}
		}
		#endregion

		#region Constructors
		public FileInfo(string path)
		{
			this.path = path;
			Update();
		}
		#endregion

		#region Methods
		private void Update()
		{
			System.IO.FileInfo info = new System.IO.FileInfo(path);
			length = info.Length;
			lastWriteTime = info.LastWriteTime.ToBinary();
		}
		#endregion
	}
}