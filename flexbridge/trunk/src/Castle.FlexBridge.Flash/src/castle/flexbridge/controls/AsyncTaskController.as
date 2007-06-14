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
	import flash.events.EventDispatcher;
	import mx.core.IMXMLObject;
	import castle.flexbridge.common.AsyncTask;
	import castle.flexbridge.common.AsyncTaskHolder;
	import castle.flexbridge.common.AsyncTaskEvent;
	
	[Event(name="holderChange", type="flash.events.Event")]
	[Event(name="taskChange", type="flash.events.Event")]
	[Event(name="result", type="castle.flexbridge.common.AsyncTaskEvent")]
	[Event(name="error", type="castle.flexbridge.common.AsyncTaskEvent")]
	[Event(name="statusChange", type="castle.flexbridge.common.AsyncTaskEvent")]
	[Event(name="pending", type="flash.events.Event")]
	
	/**
	 * An AsyncTaskController is an MXML component that wraps an
	 * AsyncTaskHolder and provides support for dispatching various
	 * events.  It is intended as a convenience for building a UI
	 * that makes extensive use of an AsyncTask.
	 */
	public class AsyncTaskController extends EventDispatcher implements IMXMLObject
	{
		private var _holder:AsyncTaskHolder;
		private var _result:*;
		private var _error:*;
		
		/**
		 * Called by the MXML framework after the component has been initialized.
		 */
		public function initialized(document:Object, id:String):void
		{
			if (_holder == null)
				dispatchEvent(new Event("pending"));
		}
		
		/**
		 * Gets or sets the AsyncTaskHolder.
		 */
		[Bindable(event="holderChange")]
		public function get holder():AsyncTaskHolder
		{
			return _holder;
		}
		
		public function set holder(value:AsyncTaskHolder):void
		{
			if (_holder != value)
			{
				if (_holder != null)
				{
					_holder.removeEventListener("taskChange", taskChangeHandler);
					_holder.removeEventListener(AsyncTaskEvent.STATUS_CHANGE, statusChangeHandler);
				}
				
				_holder = value;
				
				if (_holder != null)
				{
					// Note: Uses weak references so that the controller can
					//       be garbage collected independently of the holder.
					_holder.addEventListener("taskChange", taskChangeHandler, false, 0, true);
					_holder.addEventListener(AsyncTaskEvent.STATUS_CHANGE, statusChangeHandler, false, 0, true);
				}
								
				dispatchEvent(new Event("holderChange"));
				notifyTaskChange();
			}
		}
		
		/**
		 * Gets the current AsyncTask or null if none.
		 */
		[Bindable(event="taskChange")]
		public function get task():AsyncTask
		{
			return _holder != null ? _holder.task : null;
		}

		/**
		 * Returns the status of the task.
		 * @see AsyncTask.STATUS_PENDING, AsyncTask.STATUS_OK, AsyncTask.STATUS_FAULT.
		 */
		[Bindable(event="statusChange")]
		public function get status():int
		{
			return _holder != null ? _holder.status : AsyncTask.STATUS_PENDING;
		}
		
		/**
		 * Returns true if the task is completed.
		 * Returns false if it is still pending notification of completion.
		 */
		[Bindable(event="statusChange")]
		public function get isCompleted():Boolean
		{
			return _holder != null ? _holder.isCompleted : false;
		}
		
		/**
		 * Returns true if the task is done and it reported a successful result.
		 * Returns false if the task is still pending notification of completion
		 * or if it failed with an error.
		 */
		[Bindable(event="statusChange")]
		public function get isResultOk():Boolean
		{
			return _holder != null ? _holder.isResultOk : false;
		}
		
		/**
		 * Gets the result of the task or undefined if the holder is null
		 * or the task is pending.  Returns null if the task completed
		 * with an error.
		 */
		[Bindable(event="statusChange")]
		public function get result():*
		{
			return _holder != null ? _holder.result : undefined;
		}
		
		/**
		 * Gets the error thrown by the task or undefined if the holder is null
		 * or the task is pending.  Returns null if the task completed
		 * successfully.
		 */
		[Bindable(event="statusChange")]
		public function get error():*
		{
			return _holder != null ? _holder.error : undefined;
		}
		
		/**
		 * Refreshes the holder.
		 */
		public function refresh():void
		{
			if (_holder != null)
				_holder.refresh();
		}
		
		private function notifyTaskChange():void
		{
			dispatchEvent(new Event("taskChange"));
			notifyStatusChange();
		}
		
		private function notifyStatusChange():void
		{
			dispatchEvent(new AsyncTaskEvent(AsyncTaskEvent.STATUS_CHANGE));
			
			var cacheTask:AsyncTask = _holder.task;
			if (cacheTask != null && cacheTask.isCompleted)
			{
				if (cacheTask.isResultOk)
					dispatchEvent(new AsyncTaskEvent(AsyncTaskEvent.RESULT));
				else
					dispatchEvent(new AsyncTaskEvent(AsyncTaskEvent.ERROR));
			}
			else
			{
				dispatchEvent(new Event("pending"));
			}
		}
		
		private function taskChangeHandler(e:Event):void
		{
			notifyTaskChange();
		}
		
		private function statusChangeHandler(e:AsyncTaskEvent):void
		{
			notifyStatusChange();
		}
	}
}