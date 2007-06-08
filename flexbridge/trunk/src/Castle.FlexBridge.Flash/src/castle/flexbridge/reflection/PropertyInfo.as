package castle.flexbridge.reflection
{
	/**
	 * Provides reflection information about a property.
	 */
	public class PropertyInfo extends MemberInfo
	{
		/**
		 * Creates a reflection wrapper.
		 * @param accessorElement The &lt;accessor&gt; element of the
		 * XML returned by describeType().
		 * @param isStatic True if the property is static.
		 */
		public function PropertyInfo(accessorElement:XML, isStatic:Boolean)
		{
			super(accessorElement, isStatic);
		}
		
		/**
		 * Gets the name of the property.
		 */
		public function get name():String
		{
			return xmlElement.@name;
		}
		
		/**
		 * Gets the type of the property.
		 */
		public function get type():Class
		{
			return ReflectionUtils.getClassByName(xmlElement.@type);
		}
		
		/**
		 * Gets the declaring type of the property.
		 */
		public function get declaringType():Class
		{
			return ReflectionUtils.getClassByName(xmlElement.@declaredBy);
		}
		
		/**
		 * Returns true if the property is readable.
		 */
		public function get isReadable():Boolean
		{
			return xmlElement.@access != "writeonly";
		}
		
		/**
		 * Returns true if the property is writeable.
		 */
		public function get isWriteable():Boolean
		{
			return xmlElement.@access != "readonly";
		}
	}
}