package castle.flexbridge.common
{
	/**
	 * Error thrown to indicate that a given method is not supported.
	 */
	public class NotSupportedError extends Error
	{
		/**
		 * Constructs a NotSupportedError with the specified message.
		 * @param message The message.
		 */
		public function NotSupportedError(message:String = "")
		{
			super(message);
		}
	}
}