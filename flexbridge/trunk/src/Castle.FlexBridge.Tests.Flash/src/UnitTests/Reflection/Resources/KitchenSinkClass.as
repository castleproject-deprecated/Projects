package UnitTests.Reflection.Resources
{
	public class KitchenSinkClass extends EmptyClass implements EmptyInterface
	{
		public function KitchenSinkClass(a:int, b:*, c:String = null)
		{
			throw new Error("Exception thrown to ensure that the robustDescribeType() hack "
				+ "can handle the case properly when it applies the workaround for missing "
				+ "constructor parameter types.");
		}
		
		public const instanceConstant:int = 0;
		public static const staticConstant:int = 0;
		
		public var instanceField:int = 0;		
		public static var staticField:int = 0;

		public function simpleInstanceMethod():void { }
		public function complexInstanceMethod(a:int, b:*, c:String = null):Object { return null; }		
		public static function simpleStaticMethod():void { }
		public static function complexStaticMethod(a:int, b:*, c:String = null):Object { return null; }
		
		public function get readOnlyInstanceProperty():int { return 0; }		
		public function get readWriteInstanceProperty():int { return 0; }
		public function set readWriteInstanceProperty(value:int):void { }
		public function set writeOnlyInstanceProperty(value:int):void { }
		public static function get readOnlyStaticProperty():int { return 0; }		
		public static function get readWriteStaticProperty():int { return 0; }
		public static function set readWriteStaticProperty(value:int):void { }
		public static function set writeOnlyStaticProperty(value:int):void { }
	}
}