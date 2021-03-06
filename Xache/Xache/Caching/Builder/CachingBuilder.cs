﻿using System.Collections.Generic;

namespace Xache.Caching
{
	/// <summary>
	/// A helper class to assist in fluently building a CacheMechanism
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	/// <autogeneratedoc />
	public class CachingBuilder<TKey, TValue>
	{
		public CachingBuilder() => _cachingMethods = new List<ICachingMethod<TKey, TValue>>();

		internal List<ICachingMethod<TKey, TValue>> _cachingMethods { get; set; }
	}
}