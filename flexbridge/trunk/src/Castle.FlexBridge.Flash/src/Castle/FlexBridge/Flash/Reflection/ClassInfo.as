package Castle.FlexBridge.Flash.Reflection
{
	/**
	 * Provides reflection information about a class or interface.
	 */
	public class ClassInfo
	{
		private var _typeElement:XML;
		private var _factoryElement:XML;
		
		/**
		 * Creates a reflection wrapper.
		 * @param _factoryElement The &lt;type&gt; element of the
		 * XML returned by describeType().
		 */
		public function ClassInfo(typeElement:XML)
		{
			_typeElement = typeElement;
			_factoryElement = typeElement.elements("factory")[0];
		}
		
		/**
		 * Gets the name of the class.
		 */
		public function get name():String
		{
			return _factoryElement.@type;
		}
		
		/**
		 * Gets the class type.
		 */
		public function get type():Class
		{
			return ReflectionUtils.getClassByName(_factoryElement.@type);
		}
		
		/**
		 * Returns true if the type is an interface.
		 */
		public function get isInterface():Boolean
		{
			return _factoryElement.elements("extendsClass").(@type == "Object").length() == 0
		}
		
		/**
		 * Gets the constructor information, or null if the type is an interface.
		 */
		public function get constructor():ConstructorInfo
		{
			var constructorElements:XMLList = _factoryElement.elements("constructor");
			if (constructorElements.length() != 0)
				return new ConstructorInfo(constructorElements[0]);
				
			if (! isInterface)
				return new ConstructorInfo(<constructor/>);
				
			return null;
		}

		/**
		 * Gets the array of superclasses.
		 * May be empty if the type is an interface.
		 */
		[ArrayElementType("Class")]
		public function get superclasses():Array
		{
			return ReflectionUtils.convertXMLListToArray(
				_factoryElement.elements("extendsClass").attribute("type"),
				ReflectionUtils.getClassByName);
		}

		/**
		 * Gets the array of implemented interfaces.
		 * May be empty if the type is an interface.
		 */
		[ArrayElementType("Class")]
		public function get interfaces():Array
		{
			return ReflectionUtils.convertXMLListToArray(
				_factoryElement.elements("implementsInterface").attribute("type"),
				ReflectionUtils.getClassByName);
		}

		/**
		 * Gets the array of fields declared by the type.
		 */
		[ArrayElementType("Castle.FlexBridge.Flash.Reflection.ConstantInfo")]
		public function get constants():Array
		{
			var instanceConstants:Array = ReflectionUtils.convertXMLListToArray(
				_factoryElement.elements("constant"),
				function(constantElement:XML):ConstantInfo { return new ConstantInfo(constantElement, false) });
			var staticConstants:Array = ReflectionUtils.convertXMLListToArray(
				_typeElement.elements("constant"),
				function(constantElement:XML):ConstantInfo { return new ConstantInfo(constantElement, true) });
			return instanceConstants.concat(staticConstants);
		}

		/**
		 * Gets the array of fields declared by the type.
		 */
		[ArrayElementType("Castle.FlexBridge.Flash.Reflection.FieldInfo")]
		public function get fields():Array
		{
			var instanceFields:Array = ReflectionUtils.convertXMLListToArray(
				_factoryElement.elements("variable"),
				function(variableElement:XML):FieldInfo { return new FieldInfo(variableElement, false) });
			var staticFields:Array = ReflectionUtils.convertXMLListToArray(
				_typeElement.elements("variable"),
				function(variableElement:XML):FieldInfo { return new FieldInfo(variableElement, true) });
			return instanceFields.concat(staticFields);
		}
		
		/**
		 * Gets the array of properties declared by the type.
		 */
		[ArrayElementType("Castle.FlexBridge.Flash.Reflection.PropertyInfo")]
		public function get properties():Array
		{
			var instanceProperties:Array = ReflectionUtils.convertXMLListToArray(
				_factoryElement.elements("accessor"),
				function(accessorElement:XML):PropertyInfo { return new PropertyInfo(accessorElement, false) });
			var staticProperties:Array = ReflectionUtils.convertXMLListToArray(
				_typeElement.elements("accessor").(@declaredBy != "Class"), // filter out the "prototype" property in the Class
				function(accessorElement:XML):PropertyInfo { return new PropertyInfo(accessorElement, true) });
			return instanceProperties.concat(staticProperties);
		}

		/**
		 * Gets the array of methods declared by the type.
		 */
		[ArrayElementType("Castle.FlexBridge.Flash.Reflection.MethodInfo")]
		public function get methods():Array
		{
			var instanceMethods:Array = ReflectionUtils.convertXMLListToArray(
				_factoryElement.elements("method"),
				function(methodElement:XML):MethodInfo { return new MethodInfo(methodElement, false) });
			var staticMethods:Array = ReflectionUtils.convertXMLListToArray(
				_typeElement.elements("method"),
				function(methodElement:XML):MethodInfo { return new MethodInfo(methodElement, true) });
			return instanceMethods.concat(staticMethods);
		}
	}
}