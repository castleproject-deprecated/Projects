using System;
using System.Collections.Generic;
using System.Text;

namespace Castle.MonoRail.ViewComponents
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Castle.MonoRail.Framework;

    public class MaskedTextboxComponent : ViewComponent
    {
        // Fields
        private static readonly string _allowedCharacterOutputString = "A, a, *, 9, @, $, %, (, ), -, {, }, [, ], /, ;, :, ', \", , (comma), ., /, _, =, +, and lastly (space)";
        private List<string> _allowedCharacters = new List<string>();
        private string _mask = string.Empty;
        private string _id = string.Empty;
        private string _label = string.Empty;
        private JavascriptHelper _javascriptHelper;

        private static readonly string[] sections = new string[] { "label", "preTextbox", "insideTextbox", "postTextbox", "validateError" };

        // Properties
        public string Mask
        {
            get { return _mask; }
            set
            {
                if (value.Length > 0)
                {
                    foreach (char ch in value)
                    {
                        if (!AllowedCharacters.Contains(ch.ToString()))
                            throw new ArgumentException("Mask for MaskedTextbox can only contain these characters: '" + _allowedCharacterOutputString + "'. Your Mask has a '" + ch + "'");
                    }
                }
                _mask = value;
            }
        }

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Label
        {
            get { return _label; }
            set { _label = value; }
        }

        public List<string> AllowedCharacters
        {
            get { return _allowedCharacters; }
            set { _allowedCharacters = value; }
        }

        public override bool SupportsSection(string name)
        {
            foreach (string section in sections)
            {
                if (section.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public override void Render()
        {
            AllowedCharacters.AddRange(new string[] { "A", "a", "*", "9", "@", "$", "%", "(", ")", "/", "-", "{", "}", "[", "]", ";", ":", "'", "\"", ",", ".", "/", "_", "=", "+", " " });

            GetParameters();

            RenderJSFiles();
            RenderLabel();
            RenderPreTextbox();
            RenderTextboxStart();
            RenderInsideTextbox();
            RenderTextboxEnd();
            RenderPostTextbox();
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
            if (Context.HasSection("label"))
            {
                RenderSection("label");
            }
            else if (!string.IsNullOrEmpty(Label))
            {
                RenderText(string.Format("<label for='{0}' style='{1}'>{2}</label>", Id, "padding-left:0.4em; padding-right:1em;", Label));
            }
        }

        private void RenderPreTextbox()
        {
            if (Context.HasSection("preTextbox"))
            {
                RenderSection("preTextbox");
            }
        }

        private void RenderTextboxStart()
        {
            RenderText(string.Format("<input type=\"text\" id=\"{0}\" name=\"{0}\" ", Id));
        }

        private void RenderInsideTextbox()
        {
            if (Context.HasSection("insideTextbox"))
            {
                RenderSection("insideTextbox");
            }
        }

        private void RenderTextboxEnd()
        {
            RenderText("/>");
        }

        private void RenderPostTextbox()
        {
            if (Context.HasSection("postTextbox"))
            {
                RenderSection("postTextbox");
            }
        }

        private void RenderValidationError()
        {
            if (Context.HasSection("validateError"))
            {
                RenderSection("validateError");
            }
            else
                RenderText(Mask);
        }
    }
}
