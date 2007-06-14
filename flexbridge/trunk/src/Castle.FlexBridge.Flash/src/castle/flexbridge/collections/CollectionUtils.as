// Copyright 2007 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

package castle.flexbridge.collections
{
	import mx.events.CollectionEvent;
	import mx.events.CollectionEventKind;
	import mx.events.PropertyChangeEvent;
	import mx.collections.IList;
	import castle.flexbridge.common.NotSupportedError;

	
	/**
	 * Provides utilities for manipulating collections.
	 */
	public final class CollectionUtils
	{	
		/**
		 * Converts a CollectionEvent to an array of CollectionDeltas.
		 * 
		 * CollectionDeltas are easier to process than CollectionEvents because
		 * there are fewer exceptions and special cases.
		 * 
		 * @param e The collection event.
		 * @return The array of corresponding collection deltas.
		 */
		public static function convertCollectionEventToDeltas(e:CollectionEvent):Array
		{
			switch (e.kind)
			{
				case CollectionEventKind.REFRESH:
					return [ new CollectionDelta(CollectionDeltaKind.REFRESH) ];
				
				case CollectionEventKind.RESET:
					return [ new CollectionDelta(CollectionDeltaKind.RESET) ];

				case CollectionEventKind.ADD:
					if (e.items.length == 0)
						return [ ];
					if (e.location < 0)
						break;
						
					return [ new CollectionDelta(CollectionDeltaKind.ADD, e.location, e.items) ];

				case CollectionEventKind.REMOVE:
					if (e.items.length == 0)
						return [ ];
					if (e.location < 0)
						break;
						
					return [ new CollectionDelta(CollectionDeltaKind.REMOVE, -1, null, e.location, e.items) ];

				case CollectionEventKind.MOVE:
					if (e.items.length == 0)
						return [ ];
					if (e.oldLocation < 0 || e.location < 0)
						break;
					
					// FIXME: I'm not sure that e.items actually does contain the array of
					//        items moved for MOVE events.  The documentation is curiously
					//        silent on this point. -- Jeff.
					return [ new CollectionDelta(CollectionDeltaKind.MOVE, e.location, e.items, e.oldLocation, e.items) ];
					
				case CollectionEventKind.REPLACE:
					if (e.items.length == 0)
						return [ ];
					if (e.location < 0)
						break;

					var replacedItemCount:int = e.items.length;
					var oldItems:Array = new Array(replacedItemCount);
					var newItems:Array = new Array(replacedItemCount);
					
					for (var i:int = 0; i < replacedItemCount; i++)
					{
						var replacePropertyChangeEvent:PropertyChangeEvent = PropertyChangeEvent(e.items[i]);
						
						oldItems[i] = replacePropertyChangeEvent.oldValue;
						newItems[i] = replacePropertyChangeEvent.newValue;
					}
						
					return [ new CollectionDelta(CollectionDeltaKind.REPLACE, e.location, oldItems, e.location, newItems) ];
					
				case CollectionEventKind.UPDATE:
					return handleUpdateEvent(e);
			}
			
			// At times we translate events with unknown location (-1) to RESET
			// because without it we cannot produce a useful delta.  It is not
			// especially useful to know that a change occurred "somewhere"
			// if we can't track it.  In some cases we could try harder to
			// rederive the index information but that seems excessively wasteful.
			return [ new CollectionDelta(CollectionDeltaKind.RESET) ];
		}
		
		private static function handleUpdateEvent(e:CollectionEvent):Array
		{
			if (e.items.length == 0)
				return [ ];
			if (e.location < 0)
				throw new NotSupportedError("Support for update events without an associated location " + 
						"has not been implemented at this time as it requires looking up " + 
						"the index of the changed items.");
				
			var propertyChangeEvents:Array = e.items;
			var updatedItemCount:int = propertyChangeEvents.length;
			var updatedItems:Array = new Array(updatedItemCount);

			for (var i:int = 0; i < updatedItemCount; i++)
			{
				var propertyChangeEvent:PropertyChangeEvent = PropertyChangeEvent(propertyChangeEvents[i]);
				
				updatedItems[i] = propertyChangeEvent.target ? propertyChangeEvent.target : propertyChangeEvent.source;
			}
			
			return [ new CollectionDelta(CollectionDeltaKind.UPDATE, e.location, updatedItems, e.location, updatedItems, propertyChangeEvents) ];
		}
	}
}