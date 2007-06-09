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