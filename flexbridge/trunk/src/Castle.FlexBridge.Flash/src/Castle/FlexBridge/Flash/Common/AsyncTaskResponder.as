package Castle.FlexBridge.Flash.Common
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