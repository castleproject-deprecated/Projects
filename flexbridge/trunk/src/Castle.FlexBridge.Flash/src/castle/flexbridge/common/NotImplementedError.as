package castle.flexbridge.common
{
	/**
	 * Error thrown by an method to indicate that it
	 * has not been implemented yet.
	 */
	public class NotImplementedError extends Error
	{
		/**
		 * Constructs an not implemented error for the specified method.
		 * @param methodName The name of the method.
		 */
		public function NotImplementedError(methodName : String)
		{
			super("Method '" + methodName + "' has not been implemented.");
		}
	}
}