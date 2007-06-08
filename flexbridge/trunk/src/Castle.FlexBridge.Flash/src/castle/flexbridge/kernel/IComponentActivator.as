package castle.flexbridge.kernel
{
	/**
	 * A component activator constructs and disposes of components
	 * on demand.
	 */
	public interface IComponentActivator
	{
		/**
		 * Creates a component.
		 * 
		 * @param componentHandler The component's handler.
		 */
		function createComponent(componentHandler:IComponentHandler):Object;

		/**
		 * Disposes of a component.
		 * 
		 * @param componentInstance The component instance to dispose.
		 */
		function disposeComponent(componentInstance:Object):void;
	}
}