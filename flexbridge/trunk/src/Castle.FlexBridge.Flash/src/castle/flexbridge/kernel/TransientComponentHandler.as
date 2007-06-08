package castle.flexbridge.kernel
{
	/**
	 * A handler for a component with transient lifestyle.
	 * A transient component is recreated each time it is resolved.
	 */
	public class TransientComponentHandler extends BaseComponentHandler
	{
		public function TransientComponentHandler(kernel:IKernel, componentModel:ComponentModel)
		{
			super(kernel, componentModel);
		}
	}
}