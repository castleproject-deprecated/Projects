package castle.flexbridge.kernel
{
	/**
	 * A handler for a component with a singleton lifestyle.
	 * A singleton is reused indefinitely once allocated.
	 */
	public final class SingletonComponentHandler extends BaseComponentHandler
	{
		private var _componentInstance:Object = null;
		
		public function SingletonComponentHandler(kernel:IKernel, componentModel:ComponentModel)
		{
			super(kernel, componentModel);
		}
		
		public override function resolve():Object
		{
			if (! _componentInstance)
				_componentInstance = super.resolve();

			return _componentInstance;
		}
	}
}