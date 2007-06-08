package castle.flexbridge.common
{
	import flash.events.Event;

	/**
	 * Event type used by AsyncTask.
	 */
	public class AsyncTaskEvent extends Event
	{
		/**
		 * The event raised when the task completes successfully and the
		 * AsyncTask.result property becomes valid.
		 */
		public static const RESULT:String = "result";
		
		/**
		 * The event raised when the task completes with an error and the
		 * AsyncTask.error property becomes valid.
		 */
		public static const ERROR:String = "error";
		
		/**
		 * The event raised when the task status changes.
		 */
		public static const STATUS_CHANGE:String = "statusChange";
		
		/**
		 * Creates an AsyncTask event.
		 */
		public function AsyncTaskEvent(type:String, bubbles:Boolean=false, cancelable:Boolean=false)
		{
			super(type, bubbles, cancelable);
		}
		
		public override function clone():Event
		{
			return new AsyncTaskEvent(type, bubbles, cancelable);
		}
	}
}