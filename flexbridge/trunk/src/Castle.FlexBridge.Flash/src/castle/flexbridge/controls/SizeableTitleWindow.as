// Copyright 2007 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

package castle.flexbridge.controls
{
	import mx.core.mx_internal;
	use namespace mx_internal;

	import flash.events.MouseEvent;
	import flash.events.Event;
	import flash.system.System;
	import flash.geom.Rectangle;
	import flash.geom.Point;
	import flash.errors.IllegalOperationError;
	import mx.events.CloseEvent;
	import mx.controls.Button;
	import mx.containers.TitleWindow;
	import mx.managers.CursorManager;
	
	/**
	 * A resizeable variant of a TitleWindow.
	 * 
	 * Originally based on the SizeableTitleWindow class from
	 * the Adobe ColdFusion components library but since has been
	 * rewritten to fix numerous bugs and improve code clarity.
	 */
	public class SizeableTitleWindow extends TitleWindow
	{
		private const dragThreshold:int = 5;

		private const CURSOR_STYLE_NONE:int = -1;
		private const CURSOR_STYLE_NE:int   = 0;
		private const CURSOR_STYLE_N:int    = 1;
		private const CURSOR_STYLE_NW:int   = 2;
		private const CURSOR_STYLE_W:int    = 3;
		private const CURSOR_STYLE_SW:int   = 4;
		private const CURSOR_STYLE_S:int    = 5;
		private const CURSOR_STYLE_SE:int   = 6;
		private const CURSOR_STYLE_E:int    = 7;
		private const CURSOR_STYLE_MOVE:int = 8;
		
		[Embed(source="assets/Cursor_Symbol_NS.gif")]
		private static const CURSOR_SYMBOL_NS:Class;

		[Embed(source="assets/Cursor_Symbol_NESW.gif")]
		private static const CURSOR_SYMBOL_NESW:Class;
		
		[Embed(source="assets/Cursor_Symbol_WE.gif")]
		private static const CURSOR_SYMBOL_WE:Class;

		[Embed(source="assets/Cursor_Symbol_NWSE.gif")]
		private static const CURSOR_SYMBOL_NWSE:Class;
								
		[Embed(source="assets/Cursor_Symbol_All.gif")]
		private static const CURSOR_SYMBOL_ALL:Class;

		private var _showDragCursor:Boolean = false;
		private var _enableResize:Boolean = true;

		private var _isResizing:Boolean;
		private var _resizeCursorStyle:int;
		private var _resizeStartStageX:int;
		private var _resizeStartStageY:int;
		private var _resizeOriginalDimensions:Rectangle;
		
		private var _currentCursorStyle:int;
		private var _currentCursorID:int;
		
		/**
		 * Creates an initially closed pop-up window.
		 */
		public function SizeableTitleWindow()
		{
			_isResizing = false;
			_currentCursorID = CursorManager.NO_CURSOR;
			_currentCursorStyle = CURSOR_STYLE_NONE;
		}
		
		/**
		 * Gets or sets whether to show a special cursor when dragging
		 * the window by its title bar.
		 * The default is false.
		 */
		public function get showDragCursor():Boolean
		{
			return _showDragCursor;
		}
		
		public function set showDragCursor(value:Boolean):void
		{
			_showDragCursor = value;
		}
		
		/**
		 * Gets or sets whether resizing is enabled.
		 * The default is true.
		 */
		public function get enableResize():Boolean
		{
			return _enableResize;
		}
		
		public function set enableResize(value:Boolean):void
		{
			_enableResize = value;
		}
		
		override protected function createChildren():void
		{
			super.createChildren();

			// Listeners for updating the cursor prior to dragging or resizing.
			this.addEventListener(MouseEvent.MOUSE_MOVE, cursorMouseMoveHandler);			
			this.addEventListener(MouseEvent.MOUSE_OUT, cursorMouseOutHandler);

			// Listener for initiating the resize.
			this.addEventListener(MouseEvent.MOUSE_DOWN, resizeMouseDownHandler);
		}

		/**
		 * Sets the cursor based on the edge over which the mouse is positioned.
		 */
		private function cursorMouseMoveHandler(event:MouseEvent):void
		{
			if (_isResizing)
				return; // spurious
			
			var cursorStyle:int = computeCursorStyle(event.stageX, event.stageY);
			setCursorStyle(cursorStyle);
		}

		/**
		 * Clears the cursor when the mouse leaves the window.
		 */
		private function cursorMouseOutHandler(event:MouseEvent):void
		{
			if (_isResizing)
				return; // spurious
				
			clearCursor();
		}

		/**
		 * Checks whether sizing top border otherwise delegates the drag
		 * operation to the superclass as usual.
		 */
		override protected function startDragging(event:MouseEvent):void
		{
			if (_isResizing)
				return; // spurious
			
			var cursorStyle:int = computeCursorStyle(event.stageX, event.stageY);
			
			if (_enableResize && cursorStyle != CURSOR_STYLE_NONE && cursorStyle != CURSOR_STYLE_MOVE)
				startSizing(cursorStyle, event.stageX, event.stageY);
			else
				super.startDragging(event);
		}
		
		/**
		 * Checks whether mouse is positioned over a border and starts sizing if so.
		 */
		private function resizeMouseDownHandler(event:MouseEvent):void
		{
			if (_isResizing)
				return; // spurious

			var cursorStyle:int = computeCursorStyle(event.stageX, event.stageY);
			
			if (_enableResize && cursorStyle != CURSOR_STYLE_NONE && cursorStyle != CURSOR_STYLE_MOVE)
				startSizing(cursorStyle, event.stageX, event.stageY);			
		}

		/**
		 * Determines which sizing cursor to show based on the specified position.
		 */
		private function computeCursorStyle(stageX:Number, stageY:Number):int
		{
			var localPos:Point = globalToLocal(new Point(stageX, stageY));
			var localX:Number = localPos.x;
			var localY:Number = localPos.y;
			
			var inTopBorder:Boolean = localY >= 0 && localY < dragThreshold;
			var inBottomBorder:Boolean = localY >= height - dragThreshold && localY < height;
			var inLeftBorder:Boolean = localX >= 0 && localX < dragThreshold;
			var inRightBorder:Boolean = localX >= width - dragThreshold && localX < width;
			
			if (inTopBorder)
			{
				if (inLeftBorder)
					return CURSOR_STYLE_NW;
				else if (inRightBorder)
					return CURSOR_STYLE_NE;
				else
					return CURSOR_STYLE_N;
			}
			
			if (inBottomBorder)
			{
				if (inLeftBorder)
					return CURSOR_STYLE_SW;
				else if (inRightBorder)
					return CURSOR_STYLE_SE;
				else
					return CURSOR_STYLE_S;
			}
			
			if (inLeftBorder)
				return CURSOR_STYLE_W;
			
			if (inRightBorder)
				return CURSOR_STYLE_E;
				
			if (localY >= 0 && localY < getHeaderHeight())
			{
				// Within the title bar.
				// Do not permit movement when positioned over the close button.
				if (showCloseButton)
				{
					var closeButton:Button = mx_internal::closeButton;
					if (closeButton != null && closeButton.getBounds(this).contains(localX, localY))
						return CURSOR_STYLE_NONE;
				}
				
				return CURSOR_STYLE_MOVE;
			}

			return CURSOR_STYLE_NONE;
		}

		/**
		 * Starts a sizing operation with the specified cursor style and start positions.
		 */
		private function startSizing(cursorStyle:int, startStageX:Number, startStageY:Number):void
		{
			if (cursorStyle == CURSOR_STYLE_MOVE || cursorStyle == CURSOR_STYLE_NONE)
				throw new IllegalOperationError("Must specify a valid resizing cursor style.");
			
			_isResizing = true;
			_resizeCursorStyle = cursorStyle;
			_resizeOriginalDimensions = new Rectangle(x, y, width, height);
			_resizeStartStageX = startStageX;
			_resizeStartStageY = startStageY;
			
			setCursorStyle(cursorStyle);
			
			// Capture events for duration of resize.
			systemManager.addEventListener(MouseEvent.MOUSE_MOVE, systemManager_resizeMouseMoveHandler, true);
			systemManager.addEventListener(MouseEvent.MOUSE_UP, systemManager_resizeMouseUpHandler, true);
	
			stage.addEventListener(Event.DEACTIVATE, stage_resizeDeactivateHandler);
			stage.addEventListener(Event.MOUSE_LEAVE, stage_resizeMouseLeaveHandler);
		}

		/**
		 * Stops a sizing operation if one is in progress, otherwise does nothing.
		 */
		private function stopSizing():void
		{
			if (! _isResizing)
				return; // nothing to do
			
			_isResizing = false;
			
			clearCursor();
			
			// Unhook events.
			systemManager.removeEventListener(MouseEvent.MOUSE_MOVE, systemManager_resizeMouseMoveHandler, true);
			systemManager.removeEventListener(MouseEvent.MOUSE_UP, systemManager_resizeMouseUpHandler, true);
	
			stage.removeEventListener(Event.DEACTIVATE, stage_resizeDeactivateHandler);
			stage.removeEventListener(Event.MOUSE_LEAVE, stage_resizeMouseLeaveHandler);
		}
		
		/**
		 * Updates the current size if a resize is in progress, otherwise does nothing.
		 */
		private function applySizing(currentStageX:Number, currentStageY:Number):void
		{
			if (! _isResizing)
				return; // spurious
			
			var deltaX:Number = (currentStageX - _resizeStartStageX) * scaleX;
			var deltaY:Number = (currentStageY - _resizeStartStageY) * scaleY;
			var newDimensions:Rectangle = _resizeOriginalDimensions.clone();
			
			switch (_resizeCursorStyle)
			{
				case CURSOR_STYLE_E:
					newDimensions.right += deltaX;
					break;
					
				case CURSOR_STYLE_SE:
					newDimensions.right += deltaX;
					newDimensions.bottom += deltaY;
					break;
					
				case CURSOR_STYLE_S:
					newDimensions.bottom += deltaY;
					break;
					
				case CURSOR_STYLE_SW:
					newDimensions.left += deltaX;
					newDimensions.bottom += deltaY;
					break;
					
				case CURSOR_STYLE_W:
					newDimensions.left += deltaX;
					break;
					
				case CURSOR_STYLE_NW:
					newDimensions.left += deltaX;
					newDimensions.top += deltaY;
					break;
					
				case CURSOR_STYLE_N:
					newDimensions.top += deltaY;
					break;
					
				case CURSOR_STYLE_NE:
					newDimensions.right += deltaX;
					newDimensions.top += deltaY;
					break;
			}
			
			// Set the size.
			var actualWidth:Number = computeActualWidth(newDimensions.width);
			var actualHeight:Number = computeActualHeight(newDimensions.height);
			super.setActualSize(actualWidth, actualHeight);

			// Set the position and correct for any change in the width or height.
			if (newDimensions.width != actualWidth && newDimensions.x != _resizeOriginalDimensions.x)
				newDimensions.x += newDimensions.width - actualWidth;
				
			if (newDimensions.height != actualHeight && newDimensions.y != _resizeOriginalDimensions.y)
				newDimensions.y += newDimensions.height - actualHeight;
			
			move(newDimensions.x, newDimensions.y);
		}
		
		/**
		 * Sets the actual size of the window based on the specified suggestions.
		 * Clamps the size to what can actually be displayed onscreen.
		 */
		public function setSuggestedActualSize(suggestedWidth:Number, suggestedHeight:Number):void
		{
			super.setActualSize(computeActualWidth(suggestedWidth), computeActualHeight(suggestedHeight));
		}
		
		private function computeActualWidth(suggestedWidth:Number):Number
		{
			return Math.min(Math.max(suggestedWidth, minWidth, measuredMinWidth), maxWidth, stage.stageWidth * scaleX);
		}
		
		private function computeActualHeight(suggestedHeight:Number):Number
		{
			return Math.min(Math.max(suggestedHeight, minHeight, measuredMinHeight), maxHeight, stage.stageHeight * scaleY);
		}
		
		/**
		 * Ignore whatever we are told from the outside about actual sizes.
		 * We will control our own size.
		 */
		public override function setActualSize(w:Number, h:Number):void
		{
			// Ignore the input values.
			// We'll use the current size as a basis for determining the
			// new size.  This takes into account factors such as the measure
			// size increasing or the Stage size decreasing.
			
			setSuggestedActualSize(this.width, this.height);
		}
		
		/**
		 * Updates the current size based on 
		 */
		private function systemManager_resizeMouseMoveHandler(event:MouseEvent):void
		{
			applySizing(event.stageX, event.stageY);
		}
		
		/**
		 * Stops resizing when the mouse is released.
		 */
		private function systemManager_resizeMouseUpHandler(event:MouseEvent):void
		{
			stopSizing();
		}
	
		/**
		 * Stops resizing when the mouse leaves the Stage.
		 */
		private function stage_resizeMouseLeaveHandler(event:Event):void
		{
			stopSizing();
		}			

		/**
		 * Stops resizing when the Stage is deactivated.
		 * This can happen when focus is forcibly taken away from the Flash Player by the OS.
		 */
		private function stage_resizeDeactivateHandler(event:Event):void
		{
			stopSizing();
		}			
		
		/**
		 * Clears the current cursor.
		 */
		private function clearCursor():void
		{
			setCursorStyle(CURSOR_STYLE_NONE);
		}
		
		/**
		 * Sets the current cursor style.
		 */
		private function setCursorStyle(cursorStyle:int):void
		{
			if (_currentCursorStyle == cursorStyle)
				return;

			_currentCursorStyle = cursorStyle;
			
			// Remove the current cursor
			if (_currentCursorID != CursorManager.NO_CURSOR)
			{
				CursorManager.removeCursor(_currentCursorID);
				_currentCursorID = CursorManager.NO_CURSOR;
			}
			
			// Set the new cursor
			switch (cursorStyle) 
			{
				case CURSOR_STYLE_MOVE:
					if (_showDragCursor)
						_currentCursorID = CursorManager.setCursor(CURSOR_SYMBOL_ALL, 2, -10, -10);	
					break;
					
				case CURSOR_STYLE_E:
				case CURSOR_STYLE_W:
					_currentCursorID = CursorManager.setCursor(CURSOR_SYMBOL_WE, 2, -10, -11);     
					break;
					
				case CURSOR_STYLE_NW:
				case CURSOR_STYLE_SE:
					_currentCursorID = CursorManager.setCursor(CURSOR_SYMBOL_NWSE, 2, -11, -11);
					break;
					
				case CURSOR_STYLE_NE:
				case CURSOR_STYLE_SW:
					_currentCursorID = CursorManager.setCursor(CURSOR_SYMBOL_NESW, 2, -11, -10);
					break;
					
				case CURSOR_STYLE_N:
				case CURSOR_STYLE_S:
					_currentCursorID = CursorManager.setCursor(CURSOR_SYMBOL_NS, 2, -10, -10);
					break;
			}
		}	
	}
}