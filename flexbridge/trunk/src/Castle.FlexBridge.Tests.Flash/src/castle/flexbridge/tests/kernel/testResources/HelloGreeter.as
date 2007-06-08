package castle.flexbridge.tests.kernel.testResources
{
	public class HelloGreeter implements IGreeter
	{
		public function greet(name:String):String
		{
			return "Hello, " + name + "!";
		}
	}
}