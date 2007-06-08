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