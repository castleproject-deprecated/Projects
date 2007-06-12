package castle.flexbridge.collections
{
	import mx.events.CollectionEventKind;
	
	/**
	 * An enumeration of <code>CollectionDelta</code> kinds.
	 */
	public final class CollectionDeltaKind
	{
		public static const ADD:String = CollectionEventKind.ADD;
		public static const MOVE:String = CollectionEventKind.MOVE;
		public static const REFRESH:String = CollectionEventKind.REFRESH;
		public static const REMOVE:String = CollectionEventKind.REMOVE;
		public static const REPLACE:String = CollectionEventKind.REPLACE;
		public static const RESET:String = CollectionEventKind.RESET;
		public static const UPDATE:String = CollectionEventKind.UPDATE;		
	}
}