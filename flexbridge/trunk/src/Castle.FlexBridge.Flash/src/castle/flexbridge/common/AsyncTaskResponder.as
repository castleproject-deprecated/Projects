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
	import mx.rpc.IResponder;
	import mx.rpc.events.ResultEvent;
	import mx.rpc.events.FaultEvent;
	
	/**
	 * Private adapter used to present an AsyncTask as an IResponder.
	 */
	internal class AsyncTaskResponder implements IResponder
	{
		private var _asyncTask:AsyncTask;
		
		/**
		 * Creates a wrapper of the specified task.
		 */
		public function AsyncTaskResponder(asyncTask:AsyncTask)
		{
			_asyncTask = asyncTask;
		}
		
		public function result(data:Object):void
		{
			var resultEvent:ResultEvent = data as ResultEvent;
			_asyncTask.done(resultEvent ? resultEvent.result : data);
		}
		
		public function fault(info:Object):void
		{
			var error:Error = info as Error;
			if (error)
			{
				_asyncTask.failed(error);
			}
			else
			{			
				var faultEvent:FaultEvent = info as FaultEvent;
				if (faultEvent)
				{
					_asyncTask.failed(faultEvent.fault);
				}
				else
				{
					_asyncTask.failed(new Error("Asynchronous task failed: " + info));
				}
			}
		}
	}
}