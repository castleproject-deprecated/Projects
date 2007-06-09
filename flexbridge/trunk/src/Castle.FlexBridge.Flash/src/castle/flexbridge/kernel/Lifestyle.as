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

package castle.flexbridge.kernel
{
	/**
	 * A component lifestyle type specifies how a component is to be
	 * created and disposed.  A few simple lifestyle types are defined
	 * as constants here.
	 */
	public final class Lifestyle
	{
		/**
		 * An IComponentHandler class with constructor signature
		 * function(componentKey:String, service:Class, component:Class):IComponentHandler.
		 */
		private var _handlerClass:Class;
		
		/**
		 * The singleton lifestyle.
		 */
		public static const SINGLETON:Lifestyle = new Lifestyle(SingletonComponentHandler);
		
		/**
		 * The transient lifestyle.
		 */
		public static const TRANSIENT:Lifestyle = new Lifestyle(TransientComponentHandler);
		
		/**
		 * Creates a custom lifestyle with the specified handler factory.
		 * 
		 * @param handlerClass An IComponentHandler class with constructor signature
		 * function(service:Class, component:Class, componentKey:String):IComponentHandler
		 */
		public function Lifestyle(handlerClass:Class)
		{
			_handlerClass = handlerClass;
		}
		
		/**
		 * Creates a handler for the specified component model.
		 * 
		 * @param kernel The kernel.
		 * @param componentModel The component model.
		 */
		public function createHandler(kernel:IKernel, componentModel:ComponentModel):IComponentHandler
		{
			return IComponentHandler(new _handlerClass(kernel, componentModel));
		}
	}
}