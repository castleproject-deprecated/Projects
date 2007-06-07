package Castle.FlexBridge.Flash.Kernel
{
	/**
	 * The component model builder is responsible for producing a
	 * ComponentModel for a component.
	 */
	public interface IComponentModelBuilder
	{
		/**
		 * Builds a component model for a component.
		 * @param componentKey The component key.
		 * @param serviceType The service type.
		 * @param componentType The component type.
		 * @return The component model that was built.
		 */
		function buildModel(componentKey:String, serviceType:Class, componentType:Class):ComponentModel;
	}
}