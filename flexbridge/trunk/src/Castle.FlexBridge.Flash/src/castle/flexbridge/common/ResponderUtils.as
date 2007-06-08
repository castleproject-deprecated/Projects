package castle.flexbridge.common
{
	import mx.rpc.IResponder;
	
	/**
	 * Utilities for working with IResponder.
	 */
	public class ResponderUtils
	{
		/**
		 * Notifies multiple responders with a result.
		 * Catches and reports any errors thrown to the trace log.
		 */
		public static function notifyResultMultiple(responders:Array, result:Object):void
		{
			for each (var responder:IResponder in responders)
				notifyResult(responder, result);
		}

		/**
		 * Notifies multiple responders with a fault.
		 * Catches and reports any errors thrown to the trace log.
		 */
		public static function notifyFaultMultiple(responders:Array, fault:Object):void
		{
			for each (var responder:IResponder in responders)
				notifyFault(responder, fault);
		}

		/**
		 * Notifies a responder with a result.
		 * Catches and reports any errors thrown to the trace log.
		 */
		public static function notifyResult(responder:IResponder, result:Object):void
		{
			try
			{
				responder.result(result);
			}
			catch (e:Error)
			{
				trace("Responder.result threw an error: " + e.getStackTrace());
			}
		}
		
		/**
		 * Notifies a responder with a fault.
		 * Catches and reports any errors thrown to the trace log.
		 */
		public static function notifyFault(responder:IResponder, fault:Object):void
		{
			try
			{
				responder.fault(fault);
			}
			catch (e:Error)
			{
				trace("Responder.fault threw an error: " + e.getStackTrace());
			}
		}
	}
}