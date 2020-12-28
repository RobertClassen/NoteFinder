namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[Serializable]
	public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
	{
		#region Fields
		[SerializeField]
		private List<TKey> keys = new List<TKey>();
		[SerializeField]
		private List<TValue> values = new List<TValue>();
		#endregion

		#region Properties
		
		#endregion

		#region Constructors
		
		#endregion

		#region Methods
		public void OnBeforeSerialize()
		{
			keys.Clear();
			values.Clear();
			foreach(KeyValuePair<TKey, TValue> entry in this)
			{
				keys.Add(entry.Key);
				values.Add(entry.Value);
			}
		}

		public void OnAfterDeserialize()
		{
			Clear();
			for(int i = 0; i < keys.Count; i++)
			{
				Add(keys[i], values[i]);
			}
		}
		#endregion
	}
}