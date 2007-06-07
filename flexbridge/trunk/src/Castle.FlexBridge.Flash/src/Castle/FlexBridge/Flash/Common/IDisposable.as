package Castle.FlexBridge.Flash.Common
{
	/**
	 * Implemented by classes that require explicit disposal as
	 * part of their lifecycle to ensure that active processes
	 * terminate and that resources are released in a timely fashion.
	 */
	public interface IDisposable
	{
		/**
		 * Disposes of the object.
		 */
		function dispose():void
	}
}