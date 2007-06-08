package castle.flexbridge.common
{
	import mx.rpc.AsyncToken;
	import mx.rpc.IResponder;
	import flash.errors.IllegalOperationError;
	import mx.rpc.events.ResultEvent;
	import flash.utils.*;
	import flash.events.EventDispatcher;
	
	[Event(name="result", type="castle.flexbridge.common.AsyncTaskEvent")]
	[Event(name="error", type="castle.flexbridge.common.AsyncTaskEvent")]
	[Event(name="statusChange", type="castle.flexbridge.common.AsyncTaskEvent")]
	
	/**
	 * An AsyncTask represents a promise made by an asynchronous computation
	 * to yield some result eventually.  Tasks can be combined in a number
	 * of ways to facilitate chaining computations dependent upon asynchronous
	 * results, including spawning new asynchronous tasks.
	 * 
	 * On useful programming idiom here is that the AsyncTask start methods
	 * allocate the task themselves and pass it to the user's function.  They
	 * perform error handling and can report a failure through the chain of
	 * tasks thus being constructure.  So we can define control flow within
	 * the chain of tasks and for the most part need not worry about errors
	 * that occurred while the chain was being constructed.
	 * 
	 * An AsyncTask can be used much like a Future (aka. Promise) in
	 * other programming languages via data binding of the various properties,
	 * by watching events, or by attaching responders.
	 */
	public class AsyncTask extends EventDispatcher
	{
		/**
		 * The task is still pending notification of completion.
		 */
		public static const STATUS_PENDING:int = 0;
		
		/**
		 * The task completed successfully.
		 */
		public static const STATUS_OK:int = 1;
		
		/**
		 * The task completed with an error.
		 */
		public static const STATUS_ERROR:int = 2;
		
		private var _status:int = STATUS_PENDING;
		private var _responders:Array;
		private var _asResponder:IResponder;
		private var _output:Object;
		
		/**
		 * Creates an asynchronous task, initially in the STATUS_PENDING state.
		 */
		public function AsyncTask()
		{
		}
		
		/**
		 * Returns the status of the task.
		 * @see STATUS_PENDING, STATUS_OK, STATUS_FAULT.
		 */
		[Bindable(event="statusChange")]
		public function get status():int
		{
			return _status;
		}
		
		/**
		 * Returns true if the task is completed.
		 * Returns false if it is still pending notification of completion.
		 */
		[Bindable(event="statusChange")]
		public function get isCompleted():Boolean
		{
			return _status != STATUS_PENDING;
		}
		
		/**
		 * Returns true if the task is done and it reported a successful result.
		 * Returns false if the task is still pending notification of completion
		 * or if it failed with an error.
		 */
		[Bindable(event="statusChange")]
		public function get isResultOk():Boolean
		{
			return _status == STATUS_OK;
		}
		
		/**
		 * Returns the result of the task.
		 * 
		 * @return The Object passed to done() if the task completed successfully,
		 *     null if the task failed, or undefined if pending completion.
		 */
		[Bindable(event="statusChange")]
		public function get result():*
		{
			if (_status != STATUS_OK)
			{
				if (_status == STATUS_PENDING)
					return undefined;
				else
					return null;
			}
			
			return _output;
		}
		
		/**
		 * Returns the error thrown by the task.
		 * 
		 * @return The Error passed to failed() if the task failed,
		 *     null if the task completed successfully, or undefined if pending completion.
		 */
		[Bindable(event="statusChange")]
		public function get error():*
		{
			if (_status != STATUS_ERROR)
			{
				if (_status == STATUS_PENDING)
					return undefined;
				else
					return null;
			}
			
			return Error(_output);
		}
		
		/**
		 * Signals that the task completed successfully and reports the specified result.
		 * Throws an error if the task has already completed.
		 * 
		 * @param result The result (may be null).
		 */
		public function done(result:Object = null):void
		{
			if (_status != STATUS_PENDING)
				throw new IllegalOperationError("The task has already completed.");
				
			_status = STATUS_OK;
			_output = result;
			
			dispatchEvent(new AsyncTaskEvent(AsyncTaskEvent.STATUS_CHANGE));
			dispatchEvent(new AsyncTaskEvent(AsyncTaskEvent.RESULT));
			notifyResponders();
		}
		
		/**
		 * Signals that the task failed and reports the specified error.
		 * Throws an error if the task has already completed.
		 * 
		 * @param error The error (never null).
		 */
		public function failed(error:Error):void
		{
			if (! error)
				throw new ArgumentError("The error must not be null");
				
			if (_status != STATUS_PENDING)
				throw new IllegalOperationError("The task has already completed.");

			_status = STATUS_ERROR;
			_output = error;
			
			trace("Task failed with error: " + error.getStackTrace());
			
			dispatchEvent(new AsyncTaskEvent(AsyncTaskEvent.STATUS_CHANGE));
			dispatchEvent(new AsyncTaskEvent(AsyncTaskEvent.ERROR));
			notifyResponders();
		}
		
		/**
		 * Adds a responder to the list of responders to be notified when
		 * the task completes.  If the task has already completed, the responder
		 * will be immediately notified.
		 * 
		 * @param responder The responder.
		 */
		public function addResponder(responder:IResponder):void
		{
			if (_status != STATUS_PENDING)
			{
				if (_status == STATUS_OK)
					ResponderUtils.notifyResult(responder, _output);
				else
					ResponderUtils.notifyFault(responder, _output);
			}
			else
			{
				if (! _responders)
					_responders = new Array();
				_responders.push(responder);
			}
		}

		/**
		 * Calls the specified function passing in a new task.
		 * The function should prepare and start the asynchronous operation such that
		 * the task will be signalled when the operation completes.  If the operation
		 * completes synchronously, the function should call the task's "done"
		 * method with the result before it returns.
		 * 
		 * If the function throws an error and the task has not been signalled
		 * completion, the task's "failed" method is automatically called with
		 * the error.
		 * 
		 * @param block The function to run inside the task.
		 *              It must have the signature function(task:AsyncTask):void.
		 * @return The new task.
		 */
		public static function start(block:Function):AsyncTask
		{
			var task:AsyncTask = new AsyncTask();
			
			try
			{
				block(task);
			}
			catch (e:Error)
			{
				if (! task.isCompleted)
					task.failed(e);
			}
			
			return task;
		}
		
		/**
		 * Calls the specified function passing in a new task repeatedly
		 * until the task completes with a result of "false" or fails with
		 * an error.  The result of the task must be a Boolean.
		 * 
		 * If the function throws an error and the task has not been signalled
		 * completion, the task's "failed" method is automatically called with
		 * the error.
		 * 
		 * @param block The function to run inside the loop of tasks.
		 *              It must have the signature function(task:AsyncTask):void.
		 * @return The loop task.
		 */
		public static function startLoopUntilFalse(block:Function):AsyncTask
		{
			var task:AsyncTask = new AsyncTask();
			
			return start(block).onResultStart(function(task:AsyncTask, continueFlag:Boolean):void
			{
				if (continueFlag)			
					task.joinTask(startLoopUntilFalse(block));
				else
					task.done();
			});
		}
		
		/**
		 * When this task completes successfully, calls the specified function
		 * passing in the result from this task.
		 * 
		 * @param block The function to run when this task completes.
		 *              It must have the signature function(result:Object):void.
		 * @return This task (as a convenience for chaining).
		 */
		public function onResultDo(block:Function):AsyncTask
		{
			addResponder(new FunctionResponder(
				block,
				function(error:Error):void { }));
			
			return this;
		}
		
		/**
		 * When this task completes successfully, calls the specified function
		 * passing in the composite task and the result from this task.
		 * If this task fails with an error, the specified function is not called
		 * and the composite task is automatically signalled with the error.
		 * 
		 * Behaves like "start" except that the specified function is only
		 * called after this task completes successfully.
		 * 
		 * @param block The function to run inside the composite task.
		 *              It must have the signature function(task:AsyncTask, result:Object):void.
		 * @return The composite task.
		 */
		public function onResultStart(block:Function):AsyncTask
		{
			var task:AsyncTask = new AsyncTask();

			addResponder(new FunctionResponder(
				function(result:Object):void
				{
					try
					{
						block(task, result);
					}
					catch (e:Error)
					{
						if (! task.isCompleted)
							task.failed(e);
					}					
				},
				task.failed));
			
			return task;
		}
		
		/**
		 * When this task fails with an error, calls the specified function
		 * passing in the error from this task.
		 * 
		 * @param block The function to run when this task completes.
		 *              It must have the signature function(error:Error):void.
		 * @return This task (as a convenience for chaining).
		 */
		public function onErrorDo(block:Function):AsyncTask
		{
			addResponder(new FunctionResponder(
				function(result:Object):void { },
				block));
			
			return this;
		}
		
		/**
		 * When this task fails with an error, calls the specified function
		 * passing in the composite task and the error from this task.
		 * If this task completes successfully, the specified function is not called
		 * and the composite task is automatically signalled with the result.
		 * 
		 * Behaves like "start" except that the specified function is only
		 * called after this task fails with an error.
		 * 
		 * @param block The function to run inside the composite task.
		 *              It must have the signature function(task:AsyncTask, error:Error):void.
		 * @return The composite task.
		 */
		public function onErrorStart(block:Function):AsyncTask
		{
			var task:AsyncTask = new AsyncTask();

			addResponder(new FunctionResponder(
				task.done,
				function(error:Error):void
				{
					try
					{
						block(task, error);
					}
					catch (e:Error)
					{
						if (! task.isCompleted)
							task.failed(e);
					}
				}));
			
			return task;			
		}

		/**
		 * When this task completes, calls the specified function
		 * passing in the result and error from this task.
		 * 
		 * If this task completed successfully, the error will be null
		 *     and the result will be whatever the task returned which may be null.
		 * If this task failed with an error, the error will be non-null
		 *     and the result will be null.
		 * 
		 * Thus to check for successful completion, compare the error with null.
		 * 
		 * @param block The function to run when this task completes.
		 *              It must have the signature function(result:Object, error:Error):void.
		 * @return This task (as a convenience for chaining).
		 */
		public function onCompletionDo(block:Function):AsyncTask
		{
			addResponder(new FunctionResponder(
				function(result:Object):void
				{
					block(result, null);
				},
				function(error:Error):void
				{
					block(null, error);
				}));
			
			return this;
		}
		
		/**
		 * When this task completes, calls the specified function passing in the
		 * composite task and the result and error from this task.
		 * 
		 * If this task completed successfully, the error will be null
		 *     and the result will be whatever the task returned which may be null.
		 * If this task failed with an error, the error will be non-null
		 *     and the result will be null.
		 * 
		 * Thus to check for successful completion, compare the error with null.
		 * 
		 * Behaves like "start" except that the specified function is only
		 * called after this task completes.
		 * 
		 * @param block The function to run inside the composite task.
		 *              It must have the signature function(task:AsyncTask, result:Object, error:Error):void.
		 * @return The composite task.
		 */
		public function onCompletionStart(block:Function):AsyncTask
		{
			var task:AsyncTask = new AsyncTask();

			addResponder(new FunctionResponder(
				function(result:Object):void
				{
					try
					{
						block(task, result, null);
					}
					catch (e:Error)
					{
						if (! task.isCompleted)
							task.failed(e);
					}
				},
				function(error:Error):void
				{
					try
					{
						block(task, null, error);
					}
					catch (e:Error)
					{
						if (! task.isCompleted)
							task.failed(e);
					}
				}));
			
			return task;			
		}
		
		/**
		 * Joins this task to another one.
		 * When the other task completes, this one will also be notified of completion
		 * with the same result or error as the specified task.
		 * 
		 * Throws an exception if the task is already completed.
		 * 
		 * @param task The task to join.
		 */
		public function joinTask(task:AsyncTask):void
		{
			if (_status != STATUS_PENDING)
				throw new IllegalOperationError("The task has already completed.");
			
			task.addResponder(asResponder());
		}
		
		/**
		 * Joins this task to zero or more other ones.
		 * When all of the other tasks have completed, this one will be notified of
		 * completion.  The result of execution will be an array of tuples of
		 * the following form { "result": result, "error": error }
		 * for each of the tasks.
		 * 
		 * The task completes immediately if the specified array is empty.
		 *
		 * Throws an exception if the task is already completed.
		 * 
		 * @param tasks The array of tasks to join.
		 */
		public function joinTasks(tasks:Array):void
		{
			if (_status != STATUS_PENDING)
				throw new IllegalOperationError("The task has already completed.");
				
			if (tasks.length == 0)
			{
				done([ ]);
				return;
			}
			
			var results:Array = new Array(tasks.length);
			var count:int = tasks.length;
			
			for (var i:int = 0; i < tasks.length; i++)
			{
				tasks[i].addResponder(new FunctionResponder(
					function(result:Object):void
					{
						results[i] = { "result": result, "error": null };
						
						if (--count == 0)
							done(results);
					},
					function(error:Error):void
					{
						results[i] = { "result": null, "error": error };
						
						if (--count == 0)
							done(results);
					}));
			}
		}
		
		/**
		 * Adds this task as a responder to the specified asynchronous token.
		 * The task will be notified of completion when the async token is signalled.
		 * 
		 * Throws an exception if the task is already completed.
		 * 
		 * @param asyncToken The async token to attach to.
		 */
		public function joinToken(asyncToken:AsyncToken):void
		{
			if (_status != STATUS_PENDING)
				throw new IllegalOperationError("The task has already completed.");
				
			asyncToken.addResponder(asResponder());
		}
		
		/**
		 * Returns a wrapper for the task as an IResponder.
		 * The task will be notified of completion when the responder is signalled.
		 * 
		 * @return The responder wrapper.
		 */
		public function asResponder():IResponder
		{
			if (! _asResponder)
				_asResponder = new AsyncTaskResponder(this);
			return _asResponder;
		}
		
		private function notifyResponders():void
		{
			if (_responders)
			{
				if (_status == STATUS_OK)
					ResponderUtils.notifyResultMultiple(_responders, _output);
				else
					ResponderUtils.notifyFaultMultiple(_responders, _output);
					
				// Clear the array of responders since we will not need them anymore.
				_responders = null;
			}
		}
	}
}