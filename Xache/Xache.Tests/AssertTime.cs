using System;
using System.Diagnostics;

using Xunit;

namespace Xache.Tests
{
	public static class AssertTime
	{
		public static void Under(int ms, Action method)
		{
			var stopwatch = Stopwatch.StartNew();
			stopwatch.Start();
			method();
			stopwatch.Stop();

			Assert.True(stopwatch.ElapsedMilliseconds < ms, $"{stopwatch.ElapsedMilliseconds}ms is not under {ms}ms");
		}
	}
}