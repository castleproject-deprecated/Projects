package castle.flexbridge.controls
{
	import mx.containers.TitleWindow;
	import mx.core.UIComponent;
	import flash.display.DisplayObjectContainer;
	import mx.core.Application;
	import mx.managers.PopUpManager;
	import mx.events.CloseEvent;
	import flash.events.IEventDispatcher;
	import mx.core.IUIComponent;
	import flash.events.Event;
	import flash.geom.Point;
	import flash.geom.Rectangle;
	import mx.events.MoveEvent;
	import mx.events.ResizeEvent;
	import mx.core.Container;

	/**
	 * The event dispatched when the dialog is opened.
	 */
	[Event(name="dialogOpen", type="castle.flexbridge.controls.DialogEvent")]

	/**
	 * The event dispatched when the dialog is closed.
	 */
	[Event(name="dialogClose", type="castle.flexbridge.controls.DialogEvent")]
	
	/**
	 * The event dispatched when the dialog's close button is
	 * clicked.  The dialog itself is not closed by this action.
	 * @see The dialogClose event.
	 */
	[Event(name="close", type="mx.events.CloseEvent")]

	/**
	 * A Dialog is a SizeableTitleWindow with provisions for popping it up
	 * and centering it within the application.  The initial size of the
	 * Dialog can be specified relative to the Stage using the initialPercentMinWidth
	 * and initialPercentMinHeight properties.
	 */
	public class Dialog extends SizeableTitleWindow
	{
		private static const MINIMUM_MARGIN_WIDTH:int = 20;
		private static const MINIMUM_MARGIN_HEIGHT:int = 20;
				
		private var _isOpen:Boolean = false;
		private var _modal:Boolean = false;
		private var _centered:Boolean = true;
		private var _result:String = null;
		private var _initialPercentMinWidth:Number = 0.0;
		private var _initialPercentMinHeight:Number = 0.0;
		
		/**
		 * Gets or sets whether dialog is modal when it is opened.
		 * The default is <code>false</code>.
		 */
		public function get modal():Boolean
		{
			return _modal;
		}
		
		public function set modal(value:Boolean):void
		{
			_modal = value;
		}
		
		/**
		 * Gets or sets whether the dialog should be centered above its owner component
		 * when it is opened.
		 * The default is <code>true</code>.
		 */
		public function get centered():Boolean
		{
			return _centered;
		}
		
		public function set centered(value:Boolean):void
		{
			_centered = value;
		}
		
		/**
		 * Gets or sets the initial minimum width of the Dialog as a percentage of the 
		 * width of the Stage.
		 */
		public function get initialPercentMinWidth():Number
		{
			return _initialPercentMinWidth;
		}
		
		public function set initialPercentMinWidth(value:Number):void
		{
			_initialPercentMinWidth = value;
		}

		/**
		 * Gets or sets the initial minimum height of the Dialog as a percentage of the 
		 * height of the Stage.
		 */
		public function get initialPercentMinHeight():Number
		{
			return _initialPercentMinHeight;
		}
		
		public function set initialPercentMinHeight(value:Number):void
		{
			_initialPercentMinHeight = value;
		}
		
		/**
		 * Returns true if the dialog is open.
		 */
		[Bindable(event="isOpenChange")]
		public function get isOpen():Boolean
		{
			return _isOpen;
		}
		
		/**
		 * Gets or sets the dialog result.
		 * The default value is null.
		 * @see DialogResult for standard codes.
		 */
		[Bindable(event="resultChange")]
		public function get result():String
		{
			return _result;
		}
		
		public function set result(value:String):void
		{
			if (_result != value)
			{
				_result = value;
				
				dispatchEvent(new Event("resultChange"));
			}
		}
		
		/**
		 * Opens the dialog, showing the popup on the screen if not already open.
		 * If the popup should open and center above a component other than
		 * the Application, set the owner property before calling this method.
		 * 
		 * Dispatches a <code>DialogEvent.DIALOG_OPEN</code> event.
		 */
		public function open():void
		{
			if (! _isOpen)
			{
				if (! owner)
					owner = Application(Application.application);

				PopUpManager.addPopUp(this, owner, _modal);

				var newWidth:Number = getExplicitOrMeasuredWidth();
				var newHeight:Number = getExplicitOrMeasuredHeight();

				if (_initialPercentMinWidth > 0)
					newWidth = Math.max(newWidth, _initialPercentMinWidth * stage.stageWidth / 100.0);
				
				if (_initialPercentMinHeight > 0)
					newHeight = Math.max(newHeight, _initialPercentMinHeight * stage.stageHeight / 100.0);

				setSuggestedActualSize(newWidth, newHeight);

				if (_centered)
					PopUpManager.centerPopUp(this);

				_isOpen = true;
								
				dispatchEvent(new Event("isOpenChange"));
				dispatchEvent(new DialogEvent(DialogEvent.DIALOG_OPEN));
			}
		}
		
		/**
		 * Closes the dialog, removing the popup from the screen if not already closed.
		 * 
		 * Dispatches a <code>DialogEvent.DIALOG_CLOSE</code> event containing the result.
		 * 
		 * @param result The dialog result, or null if none.
		 */
		public function close(result:String = null):void
		{
			if (_isOpen)
			{
				PopUpManager.removePopUp(this);
				
				_isOpen = false;
				this.result = result;
				
				dispatchEvent(new DialogEvent(DialogEvent.DIALOG_CLOSE, false, false, result));
				dispatchEvent(new Event("isOpenChange"));
			}
		}
		
		/**
		 * Brings the dialog to the front and gives it focus.
		 */
		public function activate():void
		{
			if (_isOpen)
			{
				PopUpManager.bringToFront(this);
				setFocus();
			}
		}
	}
}