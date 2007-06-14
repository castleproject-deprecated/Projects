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
	/**
	 * A collection delta captures a change to a block of items in
	 * consecutive indices within a collection.  Multiple items may be affected
	 * by a single change.
	 * 
	 * This type effectively replaces CollectionEvent with a more uniform
	 * treatment of events.  Certain edge cases are removed as in the case where
	 * the CollectionEvent.location field might be -1 because items were added
	 * to multiple disjoint regions of a list at once.  Likewise, the items
	 * properties only contain collections of items, not PropertyChangeEvents
	 * as they might have for CollectionEvents of kind REPLACE or UPDATE.
	 */
	public class CollectionDelta
	{
		/**
		 * Creates a collection delta.
		 * 
		 * @param kind The kind of change represented by this delta from the
		 *             CollectionDeltaKind enumeration.
		 * @param location The resultant location of items affected by the change, or -1.
		 * @param items The resultant values of items affected by the change, or null.
		 * @param oldLocation The original location of items affected by the change, or -1.
		 * @param oldItems The original values of items affected by the change, or null.
		 * @param propertyChangeEvents The array of property changes associated with an update, or null.
		 * 
		 * @see kind, location, items, oldLocation, oldItems, propertyChangeEvents.
		 */
		public function CollectionDelta(kind:String, location:int = -1, items:Array = null,
			oldLocation:int = -1, oldItems:Array = null, propertyChangeEvents:Array = null)
		{
			this.kind = kind;
			this.location = location;
			this.items = items;
			this.oldLocation = oldLocation;
			this.oldItems = oldItems;
		}
		
		/**
		 * The kind of change represented by this delta.
		 * One of the values of the CollectionDeltaKind enumeration.
		 */
		public var kind:String;
		
		/**
		 * The index of the resultant location of the first item affected
		 * by a delta of kind ADD, MOVE, REPLACE or UPDATE; otherwise -1.
		 * 
		 * Varies by delta kind:
		 * <ul>
		 * <li>ADD:     Contains the new index at which the first item was inserted.
		 * <li>MOVE:    Contains the new index to which the first item was moved.
		 *              Usually differs from <code>oldLocation</code>.
		 * <li>REMOVE:  Contains -1.
		 * <li>REFRESH: Contains -1.
		 * <li>REPLACE: Contains the index of the first item that was replaced.
		 *              Always equals <code>oldLocation</code>.
		 * <li>RESET:   Contains -1.
		 * <li>UPDATE:  Contains the index of the first item that was updated.
		 *              Always equals <code>oldLocation</code>.
		 * </ul>
		 */
		public var location:int;
		
		/**
		 * The array of resultant items values affected by a delta of kind ADD,
		 * MOVE, REPLACE or UPDATE; otherwise null.
		 * 
		 * Varies by delta kind:
		 * <ul>
		 * <li>ADD:     Contains the array of consecutive items inserted.
		 * <li>MOVE:    Contains the array of consecutive items moved.
		 *              Always equals <code>oldItems</code>.
		 * <li>REMOVE:  Contains null.
		 * <li>REFRESH: Contains null.
		 * <li>REPLACE: Contains the array of the new values of consecutive items replaced.
		 *              Always the same size as <code>oldItems</code> but usually differs in content.
		 * <li>RESET:   Contains null.
		 * <li>UPDATE:  Contains the array of the new values of consecutive items updated.
		 *              Always equals <code>oldItems</code> because the UPDATE event only
		 *              signifies an internal change of state of the items rather than
		 *              a change to the collection unlike REPLACE.
		 * </ul>
		 */
		public var items:Array;
		
		/**
		 * The index of the original location of the first item affected
		 * by a delta of kind MOVE, REMOVE, UPDATE or REPLACE; otherwise -1.
		 * 
		 * Varies by delta kind:
		 * <ul>
		 * <li>ADD:     Contains -1.
		 * <li>MOVE:    Contains the old index from which the first item was moved.
		 *              Usually differs from <code>location</code>.
		 * <li>REMOVE:  Contains the old index of the first item that was removed.
		 * <li>REFRESH: Contains -1.
		 * <li>REPLACE: Contains the index of the first item that was replaced.
		 *              Always equals <code>location</code>.
		 * <li>RESET:   Contains -1.
		 * <li>UPDATE:  Contains the index of the first item that was updated.
		 *              Always equals <code.location</code>.
		 * </ul>
		 */
		public var oldLocation:int;

		/**
		 * The array of original items values affected by a delta of kind MOVE,
		 * REMOVE, REPLACE or UPDATE; otherwise null.
		 * 
		 * Varies by delta kind:
		 * <ul>
		 * <li>ADD:     Contains null.
		 * <li>MOVE:    Contains the array of consecutive items moved.
		 *              Always equals <code>items</code>.
		 * <li>REMOVE:  Contains the array of consecutive items removed.
		 * <li>REFRESH: Contains null.
		 * <li>REPLACE: Contains the array of the old values of consecutive items replaced.
		 *              Always the same size as <code>items</code> but usually differs in content.
		 * <li>RESET:   Contains null.
		 * <li>UPDATE:  Contains the array of the new values of consecutive items updated.
		 *              Always equals <code>items</code> because the UPDATE event only
		 *              signifies an internal change of state of the items rather than
		 *              a change to the collection unlike REPLACE.
		 * </ul>
		 */
		public var oldItems:Array;
		
		/**
		 * The array of property change events associated with a delta of kind UPDATE;
		 * otherwise null.
		 * 
		 * For deltas of kind UPDATE, contains PropertyChangEvents with additional
		 * details about the properties that changed.
		 */
		public var propertyChangeEvents:Array;
	}
}