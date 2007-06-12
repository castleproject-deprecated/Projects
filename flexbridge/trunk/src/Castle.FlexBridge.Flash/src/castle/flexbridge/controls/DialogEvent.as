package castle.flexbridge.controls
{
	import flash.events.Event;
	
	/**
	 * An event dispatched a <code>Dialog</code>.
	 */
	public class DialogEvent extends Event
	{
		/**
		 * The event dispatched when the dialog's <code>close()</code>
		 * method is called.
		 */
		public static const DIALOG_CLOSE:String = "dialogClose";
		
		/**
		 * The event dispatched when the dialog's <code>open()</code>
		 * method is called.
		 */
		public static const DIALOG_OPEN:String = "dialogOpen";
		
		/**
		 * Creates a dialog event.
		 * 
		 * @param result The dialog result when the dialog is closed or null if none.
		 */
		public function DialogEvent(type:String, bubbles:Boolean = false,
			cancelable:Boolean = false, result:String = null)
		{
			super(type, bubbles, cancelable);
			
			this.result = result;
		}
		
		/**
		 * The dialog result when the dialog is closed or null if none.
		 */
		public var result:String;
		
		public override function clone():Event
		{
			return new DialogEvent(type, bubbles, cancelable, result);
		}
	}
}