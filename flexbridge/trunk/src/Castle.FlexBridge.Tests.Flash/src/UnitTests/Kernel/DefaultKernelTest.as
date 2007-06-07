package UnitTests.Kernel
{
	import flexunit.framework.TestCase;
	import Castle.FlexBridge.Flash.Kernel.DefaultKernel;
	import Castle.FlexBridge.Flash.Reflection.Void;
	import flexunit.framework.Assert;
	import Castle.FlexBridge.Flash.Kernel.DefaultComponentModelBuilder;
	import Castle.FlexBridge.Flash.Reflection.ReflectionUtils;
	import UnitTests.Kernel.Resources.IGreeter;
	import UnitTests.Kernel.Resources.HelloGreeter;
	import Castle.FlexBridge.Flash.Kernel.Lifestyle;
	import UnitTests.Kernel.Resources.PhraseBookGreeter;
	import UnitTests.Kernel.Resources.IPhraseBook;
	import UnitTests.Kernel.Resources.FrenchPhraseBook;
	
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