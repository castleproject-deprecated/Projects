package castle.flexbridge.collections
{
	import flash.events.Event;

	/**
	 * A collection delta event conveys one or more deltas resulting
	 * from a change to a collection.
	 */
	public class CollectionDeltaEvent extends Event
	{
		private var _deltas:Array;
		
		/**
		 * The event type that signals a collection change has occurred.
		 */
		public static const COLLECTION_DELTA:String = "collectionDelta";
		
		/**
		 * Creates a collection delta event.
		 */
		public function CollectionDeltaEvent(type:String, bubbles:Boolean=false, cancelable:Boolean=false, deltas:Array = null)
		{
			super(type, bubbles, cancelable);
			
			this.deltas = deltas;
		}

		/**
		 * Gets or sets the array of CollectionDelta objects that describe the change.
		 * The order of the deltas is significant.  As they describe a sequence
		 * of changes later deltas may be dependent upon earlier ones.
		 */
		public function get deltas():Array
		{
			return _deltas;
		}
		
		public function set deltas(value:Array):void
		{
			_deltas = value;
		}
				
		override public function clone():Event
		{
			return new CollectionDeltaEvent(type, bubbles, cancelable, deltas);
		}
	}
}