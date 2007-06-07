package Castle.FlexBridge.Flash.Common
{
	/**
	 * An interface implemented by components whose initialization
	 * must be finalized by calling initialize() after all of the
	 * components properties have been set.
	 */
	public interface IInitializable
	{
		/**
		 * Initializes the component.
		 */
		function initialize():void;
	}
}