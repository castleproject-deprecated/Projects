package castle.flexbridge.kernel
{
    import castle.flexbridge.common.*;
    import castle.flexbridge.reflection.*;
	
	/**
	 * The default component activator creates an instance of a class
	 * and passes it the resolved dependencies.
	 */
	public class DefaultComponentActivator implements IComponentActivator
	{
		private var _componentType:Class;
		
		/**
		 * Creates a default component activator for the specified component type.
		 * 
		 * @param componentType The class of component to instantiate.
		 */
		public function DefaultComponentActivator(componentType:Class)
		{
			_componentType = componentType;
		}

		public function createComponent(componentHandler:IComponentHandler):Object
		{
			var componentModel:ComponentModel = componentHandler.componentModel;
			
			// Resolve constructor dependencies.
			var constructorArgs:Array = new Array();
			for each (var constructorDependency:DependencyModel in componentModel.constructorDependencies)
			{
				try
				{				
					var constructorArg:Object = componentHandler.resolveDependency(constructorDependency);
					constructorArgs.push(constructorArg);
				}
				catch (e:Error)
				{
					if (! constructorDependency.isOptional)
						throw new KernelError("Cannot create component '"
							+ componentModel.componentKey + "' because a constructor dependency could not be resolved: "
							+ e.message);
				}
			}
			
			// Create the instance.
			var componentInstance:Object = ReflectionUtils.createInstance(_componentType, constructorArgs);			
			
			// TODO: Resolve optional dependencies and handle cyclic dependencies.
			
			// Commission the component.
			var initializable:IInitializable = componentInstance as IInitializable;
			if (initializable != null)
				initializable.initialize();
				
			return componentInstance;
		}
		
		public function disposeComponent(componentInstance:Object):void
		{
			// Decommission the component.
			var disposable:IDisposable = componentInstance as IDisposable;
			if (disposable != null)
				disposable.dispose();			
		}
	}
}