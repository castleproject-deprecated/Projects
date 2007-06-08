package castle.flexbridge.kernel
{
	/**
	 * A component lifestyle type specifies how a component is to be
	 * created and disposed.  A few simple lifestyle types are defined
	 * as constants here.
	 */
	public final class Lifestyle
	{
		/**
		 * An IComponentHandler class with constructor signature
		 * function(componentKey:String, service:Class, component:Class):IComponentHandler.
		 */
		private var _handlerClass:Class;
		
		/**
		 * The singleton lifestyle.
		 */
		public static const SINGLETON:Lifestyle = new Lifestyle(SingletonComponentHandler);
		
		/**
		 * The transient lifestyle.
		 */
		public static const TRANSIENT:Lifestyle = new Lifestyle(TransientComponentHandler);
		
		/**
		 * Creates a custom lifestyle with the specified handler factory.
		 * 
		 * @param handlerClass An IComponentHandler class with constructor signature
		 * function(service:Class, component:Class, componentKey:String):IComponentHandler
		 */
		public function Lifestyle(handlerClass:Class)
		{
			_handlerClass = handlerClass;
		}
		
		/**
		 * Creates a handler for the specified component model.
		 * 
		 * @param kernel The kernel.
		 * @param componentModel The component model.
		 */
		public function createHandler(kernel:IKernel, componentModel:ComponentModel):IComponentHandler
		{
			return IComponentHandler(new _handlerClass(kernel, componentModel));
		}
	}
}