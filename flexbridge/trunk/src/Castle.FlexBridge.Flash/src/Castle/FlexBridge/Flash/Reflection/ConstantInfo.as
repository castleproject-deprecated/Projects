package Castle.FlexBridge.Flash.Reflection
{
	/**
	 * Provides reflection information about a constant.
	 */
	public class ConstantInfo extends MemberInfo
	{
		/**
		 * Creates a reflection wrapper.
		 * @param constantElement The &lt;constant&gt; element of the
		 * XML returned by describeType().
		 * @param isStatus True if the constant is static.
		 */
		public function ConstantInfo(constantElement:XML, isStatic:Boolean)
		{
			super(constantElement, isStatic);
		}
		
		/**
		 * Gets the name of the constant.
		 */
		public function get name():String
		{
			return xmlElement.@name;
		}
		
		/**
		 * Gets the type of the constant.
		 */
		public function get type():Class
		{
			return ReflectionUtils.getClassByName(xmlElement.@type);
		}
	}
}