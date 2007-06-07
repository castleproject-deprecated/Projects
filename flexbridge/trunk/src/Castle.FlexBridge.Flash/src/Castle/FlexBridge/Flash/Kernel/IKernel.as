package Castle.FlexBridge.Flash.Kernel
{
	/**
	 * A miniature inversion of control container with constructor
	 * dependency injection for use with Adobe Flex.
	 * Loosely modeled after the Castle MicroKernel but significantly stripped down.
	 * 
	 * Supports registering components with Singleton and Transient lifestyle,
	 * managing explicitly creates component instances, and performing constructor
	 * dependency injection during component resolution.
	 */
	public interface IKernel
	{
		/**
		 * Gets or sets the component model builder used by the kernel.
		 */
		function get componentModelBuilder():IComponentModelBuilder;
		function set componentModelBuilder(value:IComponentModelBuilder):void;
		
		/**
		 * Gets an instance of a component that implements the specified service.
		 * Throws an error if there is not exactly one component registered for the service.
		 *
 		 * @param serviceType The type of service to resolve.
		 * @return The resolved component, never null.
		 */
		function resolve(serviceType:Class):Object;
		
		/**
		 * Gets an instance of a component with the specified key.
		 * Throws an error if there is no component registered for the key.
		 * 
		 * @param componentKey The key of the component to resolve.
		 * @return The resolved component, never null.
		 */
		function resolveByKey(componentKey:String):Object;
		
		/**
		 * Gets an instance of each component that implements the specified service.
		 * 
		 * @param serviceType The type of service to resolve.
		 * @return The array of resolved services, possibly empty.
		 */
		function resolveAll(serviceType:Class):Array /*of the service type*/;
		
		/**
		 * Registers a component class as a singleton provider of the specified service.
		 * 
		 * @param componentKey The component key.
		 * @param serviceType The service class.
		 * @param componentType The component class (which must implement the service).
		 * @param lifestyle The component lifestyle, defaults to the value provided by the
		 *   component model builder (usually Singleton).
		 */
		function registerComponent(componentKey:String, serviceType:Class, componentType:Class, lifestyle:Lifestyle = null):void;

		/**
		 * Registers a component instance as a provider of the specified service.
		 * 
		 * @param componentKey The component key.
		 * @param serviceType The service class.
		 * @param componentInstance The component instance (which must be of a class that implements the service).
		 */
		function registerComponentInstance(componentKey:String, serviceType:Class, componentInstance:Object):void;

		/**
		 * Registers a component by its component model.
		 * 
		 * @param componentModel The component model.
		 */
		function registerComponentModel(componentModel:ComponentModel):void;
		
		/**
		 * Registers a component handler.
		 * 
		 * @param componentHandler The component handler.
		 */
		function registerComponentHandler(componentHandler:IComponentHandler):void;
		
		/**
		 * Gets an array of component handlers for the specified service.
		 * 
		 * @param serviceType The type of service.
		 * @return An array of component handlers, possibly empty.
		 */
		function getComponentHandlers(serviceType:Class):Array /* of IComponentHandler*/;
		
		/**
		 * Gets the component handler for the specified service.
		 * Throws an error if there is not exactly one component registered for the service.
		 * 
		 * @param serviceType The type of service.
		 * @return The component handler for the component that implements the service, never null.
		 */
		function getComponentHandler(service:Class):IComponentHandler;

		/**
		 * Gets the component handler with the specified key.
		 * Throws an error if there is no component registered for the key.
		 * 
		 * @param componentKey The component key.
		 * @return The component handler for the component with the specified key, never null.
		 */
		function getComponentHandlerByKey(componentKey:String):IComponentHandler;
	}
}