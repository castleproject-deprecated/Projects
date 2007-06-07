package UnitTests.Kernel.Resources
{
	public class HelloGreeter implements IGreeter
	{
		public function greet(name:String):String
		{
			return "Hello, " + name + "!";
		}
	}
}