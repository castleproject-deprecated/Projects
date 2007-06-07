package Castle.FlexBridge.Flash.Kernel
{
	/**
	 * Error thrown by the inversion of control container to indicate problems
	 * such as when a component cannot be resolved.
	 */
	public class KernelError extends Error
	{
		/**
		 * Creates a kernel error.
		 * @param message The message text.
		 */
		public function KernelError(message:String = "")
		{
			super(message);
		}
	}
}