package castle.flexbridge.controls.factories
{
	import flash.display.DisplayObject;
	import castle.flexbridge.controls.Icon;
	
	/**
	 * A factory that generated embedded icons.
	 * 
	 * Used in ActiveText with enhanced htmlText as follows:
	 *   <embed type="icon" attribs... />
	 * 
	 * The following attributes are supported:
	 * 
	 *   icon: (optional)
	 *       The name of the class for an embedded image asset.
	 *       If omitted no icon is rendered.
	 * 
	 *   width: (optional)
	 *       The explicit width of the icon in pixels.
	 * 
	 *   height: (optional)
	 *       The explicit height of the icon in pixels.
	 * 
	 *   toolTip: (optional)
	 *       The tool tip to apply to the icon.
	 */
	public class IconFactory implements IComponentFactory
	{
		/**
		 * Gets the singleton global instance of the factory.
		 */
		public static const instance:IconFactory = new IconFactory();

		public function newInstance(attributes:Object):DisplayObject
		{
			var icon:Icon = new Icon();
			
			if ("width" in attributes)
				icon.explicitWidth = parseInt(attributes["width"], 10);
			
			if ("height" in attributes)
				icon.explicitHeight = parseInt(attributes["height"], 10);
				
			if ("icon" in attributes)
				icon.icon = attributes["icon"];
				
			if ("toolTip" in attributes)
				icon.toolTip = attributes["toolTip"];
				
			return icon;
		}
	}
}