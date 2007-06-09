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
	import flash.events.EventDispatcher;
	
	/**
	 * The event fired when the value of the task property may have changed.
	 */
	[Event(name="taskChange", type="flash.events.Event")]

	/**
	 * The event fired when the status of the task may have changed.
	 */
	[Event(name="statusChange", type="castle.flexbridge.common.AsyncTaskEvent")]
	
	/**
	 * Holds an AsyncTask instance that is lazily populated and can then
	 * be refreshed on demand.  The holder supports data binding and is
	 * is used as a cache for asynchronously populated values.
	 */
	public class AsyncTaskHolder extends EventDispatcher
	{
		private var _populator:Function;
		private var _task:AsyncTask;
		
		/**
		 * Creates an async task holder.
		 * 
		 * @param populator A function to call to populate the <code>task</code>
		 *   of the holder.  Must have the signature <code>function():AsyncTask</code>.
		 */
		public function AsyncTaskHolder(populator:Function)
		{
			_populator = populator;
		}
		
		/**
		 * Gets the task.
		 * Lazily populates it if needed.
		 */
		[Bindable(event="taskChange")]
		public function get task():AsyncTask
		{
			populate();
			
			return _task;
		}

		/**
		 * Returns the status of the task.
		 * @see AsyncTask.STATUS_PENDING, AsyncTask.STATUS_OK, AsyncTask.STATUS_FAULT.
		 */
		[Bindable(event="statusChange")]
		public function get status():int
		{
			return task.status;
		}
		
		/**
		 * Returns true if the task is completed.
		 * Returns false if it is still pending notification of completion.
		 */
		[Bindable(event="statusChange")]
		public function get isCompleted():Boolean
		{
			return task.isCompleted;
		}
		
		/**
		 * Returns true if the task is done and it reported a successful result.
		 * Returns false if the task is still pending notification of completion
		 * or if it failed with an error.
		 */
		[Bindable(event="statusChange")]
		public function get isResultOk():Boolean
		{
			return task.isResultOk;
		}
		
		/**
		 * Gets the result of the task or undefined if the task is pending.
		 * Returns null if the task completed with an error.
		 */
		[Bindable(event="statusChange")]
		public function get result():*
		{
			return task.result;
		}
		
		/**
		 * Gets the error thrown by the task or undefined if the task is pending.
		 * Returns null if the task completed successfully.
		 */
		[Bindable(event="statusChange")]
		public function get error():*
		{
			return task.error;
		}
		
		/**
		 * Clears the task so that it will be repopulated on the next request.
		 * Causes a <code>taskChange</code> event to be fired and any bindings
		 * on <code>task</code> to be invalidated.
		 */
		public function clear():void
		{
			if (_task != null)
			{
				_task.removeEventListener(AsyncTaskEvent.STATUS_CHANGE, statusChangeHandler);
				_task = null;
			}
			
			notifyTaskChange();
		}
		
		/**
		 * Causes the task to be populated if it hasn't been already.
		 * This is useful for prepopulating the holder before any requests to
		 * the <code>task</code> property are made.
		 */
		public function populate():void
		{
			if (_task == null)
			{
				_task = _populator();
				
				// Note: Uses weak reference so the holder can be garbage collected
				//       independently of the task.
				_task.addEventListener(AsyncTaskEvent.STATUS_CHANGE, statusChangeHandler, false, 0, true);
				
				if (_task.status != AsyncTask.STATUS_PENDING)
					notifyStatusChange();
				
				// Note: Doesn't fire a task change notification because the effects of
				//       populate() cannot be directly observed unlike clearing
				//       the task which implies that a new task will be produced
				//       on demand so the instance will be different when observed.
			}
		}
		
		/**
		 * Refreshes the task.
		 * Equivalent to calling <code>clear</code> followed by <code>populate</code>.
		 */
		public function refresh():void
		{
			clear();
			populate();
		}
		
		private function notifyTaskChange():void
		{
			dispatchEvent(new Event("taskChange"));			
			notifyStatusChange();
		}
		
		private function notifyStatusChange():void
		{
			dispatchEvent(new AsyncTaskEvent(AsyncTaskEvent.STATUS_CHANGE));			
		}
		
		private function statusChangeHandler(e:AsyncTaskEvent):void
		{
			notifyStatusChange();
		}
	}
}