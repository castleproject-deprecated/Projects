using System;
using System.Collections.Generic;
using System.Text;

namespace Castle.MonoRail.ViewComponents
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Castle.MonoRail.Framework;

	/// <summary>
	/// ViewComponent which allows a user to more easily enter fixed width 
	/// input where you would like them to enter the data in a certain format
	/// (dates, phone numbers, etc). 
	/// </summary>
	/// <remarks>
	/// It has been tested on Internet Explorer 6/7,
	/// Firefox 1.5/2, Safari, and Opera.                              <para />
	/// 
	///  A mask is defined by a format made up of mask and placeholder characters. 
	///  Any character not in the placeholder character list below is considered 
	///  a mask character. Mask characters will be automatically entered for the 
	///  user as they type and will not be able to be removed by the user.
	///  
	///  The following placeholder characters are predefined:
	/// <list type="bullet">
	/// <item>
	/// <description>       a - Represents an alpha character (A-Z,a-z)     </description>
	/// </item>
	/// <item>
	/// <description>       9 - Represents a numeric character (0-9)     </description>
	/// </item>
	/// <item>
	/// <description>       * - Represents an alphanumeric character (A-Z, a-z, 0-9)     </description>
	/// </item>
	/// </list>
	/// 
	/// <list type="table">
	/// <listheader>
	/// <term>Sections</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term>label</term>
	/// <description>defaults to <c>&lt;label for='{id}' style='padding-left:0.4em; padding-right:1em;'>{Label parameter}&lt;/label></c></description>
	/// </item>
	/// <item>
	/// <term>preTextbox</term>
	/// <description>Text render between label and text box</description>
	/// </item>
	/// <item>
	/// <term>insideTextbox</term>
	/// <description>attributes to be added in text box tag.</description>
	/// </item>
	/// <item>
	/// <term>postTextbox</term>
	/// <description></description>
	/// </item>
	/// <item>
	/// <term>validateError</term>
	/// <description></description>
	/// </item>
	/// 
	/// </list><para/>
	/// 
	/// Requires the <c>MaskTextbox.js</c> javascript file, which it expects in the ~\Javascript
	/// folder.                                                   <para/>
	/// 
	/// Based on "Masked Input Plugin" by Josh Bush.
	/// http://digitalbush.com/projects/masked-input-plugin
	/// 
	/// Adapted by Shawn Carr.
	/// </remarks>
	/// <example>
	/// Usage of MaskedTextbox (in brail): 
	/// <code><![CDATA[
	/// <?brail component MaskedTextbox, { 
	///      @Mask : "999-99-9999", @Id : "SSN", @Label : "SSN" } ?> 
	/// <br/> 
	/// ]]></code>
	/// </example>

	[ViewComponentDetails("MaskedTextbox", Sections="label,preTextbox,insideTextbox,postTextbox,validateError")]
    public class MaskedTextboxComponent : ViewComponentEx
    {
        // Fields
        private static readonly string _allowedCharacterOutputString = "A, a, *, 9, @, $, %, (, ), -, {, }, [, ], /, ;, :, ', \", , (comma), ., /, _, =, +, and lastly (space)";
		private string _allowedCharacters = string.Empty;
        private string _mask = string.Empty;
        private string _id = string.Empty;
        private string _label = string.Empty;
        private JavascriptHelper _javascriptHelper;

        // Properties
		/// <summary>
		/// Gets or sets the mask.
		/// </summary>
		/// <value>The mask.</value>
        public string Mask
        {
            get { return _mask; }
			set
			{
				foreach (char ch in value)
				{
					if (AllowedCharacters.IndexOf(ch) == -1)
						throw new ArgumentException("Mask for MaskedTextbox can only contain these characters: '" + _allowedCharacterOutputString + "'. Your Mask has a '" + ch + "'");
				}
				_mask = value;
			}
        }

		/// <summary>
		/// Gets or sets the id of the text box element.
		/// </summary>
		/// <value>The id.</value>
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

		/// <summary>
		/// Gets or sets the text for the label for the text element.
		/// </summary>
		/// <value>The label.</value>
        public string Label
        {
            get { return _label; }
            set { _label = value; }
        }

		/// <summary>
		/// Gets or sets the allowed characters.
		/// </summary>
		/// <value>The allowed characters.</value>
        public string  AllowedCharacters
        {
            get { return _allowedCharacters; }
            set { _allowedCharacters = value; }
        }


		/// <summary>
		/// Called by the framework so the component can
		/// render its content
		/// </summary>
        public override void Render()
        {
            AllowedCharacters= "Aa*9@$%()/-{}[];:'\",./_=+ ";

            GetParameters();

            RenderJSFiles();
            RenderLabel();
			RenderOptionalSection("preTextbox");
			RenderTextboxStart();
			RenderOptionalSection("insideTextbox");
			RenderTextboxEnd();
			RenderOptionalSection("postTextbox");
			RenderValidationError();

            //there has to be a better way than this
            RenderText(string.Format(CultureInfo.InvariantCulture, "<script type=\"text/javascript\">$('{0}').mask(\"{1}\");</script>", Id, FixupOutput(Mask)));
        }

        private void GetParameters()
        {
            Mask = ComponentParams["Mask"] as string;
            Id = ComponentParams["Id"] as string;
            Label = ComponentParams["Label"] as string;
        }

        private void RenderJSFiles()
        {
            _javascriptHelper = new JavascriptHelper(Context, HttpContext, Flash, "MaskedTextboxComponent");
            _javascriptHelper.IncludeScriptFile("MaskedTextbox.js");
        }

        internal static string FixupOutput(string value)
        {
            return value.Replace(@"\", @"\\").Replace("'", @"\'").Replace("\"", "\\\"");
        }

        private void RenderLabel()
        {
			if (!RenderOptionalSection("label"))
				if (!string.IsNullOrEmpty(Label))
				{
					RenderText(string.Format("<label for='{0}' style='{1}'>{2}</label>", Id, "padding-left:0.4em; padding-right:1em;", Label));
				}
        }

        private void RenderTextboxStart()
        {
            RenderText(string.Format("<input type=\"text\" id=\"{0}\" name=\"{0}\" ", Id));
        }

        private void RenderTextboxEnd()
        {
            RenderText("/>");
        }

        private void RenderValidationError()
        {
			if (!RenderOptionalSection("validateError"))
                RenderText(Mask);
        }
    }
}
