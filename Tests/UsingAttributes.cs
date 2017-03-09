using System.Reflection;
using NUnit.Framework;
using PRI.PrereleaseAttributes;
using Tests.Mocks;

namespace Tests
{
	[TestFixture]
	public class UsingAttributes
	{
		[Test]
		public void PrereleaseWithoutMessageSucceeds()
		{
			var attribute = typeof(Prerelease).GetCustomAttribute<PrereleaseAttribute>();
			Assert.IsNotNull(attribute);
			Assert.IsNull(attribute.Message);
		}
		[Test]
		public void PrereleaseWithMessageSucceeds()
		{
			var attribute = typeof(NotedPrerelease).GetCustomAttribute<PrereleaseAttribute>();
			Assert.IsNotNull(attribute);
			Assert.AreEqual("prerelease", attribute.Message);
		}
		[Test]
		public void PreviewWithoutMessageSucceeds()
		{
			var attribute = typeof(Preview).GetCustomAttribute<PreviewAttribute>();
			Assert.IsNotNull(attribute);
			Assert.IsNull(attribute.Message);
		}
		[Test]
		public void PreviewWithMessageSucceeds()
		{
			var attribute = typeof(NotedPreview).GetCustomAttribute<PreviewAttribute>();
			Assert.IsNotNull(attribute);
			Assert.AreEqual("preview", attribute.Message);
		}
		[Test]
		public void ExperimentalWithoutMessageSucceeds()
		{
			var attribute = typeof(Experimental).GetCustomAttribute<ExperimentalAttribute>();
			Assert.IsNotNull(attribute);
			Assert.IsNull(attribute.Message);
		}
		[Test]
		public void ExperimentalWithMessageSucceeds()
		{
			var attribute = typeof(NotedExperimental).GetCustomAttribute<ExperimentalAttribute>();
			Assert.IsNotNull(attribute);
			Assert.AreEqual("experimental", attribute.Message);
		}
		[Test]
		public void AlphaWithoutMessageSucceeds()
		{
			var attribute = typeof(Alpha).GetCustomAttribute<AlphaAttribute>();
			Assert.IsNotNull(attribute);
			Assert.IsNull(attribute.Message);
		}
		[Test]
		public void AlphaWithMessageSucceeds()
		{
			var attribute = typeof(NotedAlpha).GetCustomAttribute<AlphaAttribute>();
			Assert.IsNotNull(attribute);
			Assert.AreEqual("alpha", attribute.Message);
		}
	}
}
