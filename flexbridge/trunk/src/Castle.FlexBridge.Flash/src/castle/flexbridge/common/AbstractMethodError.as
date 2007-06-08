package castle.flexbridge.common
{
	/**
	 * Error thrown by an abstract method to indicate that it
	 * has not been overridden as it should have been.
	 */
	public class AbstractMethodError extends Error
	{
		/**
		 * Constructs an abstract method error for the specified method.
		 * @param methodName The name of the abstract method.
		 */
		public function AbstractMethodError(methodName : String)
		{
			super("Method '" + methodName + "' is abstract.");
		}
	}
}