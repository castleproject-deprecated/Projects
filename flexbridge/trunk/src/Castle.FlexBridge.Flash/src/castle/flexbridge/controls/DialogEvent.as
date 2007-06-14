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