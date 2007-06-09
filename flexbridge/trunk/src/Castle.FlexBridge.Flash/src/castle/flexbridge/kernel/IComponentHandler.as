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
	 * A component handler captures information about a registered component
	 * and specifies how resolution should take place.
	 */
	public interface IComponentHandler
	{
		/**
		 * Gets the kernel in which this handler is registered.
		 */
		function get kernel():IKernel;
		
		/**
		 * Gets the component model for the component managed by this handler.
		 */
		function get componentModel():ComponentModel;
		
		/**
		 * Resolves an instance of the component.
		 */
		function resolve():Object;
		
		/**
		 * Resolves the specified dependency of a component managed by this handler. 
		 * @param dependencyModel The dependency to resolve.
		 * @return The resolved dependency.
		 */
		function resolveDependency(dependencyModel:DependencyModel):Object;
	}
}