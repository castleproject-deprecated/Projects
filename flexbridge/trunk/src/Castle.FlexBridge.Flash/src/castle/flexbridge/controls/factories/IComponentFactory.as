package castle.flexbridge.controls.factories
{
	import flash.display.DisplayObject;
	
	/**
	 * A component factory is used to construct components based on a collection
	 * of attribute key/value pairs that may be embedded in some other content.
	 * This is used when components are embedded in places where MXML cannot be
	 * used such as in the enhanced HTML of ActiveText controls.
	 * 
	 * @see castle.flexbridge.controls.ActiveText
	 * @see castle.flexbridge.controls.ActiveTextArea
	 * @see castle.flexbridge.controls.ComponentFactoryRegistry 
	 */
	public interface IComponentFactory
	{
		/**
		 * Creates a new instance of the component.
		 * 
		 * @param attributes The name/value String pairs that describe the object
		 *   to be constructed and its properties.
		 * @return The new component instance.
		 */
		function newInstance(attributes:Object):DisplayObject;
	}
}