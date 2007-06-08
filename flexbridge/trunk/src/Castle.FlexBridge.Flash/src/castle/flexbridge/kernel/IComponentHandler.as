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