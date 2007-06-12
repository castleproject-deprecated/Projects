package castle.flexbridge.controls.activeTextClasses
{
	import mx.core.UIComponent;
	import flash.errors.IllegalOperationError;
	import flash.events.Event;
	import flash.events.MouseEvent;
	import flash.events.TextEvent;
	import flash.display.DisplayObject;
	import flash.geom.Point;
	import flash.geom.Rectangle;
	import flash.text.Font;
	import flash.text.TextField;
	import flash.text.TextFormat;
	import flash.text.TextRun;
	import mx.core.ClassFactory;
	import mx.core.IFlexDisplayObject;
	import mx.core.UIComponentDescriptor;
	import mx.core.UITextField;
	import mx.events.ResizeEvent;
	import mx.events.MoveEvent;
	import mx.managers.ILayoutManagerClient;
	import mx.utils.ObjectUtil;
	import castle.flexbridge.common.HtmlUtils;
	import castle.flexbridge.controls.factories.IComponentFactory;
	import castle.flexbridge.controls.factories.TypedComponentFactory;
	import castle.flexbridge.controls.ActiveTextEvent;
	
	/**
	 * The ActiveTextFieldMixin implements common extensions to UITextField
	 * based controls with support for clickable and hoverable links and
	 * for embedded components within the HTML-text.
	 * 
	 * The mixin adds the following events:
	 * 
	 * 1. ActiveTextEvent.LINK_CLICK:
	 *    Clicking on a link whose href attribute begins with 'event:'.
	 *    <a href="event:clickMe">Click Me!</a>
	 * 
	 * 2. ActiveTextEvent.LINK_ROLL_OVER:
	 *    Rolling over a region containing any link.
	 * 
	 * 3. ActiveTextEvent.LINK_ROLL_OUT:
	 *    Rolling out a region containing any link.
	 * 
	 * The mixin also adds the following enhanced HTML tags:
	 * 
	 * 1. <embed id="optionalId" type="icon" attrib="value"... />
	 *    Embeds a component into the text inline.  Unlike the <img> tag text does
	 *    not flow around the sides of an embedded component.  Instead, sufficient
	 *    space for the width and height of the component are reserved within the
	 *    line of text in which <embed> appears.  This may cause text lines to become
	 *    very tall.
	 * 
	 *    type: (required)
	 *        The type of component to embed.key of a component that implements the IEmbeddedComponentFactory
	 *        that is used to create the embedded control.
	 * 
	 *    id: (optional)
	 *        The unique id for the embedded control for use with the
	 *        getEmbeddedControl() method.
	 * 
	 *    attrib...: (optional)
	 *        The text field ignores any other attributes but passes them to
	 *        the embedded component factory as a parameter during component
	 *        creation.  This provides factories with a straight-forward
	 *        mechanism to define custom attributes that control their operation.
	 * 
	 * TODO: Support editable fields.  The problem is that the text and htmlText
	 *       properties cannot be read back from the underlying text field.
	 *       Moreover when doing so we need to map back to our special tags.
	 * 
	 * FIXME: There seems to be a problem obtaining correct character cell
	 *        boundaries when the text field is nested some levels deep inside
	 *        a scrolling container.  When that happens getCharBoundaries()
	 *        returns a rectangle with a negative position relative to some
	 *        unknown coordinate system.  Consequently embedded controls
	 *        cannot be correctly positioned.
	 */
	public class ActiveTextFieldMixin
	{
		private static const EMBED_TAG_REGEXP:RegExp = /<embed(?P<attributes>(?:\s+\w+="[^"]*")*)\s*\/?>(?:<\/embed>)?/gis;
		private static const ATTRIBUTE_REGEXP:RegExp = /\s+(?P<attribName>\w+)=(?:"(?P<attribValue>[^"]*)")/gs;
		
		// The embedded font is used when the TextField's embedFonts is true to ensure
		// that we can obtain consistent metrics for a non-breaking space character
		// when embedding controls within the text field.
		[Embed(source="../assets/ActiveTextField.ttf", fontName="ActiveTextField", mimeType="application/x-font-truetype")]
		private static const ACTIVE_TEXT_FIELD_FONT:Class;
		private static const ACTIVE_TEXT_FIELD_FONT_NAME:String = "ActiveTextField";
		
		private static var _defaultDeviceFontNBSPWidthRatio:Number;
		private static var _defaultDeviceFontNBSPHeightRatio:Number;
		private static var _embeddedFontNBSPHeightRatio:Number;
		private static var _embeddedFontNBSPWidthRatio:Number;
		
		computeFontNBSPMetrics();
		
		private var _embeddedComponentFactory:IComponentFactory;
		
		private var _component:UIComponent;
		private var _textField:UITextField;
		
		private var _commitText:Function;
		private var _commitHtmlText:Function;
		private var _committedText:String = null;
		private var _committedHtmlText:String = null;

		private var _explicitHtmlText:String = null;
		private var _explicitText:String = null;
		
		private var _over:Boolean = false;
		private var _currentLinkUrl:String = null;
		
		/**
		 * We need to watch for changes in the embedFonts flag and update
		 * the htmlText because it can affect font metrics.  Unfortunately
		 * there does not seem to be a corresponding event available.
		 * The embedFonts flag may be toggled automatically by UITextField
		 * in response to discovering that the default format for the text
		 * field contains an embedded font, so it's not enough to intercept
		 * property writes to the primary controls.
		 */
		private var _savedEmbedFonts:Boolean;
		
		/**
		 * An array of HTML Strings and parsed InternalEmbeddedComponentInfo
		 * objects that represent the tokenized form of the html.  This is
		 * designed to make it easy to splice together the various contribution
		 * into a single HTML fragment.
		 * 
		 * The array is null if _explicitHtmlText is null.
		 */
		private var _tokenizedHtmlText:Array = null;
		
		/**
		 * Set to true when the text properties of the underlying
		 * text field need to be updated.
		 */
		private var _textPropertiesChanged:Boolean = false;
		
		/**
		 * A dictionary of embedded components referenced by ID.
		 */
		private var _embeddedComponents:Object = new Object(); /* dictionary of String to EmbeddedComponentInfo */
		private var _embeddedComponentsByInternalId:Object = new Object(); /* dictionary of int to EmbeddedComponentInfo */

		/**
		 * The auto-incrementing part of the next generated component id.
		 * We set the MSB so that this number can always be formatted to a
		 * valid 6 digit hexadecimal value.
		 * 
		 * TODO: Handle overflow after ~8 million iterations...
		 */
		private var _nextId:int = 0x800000;
		
		/**
		 * Creates a shim attached to the specified text-field based component.
		 * 
		 * @param component The text-field based component to modify.
		 * @param commitText The function to call to commit the text property
		 *                   once transformations have been applied.
		 * @param commitHtmlText The function to call to commit the htmlText property
		 *                       once transformations have been applied.
		 */		
		public function ActiveTextFieldMixin(component:UIComponent, commitText:Function, commitHtmlText:Function)
		{
			_component = component;
			
			_commitText = commitText;
			_commitHtmlText = commitHtmlText;
			_embeddedComponentFactory = TypedComponentFactory.instance;
		}
		
		/**
		 * Registers the mixin with the given text field.
		 */
		public function initialize(textField:UITextField):void
		{
			_textField = textField;
			
			textField.addEventListener(Event.RENDER, textFieldRenderHandler);
			textField.addEventListener(TextEvent.LINK, textFieldLinkHandler);
			textField.addEventListener(MouseEvent.ROLL_OVER, textFieldRollOverHandler);
			textField.addEventListener(MouseEvent.ROLL_OUT, textFieldRollOutHandler);
			textField.addEventListener(MouseEvent.MOUSE_MOVE, textFieldMouseMoveHandler);
			
			updateTextProperties();
		}
		
		/**
		 * Gets the enhanced HTML text for the control.
		 * @return The enhanced HTML text.
		 */
		public function get htmlText():String
		{
			return _explicitHtmlText;
		}
		
		/**
		 * Sets the enhanced HTML text for the control.
		 * @param value The enhanced HTML text.
		 */
		public function set htmlText(value:String):void
		{
			if (_explicitHtmlText != value)
			{
				_explicitHtmlText = value;
				_explicitText = null;
				
				tokenizeHtmlText();
			}
		}

		/**
		 * Gets the text for the control.
		 * @return The text.
		 */
		public function get text():String
		{
			return _explicitText;
		}
		
		/**
		 * Sets the text for the control.
		 * @param value The text.
		 */
		public function set text(value:String):void
		{
			if (_explicitText != value)
			{
				_explicitText = value;
				_explicitHtmlText = null;
				
				tokenizeHtmlText();
			}
		}
		
		/**
		 * Gets or sets the factory to use for constructing embedded components.
		 * The embedded component factory contructs components embedded within
		 * the enhanced htmlText with the &lt;EMBED&gt; element.  If the factory
		 * is set to null, embedded components will not be rendered.
		 * 
		 * Initialized to TypedComponentFactory.instance by default.
		 * @see TypedComponentFactory
		 */
		public function get embeddedComponentFactory():IComponentFactory
		{
			return _embeddedComponentFactory;
		}
		
		public function set embeddedComponentFactory(value:IComponentFactory):void
		{
			_embeddedComponentFactory = value;
		}

		/**
		 * Gets the embedded component with the specified id.
		 * 
		 * @param id The id of the embedded component.
		 * @return The embedded component or null if none with that id.
		 */
		public function getEmbeddedComponentInfo(id:String):EmbeddedComponentInfo
		{
			return _embeddedComponents[id] as EmbeddedComponentInfo;
		}

		/**
		 * Commits property changes before the text field.
		 */
		public function beforeCommitProperties():void
		{
			if (! _textField)
				return;
				
			if (_textPropertiesChanged)
			{
				// Workaround for a bug in TextField
				// When changing htmlText, it appears that the TextField
				// sometimes disregards its defaultTextFormat and instead
				// applies the formatting of the first character of the old
				// htmlText.  It tends to happen only when lots of display list
				// updates are taking place suggesting a race condition is
				// at work.  As a workaround we clear the htmlText before
				// applying our changes.  It seems to work...  -- Jeff.
				_textField.htmlText = "";
			}
		}
		
		/**
		 * Commits property changes after the text field.
		 */
		public function afterCommitProperties():void
		{
			if (! _textField)
				return;
				
			if (_textPropertiesChanged)
			{
				_textPropertiesChanged = false;
				
				updateEmbeddedComponentCharIndices();
				updateEmbeddedComponentPositions();
			}
		}
		
		/**
		 * Breaks the HTML text down into tokens and updates the
		 * embedded component dictionary.  Then updates the text properties.
		 */
		private function tokenizeHtmlText():void
		{
			var info:InternalEmbeddedComponentInfo;
			var oldEmbeddedComponents:Object = _embeddedComponents;
			
			_embeddedComponents = new Object();
			_embeddedComponentsByInternalId = new Object();
			
			try
			{
				if (! _explicitHtmlText)
				{
					_tokenizedHtmlText = null;
				}
				else
				{
					// Parse tags.
					_tokenizedHtmlText = [ ];
					
					var lastIndex:int = 0;
					EMBED_TAG_REGEXP.lastIndex = 0;
					for (;;)
					{
						var embedTagMatch:Object = EMBED_TAG_REGEXP.exec(_explicitHtmlText);
						if (! embedTagMatch)
							break;
							
						// Add prefix to tokenized representation.
						var index:int = int(embedTagMatch.index);
						if (index != lastIndex)
						{
							_tokenizedHtmlText.push(_explicitHtmlText.substring(lastIndex, index));
							lastIndex = EMBED_TAG_REGEXP.lastIndex;
						}
						
						// Parse attributes.
						var attributesGroup:String = embedTagMatch.attributes;
						var attributes:Object = new Object();
						
						ATTRIBUTE_REGEXP.lastIndex = 0;
						for (;;)
						{
							var attributeMatch:Object = ATTRIBUTE_REGEXP.exec(attributesGroup);
							if (! attributeMatch)
								break;
								
							var attribName:String = attributeMatch.attribName;
							var attribValue:String = HtmlUtils.htmlDecode(attributeMatch.attribValue);
							
							if (attribName in attributes)
								throw new IllegalOperationError("Embed tag contains duplicate values for same attribute: " + embedTagMatch.input);
							
							attributes[attribName] = attribValue;
						}
						
						// Build the component info.
						var id:String = attributes["id"];
						if (! id)
						{
							id = "EMBED$$" + _nextId;
							attributes["id"] = id;
						}
						
						if (id in _embeddedComponents)
							throw new IllegalOperationError("Embed tag has same Id as previous seen tag: " + embedTagMatch.input);
						
						info = oldEmbeddedComponents[id] as InternalEmbeddedComponentInfo;
						if (! info || ObjectUtil.compare(attributes, info.attributes) != 0)
						{
							info = new InternalEmbeddedComponentInfo();
							info.id = id;
							info.attributes = attributes;
							info.internalId = _nextId++;
							
							info.component = createEmbeddedComponent(attributes);
							
							addEmbeddedComponent(info.component);
						}
						else
						{
							info.charIndex = -1;
						}
							
						// Add the component to the tokenized representation.
						_embeddedComponents[id] = info;
						_embeddedComponentsByInternalId[info.internalId] = info;
						_tokenizedHtmlText.push(info);
					}
					
					// Finish up building the tokenized HTML string.
					_tokenizedHtmlText.push(_explicitHtmlText.substr(lastIndex));
				}
			}
			finally
			{
				// Remove any embedded components not recycled.
				for each (info in oldEmbeddedComponents)
				{
					removeEmbeddedComponent(info.component);
				}
				
				updateTextProperties();
			}
		}
		
		/**
		 * Applies changes to the underlying control's text and htmlText
		 * properties.  If using HTML, transforms the tokenized HTML into
		 * basic Flash HTML.
		 * 
		 * The following:
		 *   &lt;embed id="someControl" ... /&gt;
		 * 
		 * Becomes:
		 *   &lt;font face="ActiveTextField" color="##INTERNAL_ID##"
		 *         size="##HEIGHT##" letterSpacing="##WIDTH##"&gt;&amp;nbsp;&lt;/font&gt;
		 */
		private function updateTextProperties():void
		{
			if (! _textField)
				return; // not initialized yet

			if (! _tokenizedHtmlText)
			{
				if (_committedText != _explicitText)
				{		
					_commitText(_explicitText);
					_committedText = _explicitText;
					_committedHtmlText = null;
					
					_textPropertiesChanged = true;
					invalidateProperties();
				}
			}
			else
			{
				// Rememeber the value of embedFonts so we can watch for changes.
				_savedEmbedFonts = _textField.embedFonts;
				
				var nbspHeightRatio:Number, nbspWidthRatio:Number;
				if (_savedEmbedFonts)
				{
					nbspHeightRatio = _embeddedFontNBSPHeightRatio;
					nbspWidthRatio = _embeddedFontNBSPWidthRatio;
				}
				else
				{
					nbspHeightRatio = _defaultDeviceFontNBSPHeightRatio;
					nbspWidthRatio = _defaultDeviceFontNBSPWidthRatio;
				}
	
				// Transform the HTML text.
				var transformedHtmlText:String = "";
				
				for each (var token:Object in _tokenizedHtmlText)
				{
					if (token is String)
					{
						transformedHtmlText += token;
					}
					else
					{
						var info:InternalEmbeddedComponentInfo = InternalEmbeddedComponentInfo(token);
						var component:DisplayObject = info.component;
						var width:Number, height:Number;
						
						// Measure the control and set its size.
						if (component is ILayoutManagerClient)
							ILayoutManagerClient(component).validateSize(true);
	
						var uiComponent:UIComponent = component as UIComponent;
						if (uiComponent)
						{
							width = uiComponent.width = uiComponent.getExplicitOrMeasuredWidth();
							height = uiComponent.height = uiComponent.getExplicitOrMeasuredHeight();
						}
						else if (component)
						{
							width = component.width;
							height = component.height;
						}
						else
						{
							width = 0;
							height = 0;
						}
						
						// We adjust the font size to meet height requirements then use
						// letter spacing to reserve the width we need less the width of
						// an nbsp at that size.
						var size:int = Math.max(Math.ceil(height / nbspHeightRatio), 1);
						var letterSpacing:int = Math.ceil(width - size * nbspWidthRatio);
						
						transformedHtmlText += '<font face="' + ACTIVE_TEXT_FIELD_FONT_NAME + '" color="#';
						transformedHtmlText += info.internalId.toString(16);
						transformedHtmlText += '" size="';
						transformedHtmlText += size;
						transformedHtmlText += '" letterSpacing="';
						transformedHtmlText += letterSpacing;
						transformedHtmlText += '">&nbsp;</font>';
						// Remark:
						//   &#8288; is a Unicode zero-width non-breaking space
						//   (aka. word joiner) but Flash doesn't seem to map it
						//   correctly and it may not be present in the default
						//   device font.  Moreover, Flash does not handle zero-width
						//   character cells properly and will yield incorrect measurements
						//   for them.  So we make do with a non-breaking space.
						//   It turns out that negative amounts of leading space are
						//   tolerated just fine.
					}
				}
				
				if (_committedHtmlText != transformedHtmlText)
				{		
					_commitHtmlText(transformedHtmlText);
					_committedHtmlText = transformedHtmlText;
					_committedText = null;
					
					_textPropertiesChanged = true;
					invalidateProperties();
				}
			}
		}
		
		/**
		 * Updates the char indices of embedded components.
		 */
		private function updateEmbeddedComponentCharIndices():void
		{
			// NOTE: It appears that TextRun is undocumented but it's
			//       exactly what we need here to be able to scan the text
			//       field efficiently.
			var runs:Array = _textField.getTextRuns();
			
			// Find the component character index if absent.
			for each (var run:TextRun in runs)
			{
				if (run.endIndex - run.beginIndex == 1)
				{
					var textFormat:TextFormat = run.textFormat;
					if (textFormat.font == ACTIVE_TEXT_FIELD_FONT_NAME)
					{
						var info:InternalEmbeddedComponentInfo = _embeddedComponentsByInternalId[textFormat.color] as InternalEmbeddedComponentInfo;
						if (info)
							info.charIndex = run.beginIndex;
					}
				}
			}
		}
		
		/**
		 * Updates the position of embedded components.
		 */
		private function updateEmbeddedComponentPositions():void
		{
			for each (var info:InternalEmbeddedComponentInfo in _embeddedComponents)
			{
				if (info.charIndex < 0 || ! info.component)
					continue;
				
				var a:Rectangle = _textField.getBounds(_textField);
				var b:Rectangle = _textField.getRect(_textField);
				var c:Rectangle = _textField.getBounds(_textField.stage);
				var d:Rectangle = _textField.getRect(_textField.stage);
				
				var rect:Rectangle = _textField.getCharBoundaries(info.charIndex);
				if (rect)
				{
					// Align the component with the bottom of the character cell
					// and center it horizontally within the space.
					info.component.x = rect.left + (rect.width - info.component.width) / 2;
					info.component.y = rect.bottom - info.component.height;
				}
			}
		}
		
		/**
		 * Creates an embedded component using the specified attributes.
		 * 
		 * @param attributes The attributes that appeared in the EMBED tag.
		 * @return The new component, or null if none.
		 */
		private function createEmbeddedComponent(attributes:Object):DisplayObject
		{
			if (_embeddedComponentFactory != null)
				return _embeddedComponentFactory.newInstance(attributes);
			
			return null;
		}
		
		/**
		 * Adds an embedded component to the display object hierarchy.
		 * 
		 * @param child The component to add.
		 */
		private function addEmbeddedComponent(child:DisplayObject):void
		{
			child.addEventListener(ResizeEvent.RESIZE, embeddedComponentResizeHandler);
			
			_component.addChild(child);
		}
		
		/**
		 * Removes the embedded component from the display object hierarchy.
		 * 
		 * @param child The component to remove.
		 */
		private function removeEmbeddedComponent(child:DisplayObject):void
		{
			_component.removeChild(child);
		}
		
		private function invalidateProperties():void
		{
			_component.invalidateProperties();
		}
		
		private function embeddedComponentResizeHandler(e:ResizeEvent):void
		{
			updateTextProperties();
		}
		
		private function textFieldRenderHandler(e:Event):void
		{
			// If the embedFonts property changed and we are displaying HTML
			// then we need to re-evaluate our text metrics.
			if (_tokenizedHtmlText && _savedEmbedFonts != _textField.embedFonts)
			{
				updateTextProperties();
			}
			
			// Don't bother with remaining updates until finished applying changes to text.
			if (_textPropertiesChanged)
				return;
			
			// Update component positions to match up with the text about to be drawn.
			updateEmbeddedComponentPositions();
		}
		
		private function textFieldRollOverHandler(e:MouseEvent):void
		{
			_over = true;
			reassessHover(e);
		}
		
		private function textFieldRollOutHandler(e:MouseEvent):void
		{
			_over = false;
			reassessHover(e);
		}
		
		private function textFieldMouseMoveHandler(e:MouseEvent):void
		{
			reassessHover(e);
		}
					
		private function textFieldLinkHandler(e:TextEvent):void
		{
			_component.dispatchEvent(new ActiveTextEvent(ActiveTextEvent.LINK_CLICK,
				false, false, "event:" + e.text));
		}

		
		private function reassessHover(e:MouseEvent):void
		{
			var newLinkUrl:String = null;
			
			if (_over)
			{
				newLinkUrl = getLinkUrlFromEvent(e);
			}
			
			if (newLinkUrl != _currentLinkUrl)
			{
				if (_currentLinkUrl != null)
				{
					var oldLinkUrl:String = _currentLinkUrl;
					_currentLinkUrl = null;
					
					_component.dispatchEvent(new ActiveTextEvent(ActiveTextEvent.LINK_ROLL_OUT,
						false, false, oldLinkUrl));
				}
				
				if (newLinkUrl != null)
				{
					_currentLinkUrl = newLinkUrl;					
					
					_component.dispatchEvent(new ActiveTextEvent(ActiveTextEvent.LINK_ROLL_OVER,
						false, false, newLinkUrl));
				}
			}
		}
		
		private function getLinkUrlFromEvent(e:MouseEvent):String
		{
			var charIndex:int = _textField.getCharIndexAtPoint(e.localX, e.localY);
			if (charIndex >= 0)
			{
				// The text field treats points below the last line of text
				// as if they occur within that line.  We check the dimensions
				// of the bounding box of the character cell under the mouse
				// to handle that case.
				var rect:Rectangle = _textField.getCharBoundaries(charIndex);
				if (rect)
				{
					// HACK: It's unclear to me why I need to perform an additional
					//       transformation on these components.  The bounding
					//       rectangle looks like it undergoes an erroneous double
					//       translation to the local coordinate space.  But perhaps
					//       that has something to do with scrolling?  -- Jeff.
					var y:int = e.localY - _textField.getBounds(_textField.stage).top;
					
					if (y >= rect.top && y <= rect.bottom)
					{
						var textFormat:TextFormat = _textField.getTextFormat(charIndex, charIndex + 1);
						return textFormat.url;
					}
				}
			}
			
			return null;
		}
		
		/**
		 * Text fields cannot mix embedded fonts with non-embedded fonts.
		 * So we measure text metrics both for the default device font
		 * and for our embedded font.
		 */
		private static function computeFontNBSPMetrics():void
		{
			// Use a large size to get good measurements
			// It seems the limit is 127 so we choose 120 because it has several
			// integer divisors so it is more likely to produce even fractions.
			// For Times New Roman and our custom embedded font, the width ratio
			// is exactly 1/4.
			const fontSize:int = 120;
			
			var textFormat:TextFormat = new TextFormat();
			textFormat.font = ACTIVE_TEXT_FIELD_FONT_NAME;
			textFormat.size = fontSize;
			
			var textField:TextField = new TextField();
			textField.defaultTextFormat = textFormat;
			textField.htmlText = "&nbsp;";
			
			// Metrics for default device font.
			textField.embedFonts = false;
			_defaultDeviceFontNBSPHeightRatio = textField.textHeight / fontSize;
			_defaultDeviceFontNBSPWidthRatio = textField.textWidth / fontSize;
			
			// Metrics for embedded font.
			textField.embedFonts = true;
			_embeddedFontNBSPHeightRatio = textField.textHeight / fontSize;
			_embeddedFontNBSPWidthRatio = textField.textWidth / fontSize;
		}
	}
}

import flash.display.DisplayObject;
import castle.flexbridge.controls.activeTextClasses.EmbeddedComponentInfo;

/**
 * Contains information to describe an embedded component.
 */
class InternalEmbeddedComponentInfo extends EmbeddedComponentInfo
{
	/**
	 * The internal code used to refer to the embedded object.
	 */
	public var internalId:int;
}