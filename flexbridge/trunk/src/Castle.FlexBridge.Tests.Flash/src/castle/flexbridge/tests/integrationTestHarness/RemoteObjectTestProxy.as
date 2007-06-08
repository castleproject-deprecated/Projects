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

package castle.flexbridge.tests.integrationTestHarness
{
	import mx.rpc.remoting.RemoteObject;
	import flash.external.ExternalInterface;
	import com.renaun.rpc.RemoteObjectAMF0;
	import mx.rpc.AsyncToken;
	import mx.rpc.IResponder;
	import mx.rpc.Responder;
	import mx.rpc.Fault;
	import flash.utils.ByteArray;
	import mx.utils.Base64Encoder;
	import flash.net.ObjectEncoding;
	import mx.rpc.events.ResultEvent;
	import mx.rpc.events.FaultEvent;
	
	/**
	 * Provides support for testing RemoteObject interfacing.
	 */
	public class RemoteObjectTestProxy
	{
		private var remoteObject:*;
		
		private var lastToken:AsyncToken;
		private var lastMethodResult:MethodResult;
		
		/**
		 * Installs the test harness on the ExternalInterface.
		 */
		public function installExternalInterface():void
		{
			ExternalInterface.addCallback("remoteObjectTestProxy_Initialize", initialize);
			ExternalInterface.addCallback("remoteObjectTestProxy_SetCredentials", setCredentials);
			ExternalInterface.addCallback("remoteObjectTestProxy_SetRemoteCredentials", setRemoteCredentials);
			ExternalInterface.addCallback("remoteObjectTestProxy_InvokeMethod", invokeMethod);
			ExternalInterface.addCallback("remoteObjectTestProxy_GetLastMethodResult", getLastMethodResult);
			ExternalInterface.addCallback("remoteObjectTestProxy_Logout", logout);
		}
		
		/**
		 * Initializes the RemoteObject with the specified destination and object encoding.
		 * 
		 * @param destination The destination name defined in services-config.xml
		 * @param objectEncoding The object encoding name: "AMF0" or "AMF3".
		 */
		public function initialize(destination:String, objectEncoding:String):void
		{
			if (objectEncoding == "AMF0")
			{
				remoteObject = new RemoteObjectAMF0();
				remoteObject.destination = destination;				
			}
			else if (objectEncoding == "AMF3")
			{
				remoteObject = new RemoteObject(destination);
			}
			else
				throw new Error("Unsupported encoding: " + objectEncoding
					+ ".  Must be 'AMF0' or 'AMF3'.");
		}
		
		/**
		 * Sets the credentials for the RemoteObject.
		 * 
		 * @param username The username.
		 * @param password The password.
		 */
		public function setCredentials(username:String, password:String):void
		{
			remoteObject.setCredentials(username, password);
		}

		/**
		 * Sets the remote credentials for the RemoteObject.
		 * 
		 * @param username The username.
		 * @param password The password.
		 */
		public function setRemoteCredentials(username:String, password:String):void
		{
			remoteObject.setRemoteCredentials(username, password);
		}
		
		/**
		 * Invokes a remote method.
		 * 
		 * @param methodName The method name.
		 * @param amfBase64Args The base-64 and AMF encoded array of method arguments.
		 */
		public function invokeMethod(methodName:String, amfBase64Args:String):void
		{
			var args:Array = AMF.decode(Base64.decode(amfBase64Args)) as Array;
						
			setToken(remoteObject[methodName].send(args));
		}
		
		/**
		 * Gets the response from the most recently executed remote method.
		 * 
		 * @return The base-64 and AMF encoded MethodResponse, or null if no response is available yet.
		 */
		public function getLastMethodResult():String
		{
			if (lastMethodResult == null)
				return null;
				
			return Base64.encode(AMF.encode(lastMethodResult));
		}
		
		/**
		 * Logs out the user of the RemoteObject.
		 */
		public function logout():void
		{
			remoteObject.logout();
		}
		
		private function setToken(token:AsyncToken):void
		{
			lastToken = token;
			lastMethodResult = null;
			
			token.addResponder(new Responder(function(resultEvent:ResultEvent):void
			{
				if (token != lastToken)
					return;
				
				lastMethodResult = new MethodResult(resultEvent.result, null);
			}, function(faultEvent:FaultEvent):void
			{
				if (token != lastToken)
					return;
					
				lastMethodResult = new MethodResult(null, faultEvent.fault.getStackTrace());
			}));
		}
	}
}