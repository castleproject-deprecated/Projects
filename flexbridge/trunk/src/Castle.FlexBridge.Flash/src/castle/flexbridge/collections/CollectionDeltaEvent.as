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