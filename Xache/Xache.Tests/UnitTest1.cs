using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Xache.Caching;

using Xunit;

namespace Xache.Tests
{
	public class UnitTest1
	{
		[Fact]
		public void CanBegin() => CachingMechanismBuilder.Begin<string, string>();

		[Fact]
		public void CanChain()
		{
			CachingMechanismBuilder.Begin<string, string>()
									.AddCacher(new ValueRetrieverCache<string, string>((k) => k));
		}

		[Fact]
		public void NotUsingLastItemCache()
			=> Assert.Throws<ArgumentException>(() => CachingMechanismBuilder.Begin<string, string>()
														.AddCacher(new ConcurrentDictionaryCache<string, string>())
														.AddCacher(new DictionaryCache<string, string>())
														.Compile());

		[Theory]
		[InlineData("test")]
		public void CanCompile(string check)
		{
			var compiled = CachingMechanismBuilder.Begin<string, string>()
									.AddCacher(new ValueRetrieverCache<string, string>((k) => k))
									.Compile();

			Assert.Equal(check, compiled.Get(check));
		}

		[Theory]
		[InlineData(800, 80, "test", typeof(ConcurrentDictionaryCache<string, string>))]
		[InlineData(800, 80, "test", typeof(DictionaryCache<string, string>))]
		public void CachingBuilder(int underMsInitial, int underMsNext, string get, Type middlemanType)
		{
			var cache = CachingMechanismBuilder.Begin<string, string>()
							.AddCacher((ICachingMethod<string, string>)Activator.CreateInstance(middlemanType))
							.AddCacher(new ValueRetrieverCache<string, string>((k) => {
								return Hash(k);
							}))
							.Compile();

			AssertTime.Under(underMsInitial, () => cache.Get(get));
			AssertTime.Under(underMsNext, () => cache.Get(get));
		}

		[Fact]
		public void DoesSame()
		{
			var val = new ValueRetrieverCache<string, string>((a) => {
				return a + ".";
			});

			const string test = " j o h n n y j o h n n y ";

			Assert.Equal(val.GetFrom(test), val.GetOrStore(test, () => {
				return "no bad";
			}));
		}

		[Fact]
		public void CachingBuilderFailsOnNoCachers()
			=> Assert.Throws<ArgumentException>(() => CachingMechanismBuilder.Begin<string, string>()
														.Compile());

		private static string Hash(string input)
		{
			System.Threading.Thread.Sleep(100);
			using (var sHA1Managed = (new SHA1Managed()))
			{
				var hash = sHA1Managed.ComputeHash(Encoding.UTF8.GetBytes(input));
				return string.Join("", hash.Select(b => b.ToString("x2")).ToArray());
			}
		}
	}
}