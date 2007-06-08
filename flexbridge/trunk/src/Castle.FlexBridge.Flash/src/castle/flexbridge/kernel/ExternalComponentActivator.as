package castle.flexbridge.kernel
{
	/**
	 * A component activator that manages an instance of a component that
	 * was created externally rather than being managed by the container.
	 */
	public class ExternalComponentActivator implements IComponentActivator
	{
		private var _componentInstance:Object;
		
		/**
		 * Creates an external component activator with the specified component instance.
		 * 
		 * @param componentInstance The external component instance.
		 */
		public function ExternalComponentActivator(componentInstance:Object)
		{
			_componentInstance = componentInstance;
		}
		
		public function createComponent(componentHandler:IComponentHandler):Object
		{
			return _componentInstance;
		}
		
		public function disposeComponent(componentInstance:Object):void
		{
			// External instances are never disposed.
		}
	}
}