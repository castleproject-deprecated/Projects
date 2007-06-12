package castle.flexbridge.controls
{
	import flash.events.Event;

	/**
	 * Describes an event raised by the ActiveTextArea control.
	 */
	public class ActiveTextEvent extends Event
	{
		/**
		 * The type of event dispatched when a link is clicked if its Url
		 * begins with "event:".
		 */
		public static const LINK_CLICK:String = "linkClick";
		
		/**
		 * The type of event dispatched when the mouse enters the area of a link.
		 */
		public static const LINK_ROLL_OVER:String = "linkRollOver";
		
		/**
		 * The type of event dispatched when the mouse leaves the area of a link.
		 */
		public static const LINK_ROLL_OUT:String = "linkRollOut";
		
		/**
		 * Initializes an event.
		 */
		public function ActiveTextEvent(type:String, bubbles:Boolean = false, cancelable:Boolean = false,
			linkUrl:String = null)
		{
			super(type, bubbles, cancelable);
			
			this.linkUrl = linkUrl;
		}
		
		/**
		 * The Url from the href attribute of the link related to this event,
		 * or null if none.
		 */
		public var linkUrl:String;
		
		public override function clone():Event
		{
			return new ActiveTextEvent(type, bubbles, cancelable, linkUrl);
		}
	}
}