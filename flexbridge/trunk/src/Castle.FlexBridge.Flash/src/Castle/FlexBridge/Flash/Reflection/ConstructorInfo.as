package Castle.FlexBridge.Flash.Reflection
{
	/**
	 * Provides reflection information about a constructor.
	 */
	public class ConstructorInfo
	{
		private var _constructorElement:XML;
		
		/**
		 * Creates a reflection wrapper.
		 * @param constructorElement The &lt;constructor&gt; element of the
		 * XML returned by describeType().
		 */
		public function ConstructorInfo(constructorElement:XML)
		{
			_constructorElement = constructorElement;
		}
		
		/**
		 * Gets the array of constructor parameters.
		 */
		[ArrayElementType("Castle.FlexBridge.Flash.Reflection.ParameterInfo")]
		public function get parameters():Array
		{
			return ReflectionUtils.convertXMLListToArray(
				_constructorElement.elements("parameter"),
				function(parameterElement:XML):ParameterInfo { return new ParameterInfo(parameterElement) });
		}
	}
}