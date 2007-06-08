package castle.flexbridge.reflection
{
	import flash.utils.describeType;
	import flash.utils.getDefinitionByName;
	import flash.utils.getQualifiedClassName;
	import flash.utils.Dictionary;
    import castle.flexbridge.common.*;
	
	/**
	 * Reflection utility functions.
	 */
	public final class ReflectionUtils
	{
		private static var _descriptionCache:Dictionary = new Dictionary();
		
		/**
		 * Gets the class of an object.
		 * 
		 * @param obj The object.
		 * @return The object's class.
		 */
		public static function getClass(obj:Object):Class
		{
			return obj.constructor as Class;
		}
		
		/**
		 * Gets a class by its name.
		 * If name is "void" returns Void.
		 * If name is "*" returns Any.
		 * 
		 * @param name The class name.
		 * @return The class.
		 */
		public static function getClassByName(name:String):Class
		{
			if (name == "void")
				return Void;
			if (name == "*")
				return Any;
			
			return getDefinitionByName(name) as Class;
		}
		
		/**
		 * Gets reflection information for a class or interface.
		 * 
		 * @param type The class or interface for which to obtain information.
		 * @return The reflection information.
		 */
		public static function getClassInfo(type:Class):ClassInfo
		{
			var typeElement:XML = robustDescribeType(type);
			return new ClassInfo(typeElement);
		}
		
		/**
		 * Obtains a description of a type.
		 * This method works around a bug in describeType() where it
		 * fails to return correct type information for constructor parameters
		 * unless an instance of the type has actually been constructed.
		 * The result is cached to speed up subsequent accesses.
		 * 
		 * This hack is similar to one developed independently by "dk"
		 * @see http://thefoundry.anywebcam.com/index.php/actionscript/flash-utils-describetype-bug/
		 * 
		 * @param type The class or interface to be described.
		 * @return The description of the type.
		 */
		public static function robustDescribeType(type:Class):XML
		{
			var description:XML = _descriptionCache[type] as XML;
			if (description != null)
				return description;
				
			// Get an initial description of the type.
			description = describeType(type);
			
			// If the type's constructor has any parameters, Flash Player 9 will
			// consistently return them as "*" until an instance of the type is
			// created.  So check whether there is at least one constructor parameter
			// and all of them were returned as '*'.
			var parameterElements:XMLList = description.factory.constructor.parameter;
			var parameterCount:int = parameterElements.length();
			if (parameterCount != parameterElements.(@type != '*').length())
			{
				// As a workaround, instantiate the type with "unknown" parameter values.
				// It does not matter if an exception is thrown.
				try
				{
					createInstance(type, new Array(parameterCount));
				}
				catch (e:Error)
				{
					// Ignore the exception.  The workaround will still work
					// even if instantiation fails as long as the constructor
					// actually gets invoked.
				}
				
				// Get a new description.  It should be good now.
				description = describeType(type);
			}
			
			_descriptionCache[type] = description;
			return description;
		}
		
		/**
		 * Creates an instance of a type with the specified arguments.
		 * This method is used to workaround the fact that Function.apply cannot
		 * be used with object constructors.
		 * 
		 * @param type The type of object to create.
		 * @param args The constructor arguments.
		 * @return The created argument.
		 */
		public static function createInstance(type:Class, args:Array):Object
		{
			switch (args.length)
			{
				case 0:
					return new type();
				
				case 1:
					return new type(args[0]);
					
				case 2:
					return new type(args[0], args[1]);
				
				case 3:
					return new type(args[0], args[1], args[2]);

				case 4:
					return new type(args[0], args[1], args[2], args[3]);

				case 5:
					return new type(args[0], args[1], args[2], args[3], args[4]);

				case 6:
					return new type(args[0], args[1], args[2], args[3], args[4], args[5]);

				case 7:
					return new type(args[0], args[1], args[2], args[3], args[4], args[5], args[6]);

				case 8:
					return new type(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]);
				
				default:
					throw new NotSupportedError("The constructor for type '" +
						getQualifiedClassName(type) + "' has too many parameters to be handled by createInstance().");
			}
		}
		
		/**
		 * Applies the converter function to each element of the input
		 * XMLList and stores its results in the same order in an output
		 * array of the same length.
		 * 
		 * @param input The input list.
		 * @param converter The converter function with signature function(elem:Object):Object.
		 * @return The output array.
		 */
		internal static function convertXMLListToArray(input:XMLList, converter:Function):Array
		{
			var length:int = input.length();
			var output:Array = new Array(length);
			
			for (var i:int = 0; i < length; i++)
				output[i] = converter(input[i]);
				
			return output;
		}
	}
}