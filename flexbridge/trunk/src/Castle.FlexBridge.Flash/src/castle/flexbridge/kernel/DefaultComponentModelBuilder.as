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