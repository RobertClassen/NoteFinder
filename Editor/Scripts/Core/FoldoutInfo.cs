namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	[Serializable]
	public class FoldoutInfo
	{
		#region Constants
		private const string folderIconName = "Folder Icon";
		#if UNITY_2019_3_OR_NEWER
		private const string folderOpenedIconName = "FolderOpened Icon";
		#else
		private const string folderOpenedIconName = "FolderEmpty Icon";
		#endif
		#endregion

		#region Fields
		[SerializeField]
		private int hash = 0;
		[SerializeField]
		private GUIContent label = null;
		[SerializeField]
		private GUIContent labelOpened = null;
		#endregion

		#region Properties
		public int Hash
		{ get { return hash; } }
		#endregion

		#region Constructors
		public FoldoutInfo(int hash, string displayName, bool isDirectory)
		{
			this.hash = hash;
			if(isDirectory)
			{
				label = EditorGUIUtility.TrTextContentWithIcon(displayName, folderIconName);
				labelOpened = EditorGUIUtility.TrTextContentWithIcon(displayName, folderOpenedIconName);
			}
			else
			{
				labelOpened = label = EditorGUIUtility.TrTextContentWithIcon(displayName, NoteFinder.ScriptIconName);
			}
		}
		#endregion

		#region Methods
		public GUIContent GetLabel(bool isExpanded)
		{
			return isExpanded ? labelOpened : label;
		}
		#endregion
	}
}