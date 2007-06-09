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
    import castle.flexbridge.common.*;
	
	/**
	 * Base class for component handlers.
	 */
	public class BaseComponentHandler implements IComponentHandler
	{
		private var _kernel:IKernel;
		private var _componentModel:ComponentModel;
		
		public function BaseComponentHandler(kernel:IKernel, componentModel:ComponentModel)
		{
			_kernel = kernel;
			_componentModel = componentModel;
		}
		
		public function get kernel():IKernel
		{
			return _kernel;
		}
		
		public function get componentModel():ComponentModel
		{
			return _componentModel;
		}
		
		public virtual function resolve():Object
		{
			return componentModel.componentActivator.createComponent(this);
		}
		
		public virtual function resolveDependency(dependencyModel:DependencyModel):Object
		{
			// TODO: Detect circularity and handle configuration parameters (dependencies that are not services).
			return _kernel.resolve(dependencyModel.dependencyType);
		}
	}
}