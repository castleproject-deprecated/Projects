package castle.flexbridge.controls
{
	import mx.core.UIComponent;
	import mx.core.IFlexDisplayObject;
	import flash.display.DisplayObject;
	import flash.utils.getDefinitionByName;

	/**
	 * An icon is a simple control that just displays the specified icon.
	 * 
	 * It is useful when the icon is not being displayed by a parent container
	 * such as a Tree or Panel and when the icon may need to be updated dynamically
	 * making the use of the Image control unsuitable.
	 */
	public class Icon extends UIComponent
	{
		private var _icon:Object = null;
		private var _iconChanged:Boolean = true;
		
		private var _iconDisplayObject:IFlexDisplayObject;
		
		/**
		 * Creates an icon object label initially with no icon.
		 */
		public function Icon()
		{
		}
		
		/**
		 * Gets the class or class name of the icon to display, or null if none.
		 */
		[Bindable]
		public function get icon():Object
		{
			return _icon;
		}
		
		/**
		 * Sets the class or class name of the icon to display, or null if none.
		 */
		public function set icon(icon:Object):void
		{
			_icon = icon;
			_iconChanged = true;
			
			invalidateProperties();
		}
		
		protected override function commitProperties():void
		{
			if (_iconChanged)
			{
				_iconChanged = false;
				
				if (_iconDisplayObject != null)
				{
					removeChild(DisplayObject(_iconDisplayObject));
					_iconDisplayObject = null;
				}
				
				if (_icon != null)
				{
					var iconClass:Class = _icon as Class;
					if (iconClass == null)
					{
						var iconClassName:String = _icon as String;
						if (iconClassName != null)
						{
							iconClass = Class(getDefinitionByName(iconClassName));
						}
					}
					
					_iconDisplayObject = IFlexDisplayObject(new iconClass);
					addChild(DisplayObject(_iconDisplayObject));
				}
			
				invalidateSize();
				invalidateDisplayList();
			}
		}

		protected override function measure():void
		{
			super.measure();
			
			if (_iconDisplayObject == null)
			{
				measuredWidth = 16;
				measuredHeight = 16;
			}
			else
			{
				measuredHeight = _iconDisplayObject.measuredHeight;
				measuredWidth = _iconDisplayObject.measuredWidth;
			}
		}
		
		protected override function updateDisplayList(unscaledWidth:Number, unscaledHeight:Number):void
		{
			super.updateDisplayList(unscaledWidth, unscaledHeight);

			if (_iconDisplayObject != null)
			{
				_iconDisplayObject.x = 0;
				_iconDisplayObject.y = 0;
				_iconDisplayObject.width = unscaledWidth;
				_iconDisplayObject.height = unscaledHeight;
			}
		}
	}
}