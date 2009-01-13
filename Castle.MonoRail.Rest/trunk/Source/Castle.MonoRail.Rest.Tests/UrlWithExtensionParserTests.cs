using MbUnit.Framework;

namespace Castle.MonoRail.Rest.Tests
{
	[TestFixture]
	public class UrlWithExtensionParserTests
	{
		UrlWithExtensionParser parser;

		[SetUp]
		public void SetUp()
		{
			parser = new UrlWithExtensionParser();
		}

		[Test]
		public void WithUrlThatHasNoExtension_ShouldReturnCorrectParts()
		{
			var url = "/path/to/resource";
			var parts = parser.GetParts(url);

			Assert.AreEqual(3, parts.Length);
			Assert.AreEqual("path", parts[0]);
			Assert.AreEqual("to", parts[1]);
			Assert.AreEqual("resource", parts[2]);
		}

		[Test]
		public void WithUrlThatHasASingleDotInTheFinalSegment_ShouldReturnCorrectParts()
		{
			var url = "/path/to/resource.json";
			var parts = parser.GetParts(url);

			Assert.AreEqual(4, parts.Length);
			Assert.AreEqual("path", parts[0]);
			Assert.AreEqual("to", parts[1]);
			Assert.AreEqual("resource", parts[2]);
			Assert.AreEqual("json", parts[3]);
		}

		[Test]
		public void WithUrlThatHasMultipleDotsInTheFinalSegment_ShouldReturnCorrectParts()
		{
			var url = "/path/to/resource.with.dots.json";
			var parts = parser.GetParts(url);

			Assert.AreEqual(4, parts.Length);
			Assert.AreEqual("path", parts[0]);
			Assert.AreEqual("to", parts[1]);
			Assert.AreEqual("resource.with.dots", parts[2]);
			Assert.AreEqual("json", parts[3]);
		}

		[Test]
		public void WithUrlThatHasMultipleDotsInEarlierSegments_ShouldReturnCorrectParts()
		{
			var url = "/path/with.dots/leading.to/resource";
			var parts = parser.GetParts(url);

			Assert.AreEqual(4, parts.Length);
			Assert.AreEqual("path", parts[0]);
			Assert.AreEqual("with.dots", parts[1]);
			Assert.AreEqual("leading.to", parts[2]);
			Assert.AreEqual("resource", parts[3]);
		}
	}
}
