package Castle.FlexBridge.Flash.Reflection
{
	/**
	 * Provides reflection information about a method.
	 */
	public class MethodInfo extends MemberInfo
	{
		/**
		 * Creates a reflection wrapper.
		 * @param methodElement The &lt;method&gt; element of the
		 * XML returned by describeType().
		 * @param isStatic True if the method is static.
		 */
		public function MethodInfo(methodElement:XML, isStatic:Boolean)
		{
			super(methodElement, isStatic);
		}
		
		/**
		 * Gets the name of the method.
		 */
		public function get name():String
		{
			return xmlElement.@name;
		}
		
		/**
		 * Gets the declaring type of the method.
		 */
		public function get declaringType():Class
		{
			return ReflectionUtils.getClassByName(xmlElement.@declaredBy);
		}
		
		/**
		 * Gets the return type of the method.
		 */
		public function get returnType():Class
		{
			return ReflectionUtils.getClassByName(xmlElement.@returnType);
		}

		/**
		 * Gets the array of method parameters.
		 */
		[ArrayElementType("Castle.FlexBridge.Flash.Reflection.ParameterInfo")]
		public function get parameters():Array
		{
			return ReflectionUtils.convertXMLListToArray(
				xmlElement.elements("parameter"),
				function(parameterElement:XML):ParameterInfo { return new ParameterInfo(parameterElement) });
		}
	}
}