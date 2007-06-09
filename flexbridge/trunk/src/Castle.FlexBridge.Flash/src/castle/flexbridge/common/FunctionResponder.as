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