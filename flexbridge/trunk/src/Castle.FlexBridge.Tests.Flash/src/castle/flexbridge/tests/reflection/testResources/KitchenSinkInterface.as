package castle.flexbridge.tests.reflection.testResources
{
	public interface KitchenSinkInterface extends EmptyInterface
	{
		function simpleInstanceMethod():void;
		function complexInstanceMethod(a:int, b:*, c:String = null):Object;
		
		function get readOnlyInstanceProperty():int;		
		function get readWriteInstanceProperty():int;
		function set readWriteInstanceProperty(value:int):void;		
		function set writeOnlyInstanceProperty(value:int):void;
	}
}