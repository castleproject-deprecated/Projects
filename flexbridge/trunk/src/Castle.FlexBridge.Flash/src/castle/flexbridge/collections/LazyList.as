package castle.flexbridge.collections
{
	import mx.collections.IList;
	import flash.events.EventDispatcher;
	import flash.errors.IllegalOperationError;
	import mx.collections.errors.ItemPendingError;
	import mx.rpc.IResponder;
	import mx.events.CollectionEvent;
	import mx.events.CollectionEventKind;
	import flash.utils.Dictionary;
	import castle.flexbridge.common.AsyncTask;
	import castle.flexbridge.common.ResponderUtils;
	import castle.flexbridge.common.NotSupportedError;

	[Event(name="collectionChange", type="flash.events.Event")]
	
	/**
	 * An IList implementation that can retrieve missing items asynchronously
	 * from some data source and cache them for later.
	 */
	public class LazyList extends EventDispatcher implements IList
	{
		private var _getItemsFunction:Function;
		private var _cache:Dictionary;
		private var _length:int;
		
		/**
		 * Creates a lazy list with the specified data provider and the
		 * specified initial length.
		 * 
		 * @param getItemsFunction The function to call to populate the list with
		 *    items when they are not available.
		 *    Must have the signature function(lazyList:LazyList, index:int, prefetch:int):AsyncTask.
		 *    The index indicates the index of the item to retrieve at a minimum.
		 *    If positive, the prefetch specifies how many items with greater indices
		 *        to return if available.
		 *    If negative, the prefetch specifies how many items with lesser indices
		 *        to return if available.
		 *    The result of the asynchronous task is ignored. 
		 *    The function should use the populateItems method and the length
		 *    property of the list to do the work.
		 * 
		 * @param length The initial length, or 0 if not specified.
		 */
		public function LazyList(getItemsFunction:Function, length:int = 0)
		{
			if (getItemsFunction == null)
				throw new ArgumentError("getItemsFunction must not be null.");
				
			_getItemsFunction = getItemsFunction;
			_cache = new Dictionary();
			_length = length;
		}
		
		/**
		 * Gets the known length of the list.
		 */
		[Bindable(event="collectionChange")]
		public function get length():int
		{
			return _length;
		}
		
		/**
		 * Sets the known length of the list.
		 * Note: Not accessible through IList interface.
		 */
		public function set length(length:int):void
		{
			if (length < 0)
				throw new ArgumentError("Length must not be less than zero.");
			
			if (_length != length)
			{
				_length = length;
				
				dispatchEvent(new CollectionEvent(CollectionEvent.COLLECTION_CHANGE,
					false, false, CollectionEventKind.REFRESH));
			}
		}
		
		/**
		 * Gets the item at the specified index.
		 */
		public function getItemAt(index:int, prefetch:int=0):Object
		{
			if (index < 0 || index >= length)
				throw new ArgumentError("Index is out of bounds.");
				
			// Get value from cache if possible.
			var value:* = _cache[index];
			if (value)
				return value;

			// Ask the data provider for the specified items.
			var task:AsyncTask = _getItemsFunction(this, index, prefetch);
			
			if (task.isCompleted)
			{
				return getItemAtOrThrowIfAbsent(index);
			}
			
			// Handle asynchronous case.
			var itemPendingError:ItemPendingError = new ItemPendingError("Item not available.  Will be populated lazily.");
			
			task.onCompletionDo(function(items:Array, error:Error):void
			{
				if (error == null)
				{
					try
					{
						var value:Object = getItemAtOrThrowIfAbsent(index);
						
						ResponderUtils.notifyResultMultiple(itemPendingError.responders, value);
						return;
					}
					catch (e:Error)
					{
						error = e;
					}
				}
				
				ResponderUtils.notifyFaultMultiple(itemPendingError.responders, error);
			});
			
			throw itemPendingError;
		}
		
		/**
		 * Returns true if the item at the specified index has been populated.
		 * 
		 * @index The index.
		 */
		public function isItemPopulatedAt(index:int):Boolean
		{
			if (index < 0 || index >= length)
				throw new ArgumentError("Index is out of bounds.");

			return _cache[index] ? true : false; // maybe undefined;
		}
		
		private function getItemAtOrThrowIfAbsent(index:int):Object
		{
			var value:* = _cache[index];
			if (value)
				return value;
				
			throw new IllegalOperationError("The item at index '" + index + "' has not been populated.");
		}

		public function toArray():Array
		{
			var array:Array = new Array(_length);
			
			for (var i:int = 0; i < _length; i++)
			{
				var value:* = _cache[i];
				if (! value)
				{
					throw new NotSupportedError("Calling toArray() on incompletely populated lazy list is not supported at this time.");
				}
				
				array[i] = value;
			}
			
			return array;
		}
		
		public function getItemIndex(item:Object):int
		{
			for (var prop:String in _cache)
			{
				var index:int = int(prop);
				var cacheItem:Object = _cache[index];
				
				if (item === cacheItem)
					return index;
			}
			
			return -1;
		}
		
		public function addItemAt(item:Object, index:int):void
		{
			throw new IllegalOperationError("List is read-only.");
		}
		
		public function addItem(item:Object):void
		{
			throw new IllegalOperationError("List is read-only.");
		}		

		public function itemUpdated(item:Object, property:Object=null, oldValue:Object=null, newValue:Object=null):void
		{
			throw new IllegalOperationError("List is read-only.");
		}
		
		public function removeAll():void
		{
			throw new IllegalOperationError("List is read-only.");
		}
		
		public function removeItemAt(index:int):Object
		{
			throw new IllegalOperationError("List is read-only.");
		}
		
		public function setItemAt(item:Object, index:int):Object
		{
			throw new IllegalOperationError("List is read-only.");
		}
		
		/**
		 * Sets a series of items in the lazy list.
		 * 
		 * @param index The index of the first item to set.
		 * @param items The array of items to set.
		 */
		public function populateItems(index:int, items:Array):void
		{
			if (index < 0 || index + items.length > _length)
				throw new ArgumentError("The index or number of array items is out of bounds.");
				
			for (var i:int = 0; i < items.length; i++)
			{
				_cache[index + i] = items[i];
			}
			
			dispatchEvent(new CollectionEvent(CollectionEvent.COLLECTION_CHANGE,
				false, false, CollectionEventKind.ADD, index, -1, items));
		}
	}
}