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