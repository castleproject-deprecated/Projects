package castle.flexbridge.controls
{
	import mx.controls.TextArea;
	import flash.display.DisplayObject;
	import castle.flexbridge.controls.factories.IComponentFactory;
	import castle.flexbridge.controls.activeTextClasses.ActiveTextFieldMixin;
	import castle.flexbridge.controls.activeTextClasses.EmbeddedComponentInfo;
	
	[Event(name="linkClick", type="castle.flexbridge.controls.ActiveTextEvent")]
	[Event(name="linkRollOver", type="castle.flexbridge.controls.ActiveTextEvent")]
	[Event(name="linkRollOut", type="castle.flexbridge.controls.ActiveTextEvent")]
	
	/**
	 * An ActiveTextArea control extends the basic TextArea control with support
	 * for clickable and hoverable links and for embedded components within
	 * the HTML-text.
	 * 
	 * @see castle.flexbridge.controls.activeTextClasses.ActiveTextFieldMixin
	 *   for documentation of these advanced features
	 * 
	 * TODO: Support editable fields.  The problem is that the text and htmlText
	 *       properties cannot be read back from the underlying text field.
	 *       Moreover when doing so we need to map back to our special tags.
	 */
	public class ActiveTextArea extends TextArea
	{
		private var _mixin:ActiveTextFieldMixin;
		
		/**
		 * Creates an active text area control.
		 */
		public function ActiveTextArea()
		{
			_mixin = new ActiveTextFieldMixin(this, commitText, commitHtmlText);
		}
		
		/**
		 * @inheritDoc
		 */
		protected override function createChildren():void
		{
			super.createChildren();
			
			_mixin.initialize(textField);
		}

		/**
		 * @inheritDoc
		 */
		public override function get text():String
		{
			return _mixin.text;
		}
		
		/**
		 * @inheritDoc
		 */
		public override function set text(value:String):void
		{
			_mixin.text = value;
		}
		
		/**
		 * @inheritDoc
		 */
		public override function get htmlText():String
		{
			return _mixin.htmlText;
		}
		
		/**
		 * @inheritDoc
		 */
		public override function set htmlText(value:String):void
		{
			_mixin.htmlText = value;
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
			return _mixin.embeddedComponentFactory;
		}
		
		public function set embeddedComponentFactory(value:IComponentFactory):void
		{
			_mixin.embeddedComponentFactory = value;
		}

		/**
		 * Gets information about the embedded component with the specified id.
		 * 
		 * @param id The id of the embedded component.
		 * @return The embedded component info or null if none with that id.
		 */
		public function getEmbeddedComponentInfo(id:String):EmbeddedComponentInfo
		{
			return _mixin.getEmbeddedComponentInfo(id);
		}

		/**
		 * @inheritDoc
		 */
		protected override function commitProperties():void
		{
			_mixin.beforeCommitProperties();
			
			super.commitProperties();
			
			_mixin.afterCommitProperties();
		}
		
		private function commitText(value:String):void
		{
			super.text = value;
		}
		
		private function commitHtmlText(value:String):void
		{
			super.htmlText = value;
		}
	}
}