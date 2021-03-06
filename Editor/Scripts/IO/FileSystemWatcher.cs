namespace Notes.IO
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using UnityEngine;

	[Serializable]
	public class FileSystemWatcher
	{
		#region Fields
		[SerializeField]
		private string path = null;
		[SerializeField]
		private string fileExtension = null;

		[SerializeField]
		private StringFileInfoDictionary fileInfos = new StringFileInfoDictionary();
		#endregion

		#region Properties
		
		#endregion

		#region Constructors
		public FileSystemWatcher(string path, string fileExtension, Action<string> onFileModified)
		{
			this.path = path;
			this.fileExtension = fileExtension;

			AddNewFiles(onFileModified);
		}
		#endregion

		#region Methods
		private void RemoveDeletedFiles(Action<string> onFileModified)
		{
			string[] filePaths = fileInfos.Keys.ToArray();
			for(int i = 0; i < filePaths.Length; i++)
			{
				if(!File.Exists(filePaths[i]))
				{
					fileInfos.Remove(filePaths[i]);
					onFileModified?.Invoke(filePaths[i]);
				}
			}
		}

		private void AddNewFiles(Action<string> onFileModified)
		{
			foreach(string filePath in Directory.EnumerateFiles(path, fileExtension, SearchOption.AllDirectories))
			{
				if(!fileInfos.ContainsKey(filePath))
				{
					fileInfos.Add(filePath, new FileInfo(filePath));
					onFileModified?.Invoke(filePath);
				}
			}
		}

		public void ValidateFiles(Action<string> onFileModified)
		{
			RemoveDeletedFiles(onFileModified);
			AddNewFiles(onFileModified);

			foreach(KeyValuePair<string, FileInfo> fileInfo in fileInfos)
			{
				if(fileInfo.Value.IsModified)
				{
					onFileModified?.Invoke(fileInfo.Value.Path);
				}
			}
		}
		#endregion
	}
}