// Copyright 2007 Jonathon Rossi - http://www.jonorossi.com/
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

namespace Castle.VisualStudio.NVelocityLanguageService
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using Castle.NVelocity;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Package;
    using Microsoft.VisualStudio.TextManager.Interop;

    public enum NVelocityTokenColor
    {
        NVText = 1,
        NVKeyword,
        NVComment,
        NVIdentifier,
        NVString,
        NVNumber,
        NVDirective,
        NVOperator,
        NVBracket,
        NVDictionaryDelimiter,
        NVDictionaryKey,
        NVDictionaryEquals,

        XmlText,
        XmlComment,
        XmlTagName,
        XmlAttributeName,
        XmlAttributeValue,
        XmlTagDelimiter,
        XmlOperator,
        XmlEntity,
        XmlCDataSection//,
        //XmlProcessingInstruction
    }

    [Guid(NVelocityConstants.LanguageServiceGuidString)]
    public class NVelocityLanguage : LanguageService
    {
        private NVelocityScanner lineScanner;
        private LanguagePreferences preferences;

        private readonly ColorableItem[] _colorableItems;

        public NVelocityLanguage()
        {
            #region Colorable Items

            _colorableItems = new ColorableItem[] {
                // NVText
                new ColorableItem("NVelocity – Text", "NVelocity – Text", COLORINDEX.CI_SYSPLAINTEXT_FG, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // NVKeyword
                new ColorableItem("NVelocity – Keyword", "NVelocity – Keyword", COLORINDEX.CI_BLUE, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_BOLD),
                // NVComment
                new ColorableItem("NVelocity – Comment", "NVelocity – Comment", COLORINDEX.CI_DARKGREEN, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // NVIdentifier
                new ColorableItem("NVelocity – Identifier", "NVelocity – Identifier", COLORINDEX.CI_PURPLE, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // NVString
                new ColorableItem("NVelocity – String", "NVelocity – String", COLORINDEX.CI_MAROON, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // NVNumber
                new ColorableItem("NVelocity – Number", "NVelocity – Number", COLORINDEX.CI_SYSPLAINTEXT_FG, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // NVDirective
                new ColorableItem("NVelocity – Directive", "NVelocity – Directive", COLORINDEX.CI_BLUE, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_BOLD),
                // NVOperator
                new ColorableItem("NVelocity – Operator", "NVelocity – Operator", COLORINDEX.CI_SYSPLAINTEXT_FG, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // NVBracket
                new ColorableItem("NVelocity – Bracket", "NVelocity – Bracket", COLORINDEX.CI_SYSPLAINTEXT_FG, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // NVDictionaryDelimiter
                new ColorableItem("NVelocity – Dictionary Delimiter", "NVelocity – Dictionary Delimiter", COLORINDEX.CI_PURPLE, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // NVDictionaryKey
                new ColorableItem("NVelocity – Dictionary Key", "NVelocity – Dictionary Key", COLORINDEX.CI_RED, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // NVDictionaryEquals
                new ColorableItem("NVelocity – Dictionary Equals", "NVelocity – Dictionary Equals", COLORINDEX.CI_BLUE, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // XmlText
                new ColorableItem("NVelocity – XML Text", "NVelocity – XML Text", COLORINDEX.CI_SYSPLAINTEXT_FG, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // XmlComment
                new ColorableItem("NVelocity – XML Comment", "NVelocity – XML Comment", COLORINDEX.CI_DARKGREEN, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // XmlTagName
                new ColorableItem("NVelocity – XML Tag Name", "NVelocity – XML Tag Name", COLORINDEX.CI_MAROON, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // XmlAttributeName
                new ColorableItem("NVelocity – XML Attribute Name", "NVelocity – XML Attribute Name", COLORINDEX.CI_RED, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // XmlAttributeValue
                new ColorableItem("NVelocity – XML Attribute Value", "NVelocity – XML Attribute Value", COLORINDEX.CI_BLUE, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // XmlTagDelimiter
                new ColorableItem("NVelocity – XML Tag Delimiter", "NVelocity – XML Tag Delimiter", COLORINDEX.CI_BLUE, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // XmlOperator
                new ColorableItem("NVelocity – XML Operator", "NVelocity – XML Operator", COLORINDEX.CI_BLUE, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // XmlEntity
                new ColorableItem("NVelocity – XML Entity", "NVelocity – XML Entity", COLORINDEX.CI_RED, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // XmlCDataSection
                new ColorableItem("NVelocity – XML Text", "NVelocity – XML Text", COLORINDEX.CI_BLUE, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT)
                // XmlProcessingInstruction
                // ===== not done
            };
            #endregion
        }

        public override void Dispose()
        {
            try
            {
                // Dispose the preferences
                if (preferences != null)
                {
                    preferences.Dispose();
                    preferences = null;
                }

                // Dispose the scanner
                lineScanner = null;
            }
            finally
            {
                base.Dispose();
            }
        }

        public override LanguagePreferences GetLanguagePreferences()
        {
            if (preferences == null)
            {
                preferences = new LanguagePreferences(Site, typeof(NVelocityLanguage).GUID, "NVelocity");
                preferences.Init();
                preferences.LineNumbers = true;
                preferences.Apply();
            }
            return preferences;
        }

        public override Source CreateSource(IVsTextLines buffer)
        {
            return new Source(this, buffer, GetColorizer(buffer));
        }

        public override IScanner GetScanner(IVsTextLines buffer)
        {
            if (lineScanner == null)
                lineScanner = new NVelocityScanner();
            return lineScanner;
        }

        public override string Name
        {
            get { return "NVelocity"; }
        }

        public override void OnIdle(bool periodic)
        {
            Source src = GetSource(LastActiveTextView);
            if (src != null && src.LastParseTime == Int32.MaxValue)
            {
                src.LastParseTime = 0;
            }
            base.OnIdle(periodic);
        }

        public override AuthoringScope ParseSource(ParseRequest req)
        {
            if (req == null)
                throw new ArgumentNullException("req");

            NVelocityAuthoringScope scope = new NVelocityAuthoringScope();
            //Source source = GetSource(req.FileName);
            
            if (req.Reason == ParseReason.Check)
            {
                Scanner scanner = new Scanner();
                scanner.SetSource(req.Text);

                try
                {
                    while (!scanner.EOF)
                        scanner.GetToken();
                }
                catch (ScannerError se)
                {
                    TextSpan textSpan = new TextSpan();
                    textSpan.iStartLine = scanner.CurrentPos.StartLine - 1;
                    textSpan.iStartIndex = scanner.CurrentPos.StartPos;
                    textSpan.iEndLine = scanner.CurrentPos.EndLine - 1;
                    textSpan.iEndIndex = scanner.CurrentPos.EndPos;

                    req.Sink.AddError(req.FileName, se.Message, textSpan, Severity.Error);
                }
            }
            
            return scope;
        }

        public override int GetItemCount(out int count)
        {
            count = _colorableItems.Length;
            return VSConstants.S_OK;
        }

        public override int GetColorableItem(int index, out IVsColorableItem item)
        {
            if (index < 1 || index > _colorableItems.Length)
            {
                item = null;
                return VSConstants.S_FALSE;
            }
            item = _colorableItems[index - 1];
            return VSConstants.S_OK;
        }
    }
}
