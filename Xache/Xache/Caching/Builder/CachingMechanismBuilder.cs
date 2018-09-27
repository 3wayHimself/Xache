﻿using System;

namespace Xache.Caching
{
	/// <summary>
	/// Assists in fluently building a CacheMechanism
	/// </summary>
	/// <autogeneratedoc />
	public static class CachingMechanismBuilder
	{
		/// <summary>Begins an instance of a CachingBuilder to fluently build a CacheMechanism.</summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <returns></returns>
		/// <autogeneratedoc />
		public static CachingBuilder<TKey, TValue> Begin<TKey, TValue>()
			=> new CachingBuilder<TKey, TValue>();

		/// <summary>Add a type of cacher to the CachingBuilder.</summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="cm">The caching builder.</param>
		/// <param name="cachingMethod">The caching method.</param>
		/// <returns></returns>
		/// <autogeneratedoc />
		public static CachingBuilder<TKey, TValue> AddCacher<TKey, TValue>(this CachingBuilder<TKey, TValue> cm, ICachingMethod<TKey, TValue> cachingMethod)
		{
			cm._cachingMethods.Add(cachingMethod);
			return cm;
		}

		/// <summary>Compiles the CachingBuilder into a CachingMechanism.</summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="cb">The CachingBuilder.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentException">No caching methods added</exception>
		/// <autogeneratedoc />
		public static CacheMechanism<TKey, TValue> Compile<TKey, TValue>(this CachingBuilder<TKey, TValue> cb)
		{
			if (cb._cachingMethods.Count < 1) throw new ArgumentException("No caching methods added");

			var lastIndex = cb._cachingMethods.Count - 1;

			var lastItem = cb._cachingMethods[lastIndex];

			if (!(lastItem is ILastCachingMethod<TKey, TValue> lcm)) throw new ArgumentException($"The last item in the chain MUST be an {typeof(ILastCachingMethod<TKey, TValue>)}");
				
			var last = lcm
						.GetAsLast();

			for (var i = lastIndex - 1; i >= 0; i--)
				last = cb._cachingMethods[i]
						.GetAnother(last);

			return new CacheMechanism<TKey, TValue>(last);
		}

		private static Func<TKey, TValue> GetAnother<TKey, TValue>(this ICachingMethod<TKey, TValue> cm, Func<TKey, TValue> func)
			=>
				(k) => cm.GetOrStore(k,
					() => func(k));

		private static Func<TKey, TValue> GetAsLast<TKey, TValue>(this ILastCachingMethod<TKey, TValue> cm)
			=>
				(k) => cm.GetFrom(k);
	}
}