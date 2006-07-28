namespace Generator.Tests.Functional

import Generator
import Generator.Extentions
import NUnit.Framework

[TestFixture]
class ControllerGeneratorTest(GeneratorTestCase):

	[Test]
	def Usage():
		Assert.AreEqual(-1, Main(("controller",)))

	[Test, Ignore("TODO: do work from cmd line")]
	def Generate():
		Assert.AreEqual(0, Main(("controller", "Test")))