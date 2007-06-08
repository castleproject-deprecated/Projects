package castle.flexbridge.tests.kernel
{
	import flexunit.framework.*;
	import castle.flexbridge.kernel.*;
	import castle.flexbridge.reflection.*;
	import castle.flexbridge.tests.kernel.testResources.*;
	
	public class DefaultKernelTest extends TestCase
	{
		private var kernel:DefaultKernel;
		
		public override function setUp():void
		{
			super.setUp();
			
			kernel = new DefaultKernel();
		}
		
		public function testDefaultComponentModelBuilder():void
		{
			Assert.assertEquals(DefaultComponentModelBuilder,
				ReflectionUtils.getClass(kernel.componentModelBuilder));
		}
		
		public function testSingletonComponentWithNoDependencies():void
		{
			kernel.registerComponent("Greeter", IGreeter, HelloGreeter);
			
			var greeter:IGreeter = IGreeter(kernel.resolve(IGreeter));
			Assert.assertEquals("Hello, World!", greeter.greet("World"));
			
			var sameGreeter:IGreeter = IGreeter(kernel.resolve(IGreeter));
			Assert.assertStrictlyEquals(greeter, sameGreeter);
		}

		public function testTransientComponentWithNoDependencies():void
		{
			kernel.registerComponent("Greeter", IGreeter, HelloGreeter, Lifestyle.TRANSIENT);
			
			var greeter:IGreeter = IGreeter(kernel.resolve(IGreeter));
			Assert.assertEquals("Hello, World!", greeter.greet("World"));
			
			var differentGreeter:IGreeter = IGreeter(kernel.resolve(IGreeter));
			Assert.assertEquals("Hello, World!", differentGreeter.greet("World"));
			
			Assert.assertTrue(greeter !== differentGreeter);
		}
		
		public function testSingletonWithConstructorDependencies():void
		{
			kernel.registerComponent("Greeter", IGreeter, PhraseBookGreeter);
			kernel.registerComponent("PhraseBook", IPhraseBook, FrenchPhraseBook);
			
			var greeter:IGreeter = IGreeter(kernel.resolve(IGreeter));
			Assert.assertEquals("Bonjour, World!", greeter.greet("World"));
		}
	}
}