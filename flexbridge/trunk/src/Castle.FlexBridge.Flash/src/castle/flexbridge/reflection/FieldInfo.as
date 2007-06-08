package castle.flexbridge.reflection
{
	/**
	 * Provides reflection information about a field.
	 */
	public class FieldInfo extends MemberInfo
	{
		/**
		 * Creates a reflection wrapper.
		 * @param variableElement The &lt;variable&gt; element of the
		 * XML returned by describeType().
		 * @param isStatic True if the field is static.
		 */
		public function FieldInfo(variableElement:XML, isStatic:Boolean)
		{
			super(variableElement, isStatic);
		}
		
		/**
		 * Gets the name of the field.
		 */
		public function get name():String
		{
			return xmlElement.@name;
		}
		
		/**
		 * Gets the type of the field.
		 */
		public function get type():Class
		{
			return ReflectionUtils.getClassByName(xmlElement.@type);
		}
	}
}