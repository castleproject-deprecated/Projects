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
	import flash.utils.Dictionary;
	
	/**
	 * Default implementation of the inversion of control container.
	 */
	public class DefaultKernel implements IKernel
	{
		private var _handlersByServiceType:Dictionary; /* dictionary of Class to Array of IComponentHandler */
		private var _handlersByComponentKey:Object; /* dictionary of String to IComponentHandler */
		private var _componentModelBuilder:IComponentModelBuilder;

		/**
		 * Creates a kernel initially without any components registered.
		 */		
		public function DefaultKernel()
		{
			_handlersByServiceType = new Dictionary();
			_handlersByComponentKey = new Object();
		}
		
		public function get componentModelBuilder():IComponentModelBuilder
		{
			if (_componentModelBuilder == null)
				_componentModelBuilder = new DefaultComponentModelBuilder();
			return _componentModelBuilder;
		}
		
		public function set componentModelBuilder(value:IComponentModelBuilder):void
		{
			_componentModelBuilder = value;
		}
		
		public function resolve(serviceType:Class):Object
		{
			var handler:IComponentHandler = getComponentHandler(serviceType);
			
			return handler.resolve();
		}
		
		public function resolveByKey(componentKey:String):Object
		{
			var handler:IComponentHandler = getComponentHandlerByKey(componentKey);
			
			return handler.resolve();
		}
		
		public function resolveAll(serviceType:Class):Array /*of the service type*/
		{
			var handlers:Array /*of IComponentHandler*/ = getComponentHandlers(serviceType);
			var components:Array /*of Object*/ = new Array(handlers.length);
			
			for (var i:int; i < handlers.length; i++)
				components[i] = handlers[i].resolve();
				
			return components;
		}
		
		public function registerComponent(componentKey:String, serviceType:Class, componentType:Class, lifestyle:Lifestyle = null):void
		{
			var componentModel:ComponentModel = componentModelBuilder.buildModel(componentKey, serviceType, componentType);

			if (lifestyle != null)
				componentModel.lifestyle = lifestyle;
			
			var componentHandler:IComponentHandler = componentModel.lifestyle.createHandler(this, componentModel);
			registerComponentHandler(componentHandler);
			
			// Register an alternative lookup using the concrete class.
			// TODO: Support more flexible resolution strategies for components.
			if (serviceType != componentType)
				addHandler(componentType, componentHandler);
		}

		public function registerComponentInstance(componentKey:String, serviceType:Class, componentInstance:Object):void
		{
			var componentModel:ComponentModel = new ComponentModel(componentKey, serviceType);
			componentModel.componentActivator = new ExternalComponentActivator(componentInstance);
			
			registerComponentModel(componentModel);
		}
		
		public function registerComponentModel(componentModel:ComponentModel):void
		{
			var componentHandler:IComponentHandler = componentModel.lifestyle.createHandler(this, componentModel);
			registerComponentHandler(componentHandler);
		}
		
		public function registerComponentHandler(componentHandler:IComponentHandler):void
		{
			addHandler(componentHandler.componentModel.serviceType, componentHandler);
		}
		
		public function getComponentHandlers(serviceType:Class):Array /* of IComponentHandler*/
		{
			var handlers:Array = _handlersByServiceType[serviceType] as Array;
			if (! handlers)
				return [];
				
			return handlers;
		}
		
		public function getComponentHandler(serviceType:Class):IComponentHandler
		{
			var handlers:Array = getComponentHandlers(serviceType);
			
			if (handlers.length == 0)
				throw new KernelError("Cannot get the component handler because there are no components registered for service '" + serviceType + "'.");
				
			if (handlers.length > 1)
				throw new KernelError("Cannot get the component handler because there are multiple components are registered for service '" + serviceType + "' so the request is ambiguous.");
				
			var handler:IComponentHandler = handlers[0];
			
			return handler;
		}

		public function getComponentHandlerByKey(componentKey:String):IComponentHandler
		{
			var handler:IComponentHandler = _handlersByComponentKey[componentKey] as IComponentHandler;
			if (! handler)
				throw new KernelError("Cannot get the component handler because there are no components registered with key '" + componentKey + "'.");
			
			return handler;
		}
				
		private function addHandler(serviceType:Class, componentHandler:IComponentHandler):void
		{
			var componentKey:String = componentHandler.componentModel.componentKey;
			var handlerByKey:IComponentHandler = _handlersByComponentKey[componentKey] as IComponentHandler;
			
			if (! handlerByKey)
			{
				_handlersByComponentKey[componentKey] = componentHandler;
			}
			else if (handlerByKey !== componentHandler)
			{
				throw new KernelError("Cannot register the component because there is already another component registered with key '" + componentKey + "'.");
			}
						
			var handlers:Array = _handlersByServiceType[serviceType] as Array;
			if (handlers)
			{
				handlers.push(componentHandler);
			}
			else
			{
				_handlersByServiceType[serviceType] = [ componentHandler ];
			}
		}
	}
}