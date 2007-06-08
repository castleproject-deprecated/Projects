package castle.flexbridge.reflection
{
	/**
	 * Abstract base class for class member reflection types.
	 */
	public class MemberInfo
	{
		private var _xmlElement:XML;
		private var _isStatic:Boolean;
		
		/**
		 * Creates a reflection wrapper.
		 * @param xmlElement The XML element of the returned by describeType().
		 * @param isStatic True if the member is static.
		 */
		public function MemberInfo(xmlElement:XML, isStatic:Boolean)
		{
			_xmlElement = xmlElement;
			_isStatic = isStatic;
		}
		
		/**
		 * Returns true if the member is static.
		 */
		public function get isStatic():Boolean
		{
			return _isStatic;
		}
		
		/**
		 * Gets the XML element returned by describeType().
		 */
		protected function get xmlElement():XML
		{
			return _xmlElement;
		}
	}
}