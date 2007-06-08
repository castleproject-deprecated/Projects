package castle.flexbridge.kernel
{
	/**
	 * Describes a component managed by the IoC container.
	 */
	public class ComponentModel
	{
		private var _componentKey:String;
		private var _serviceType:Class;
		private var _constructorDependencies:Array;
		private var _componentActivator:IComponentActivator;
		private var _lifestyle:Lifestyle;
		
		/**
		 * Creates a component model.
		 */
		public function ComponentModel(componentKey:String, serviceType:Class)
		{
			_componentKey = componentKey;
			_serviceType = serviceType;
			_constructorDependencies = [ ];
			_componentActivator = null;
			_lifestyle = Lifestyle.SINGLETON;
		}
		
		/**
		 * Gets or sets the unique key assigned to the component.
		 */
		public function get componentKey():String
		{
			return _componentKey;
		}
		
		public function set componentKey(value:String):void
		{
			_componentKey = value;
		}
		
		/**
		 * Gets or sets the service type associated with the component.
		 */
		public function get serviceType():Class
		{
			return _serviceType;
		}
		
		public function set serviceType(value:Class):void
		{
			_serviceType = value;
		}
		
		/**
		 * Gets or sets the array of constructor dependencies in the
		 * same order they appear in the constructor's declaration.
		 */
		[ArrayElementType("castle.flexbridge.kernel.DependencyModel")]
		public function get constructorDependencies():Array
		{
			return _constructorDependencies;
		}
		
		public function set constructorDependencies(value:Array):void
		{
			_constructorDependencies = value;
		}
		
		/**
		 * Gets or sets the component activator to use for activating the component.
		 */
		public function get componentActivator():IComponentActivator
		{
			return _componentActivator;
		}
		
		public function set componentActivator(value:IComponentActivator):void
		{
			_componentActivator = value;
		}
		
		/**
		 * Gets or sets the lifestyle of the component.
		 */
		public function get lifestyle():Lifestyle
		{
			return _lifestyle;
		}
		
		public function set lifestyle(value:Lifestyle):void
		{
			_lifestyle = value;
		}
	}
}