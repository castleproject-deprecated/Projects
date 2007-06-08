package castle.flexbridge.reflection
{
	/**
	 * Provides reflection information about a method or
	 * constructor parameter.
	 */
	public class ParameterInfo
	{
		private var _parameterElement:XML;
		
		/**
		 * Creates a reflection wrapper.
		 * @param parameterElement The &lt;parameter&gt; element of the
		 * XML returned by describeType().
		 */
		public function ParameterInfo(parameterElement:XML)
		{
			_parameterElement = parameterElement;
		}
		
		/**
		 * Gets the zero-based index of the parameter.
		 */
		public function get index():int
		{
			return int(_parameterElement.@index) - 1;
		}
		
		/**
		 * Gets the type of the parameter.
		 */
		public function get type():Class
		{
			return ReflectionUtils.getClassByName(_parameterElement.@type);
		}
		
		/**
		 * Returns true if the parameter is optional.
		 */
		public function get isOptional():Boolean
		{
			return _parameterElement.@optional == "true";
		}
	}
}