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
	import flash.utils.describeType;
    import castle.flexbridge.reflection.*;
	
	/**
	 * Builds a component model using reflection on the constructor arguments
	 * of the component type.
	 */
	public class DefaultComponentModelBuilder implements IComponentModelBuilder
	{
		public function buildModel(componentKey:String, serviceType:Class, componentType:Class):ComponentModel
		{
			var model:ComponentModel = new ComponentModel(componentKey, serviceType);
			model.componentActivator = new DefaultComponentActivator(componentType);
			
			var classInfo:ClassInfo = ReflectionUtils.getClassInfo(componentType);
			if (classInfo.isInterface)
				throw new KernelError("Cannot build model for type " + classInfo.name + " because it is not a class.");
			
			var constructorParameters:Array = classInfo.constructor.parameters;
			model.constructorDependencies = constructorParameters.map(
				function(constructorParameter:ParameterInfo, index:int, array:Array):DependencyModel
				{
					return new DependencyModel("constructor-parameter-" + constructorParameter.index,
						constructorParameter.type, constructorParameter.isOptional);
				});
			
			return model;
		}
	}
}