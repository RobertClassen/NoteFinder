namespace Notes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	internal static class ListExtensions
	{
		#region Fields
		
		#endregion

		#region Properties
		
		#endregion

		#region Constructors
		
		#endregion

		#region Methods
		public static void AddSorted<T>(this List<T> list, T item) where T : IComparable<T>
		{
			int index = list.BinarySearch(item);
			if(index < 0)
			{
				index = ~index;
			}
			list.Insert(index, item);
		}

		public static void SetOrAddSorted<T>(this List<T> list, Predicate<T> match, T item) where T : IComparable<T>
		{
			if(!list.TrySet(match, item))
			{
				list.AddSorted(item);
			}
		}

		public static bool TryRemove<T>(this List<T> list, Predicate<T> match) where T : IComparable<T>
		{
			int index = list.FindIndex(match);
			if(index >= 0)
			{
				list.RemoveAt(index);
				return true;
			}
			return false;
		}

		public static bool TrySet<T>(this List<T> list, Predicate<T> match, T item) where T : IComparable<T>
		{
			int index = list.FindIndex(match);
			if(index.IsClamped(0, list.Count - 1))
			{
				list[index] = item;
				return true;
			}
			return false;
		}
		#endregion
	}
}