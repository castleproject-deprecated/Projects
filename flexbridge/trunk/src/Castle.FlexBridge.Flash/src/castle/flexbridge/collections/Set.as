package castle.flexbridge.collections
{
	import flash.utils.Dictionary;
	import flash.events.EventDispatcher;
	import flash.events.Event;
	
	/**
	 * Indicates that the collection has changed and includes one or more
	 * deltas describing the nature of the change.
	 */
	[Event(name="collectionDelta", type="castle.flexbridge.collections.CollectionDeltaEvent")]
	
	/**
	 * A Set is a wrapper around a Dictionary that provides
	 * unordered set-like operations such as determining inclusion in the
	 * set.
	 * 
	 * TODO: Provide more operations as needed.
	 */
	public class Set extends EventDispatcher
	{
		private var _length:int;
		private var _generation:int;
		private var _dictionary:Dictionary;
		
		/**
		 * Creates an empty set.
		 */
		public function Set()
		{
			_length = 0;
			_generation = 0;
			_dictionary = new Dictionary();
		}
		
		/**
		 * Gets the current generation number.
		 * The generation number starts at 0 and increases with each
		 * change to the Set.  It may be used for databinding.
		 */
		[Bindable(event="collectionDelta")]
		public function get generation():int
		{
			return _generation;
		}
		
		/**
		 * Gets the number of values in the set.
		 */
		[Bindable(event="collectionDelta")]
		public function get length():int
		{
			return _length;
		}
		
		/**
		 * Clears the set.
		 */
		public function clear():void
		{
			if (_length != 0)
			{
				_dictionary = new Dictionary();
				_length = 0;
				
				notifyCollectionDelta(new CollectionDelta(CollectionDeltaKind.RESET));
			}
		}
		
		/**
		 * Determines if the set contains the specified value.
		 */
		public function contains(value:Object):Boolean
		{
			return value in _dictionary;
		}
		
		/**
		 * Adds a value to the set.
		 */
		public function add(value:Object):void
		{
			if (! (value in _dictionary))
			{
				_dictionary[value] = null;				
				_length += 1;
				
				notifyCollectionDelta(new CollectionDelta(CollectionDeltaKind.ADD,
					-1, [ value ], 0, null, null));
			}
		}

		/**
		 * Adds all of the values in the array to the set.
		 */
		public function addAll(values:Array):void
		{
			var changed:Boolean = false;
			
			for each (var value:Object in values)
			{
				if (! (value in _dictionary))
				{
					_dictionary[value] = null;
					_length += 1;
				
					changed = true;
				}
			}
			
			if (changed)
				notifyCollectionDelta(new CollectionDelta(CollectionDeltaKind.ADD,
					-1, values, 0, null, null));
		}
				
		/**
		 * Removes a value from the set.
		 */
		public function remove(value:Object):void
		{
			if (value in _dictionary)
			{
				delete _dictionary[value];
				_length -= 1;

				notifyCollectionDelta(new CollectionDelta(CollectionDeltaKind.REMOVE,
					-1, [ value ], 0, null, null));
			}
		}
		
		/**
		 * Removes all of the values in the array from the set.
		 */
		public function removeAll(values:Array):void
		{
			var changed:Boolean = false;
			
			for each (var value:Object in values)
			{
				if (value in _dictionary)
				{
					delete _dictionary[value];
					_length -= 1;
					
					changed = true;
				}
			}
			
			if (changed)
				notifyCollectionDelta(new CollectionDelta(CollectionDeltaKind.REMOVE,
					-1, values, 0, null, null));
		}
		
		/**
		 * Sets whether a value is a member of the set or not.
		 * 
		 * @param value The value.
		 * @param isMember If true the value is added to the set
		 *   otherwise it is removed.
		 */
		public function setMembership(value:Object, isMember:Boolean):void
		{
			if (isMember)
				add(value);
			else
				remove(value);
		}
		
		/**
		 * Converts all of the values of the set to an array.
		 */
		public function toArray():Array
		{
			var result:Array = [ ];
			
			for (var value:Object in _dictionary)
				result.push(value);
				
			return result;
		}
		
		private function notifyCollectionDelta(delta:CollectionDelta):void
		{
			_generation += 1;
			
			dispatchEvent(new CollectionDeltaEvent(CollectionDeltaEvent.COLLECTION_DELTA, false, false, [ delta ]));
		}
	}
}