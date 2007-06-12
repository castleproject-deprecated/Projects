package castle.flexbridge.controls.activeTextClasses
{
	import flash.display.DisplayObject;
	
	/**
	 * Contains information to describe an embedded component within
	 * an ActiveTextField.
	 */
	public class EmbeddedComponentInfo
	{
		/**
		 * Gets the external id of the embedded object.
		 */
		public var id:String;
		
		/**
		 * Gets the attributes of the component as key/value pairs as
		 * they were specified in the &lt;EMBED&gt; tag.
		 */
		public var attributes:Object;
	
		/**
		 * Gets the index of the character position within the text that contains
		 * the component, or -1 if the component has not been created yet.
		 */
		public var charIndex:int = -1;
	
		/**
		 * The embedded display object, or null if not created yet.
		 */
		public var component:DisplayObject = null;
	}
}