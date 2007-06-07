package Castle.FlexBridge.Flash.Common
{
	import mx.rpc.IResponder;

	/**
	 * A simple implementation of IResponder that calls the specified
	 * result and fault functions as appropriate.  Unlike ItemResponder
	 * the FunctionResponder does not pass a token object to the functions
	 * so their signature is a little simpler for the common cases.
	 */
	public class FunctionResponder implements IResponder
	{
		private var _result:Function;
		private var _fault:Function;
	
		/**
		 * Constructs a responder.
		 * @param result The result function with signature function(data:Object):void
		 * @param fault The fault function with signature function(info:Object):void
		 */	
		public function FunctionResponder(result:Function, fault:Function)
		{
			_result = result;
			_fault = fault;
		}
		
		public function result(data:Object):void
		{
			_result(data);
		}
		
		public function fault(info:Object):void
		{
			_fault(info);
		}
	}
}