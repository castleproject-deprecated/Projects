package castle.flexbridge.controls.factories
{
	import flash.errors.IllegalOperationError;
	import flash.display.DisplayObject;
	
	/**
	 * A simple registry for component factories.
	 * The registry interprets the "type" attribute of the component to be
	 * created as a key to select the actual component factory to use.
	 */
	public class TypedComponentFactory implements IComponentFactory
	{
		/**
		 * Gets the singleton global instance of the registry.
		 */
		public static const instance:TypedComponentFactory = new TypedComponentFactory();
		
		private var _factories:Object = new Object();
		
		/**
		 * Registers a factory for manufacturing an embedded component when the "type"
		 * attribute of the embedded element equals the specified value.
		 * 
		 * @param type The value to appear in the "type" attribute.
		 * @param factory The embedded component factory.
		 */
		public function registerFactory(type:String, factory:IComponentFactory):void
		{
			_factories[type] = factory;
		}
		
		/**
		 * Creates a new instance of the component.
		 * Delegates to the registered factory associated with the value of the
		 * "type" attribute in the specified attributes object.
		 * 
		 * @param attributes The name/value String pairs that describe the object
		 *   to be constructed and its properties.
		 * @return The new component instance.
		 */
		public function newInstance(attributes:Object):DisplayObject
		{
			var type:String = attributes["type"];
			var factory:IComponentFactory = _factories[type] as IComponentFactory;
			if (factory == null)
				throw new IllegalOperationError("There is no registered component factory for type '" + type + "'.");
				
			return factory.newInstance(attributes);
		}
		
		// Register known factories.
		instance.registerFactory("icon", IconFactory.instance);
	}
}