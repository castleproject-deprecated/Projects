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
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Forms;
    using Castle.NVelocity;
    using Microsoft.VisualStudio.Package;

    public class NVelocityScanner : IScanner
    {
        private Scanner scanner = new Scanner();
        private List<Stack<ScannerState>> lineState = new List<Stack<ScannerState>>();
        private bool isScanningLine = false;

        public NVelocityScanner()
        {
            scanner.IsLineScanner = true;

            // Insert the state for the initial line number
            Stack<ScannerState> initialState = new Stack<ScannerState>();
            initialState.Push(ScannerState.Default);
            lineState.Add(initialState);
        }

        public void SetSource(string source, int offset)
        {
            scanner.SetSource(source);
        }

        public bool ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int /*state*/ prevLineNumber)
        {
            if (!isScanningLine)
            {
                scanner.RestoreState(lineState[prevLineNumber]);
                isScanningLine = true;
            }

            Token token;
            try
            {
                if (scanner.EOF)
                {
                    prevLineNumber = prevLineNumber + 1;
                    if (lineState.Count == prevLineNumber)
                        lineState.Add(scanner.RetrieveState());
                    else
                        lineState[prevLineNumber] = scanner.RetrieveState();

                    isScanningLine = false;
                    return false;
                }

                token = scanner.GetToken();
            }
            catch (ScannerError)
            {
                isScanningLine = false;
                return false;
            }

            if (token == null || token.Type == Castle.NVelocity.TokenType.Error)
                return false;

            NVelocityTokenColor color = NVelocityTokenColor.XmlText;

            switch (token.Type)
            {
                // +==========================+
                // |     NVelocity Tokens     |
                // +==========================+
                case Castle.NVelocity.TokenType.NVText:
                case Castle.NVelocity.TokenType.NVEscapeDirective:
                case Castle.NVelocity.TokenType.NVComma:
                case Castle.NVelocity.TokenType.NVDoubleDot:
                    color = NVelocityTokenColor.NVText;
                    break;
                case Castle.NVelocity.TokenType.NVTrue:
                case Castle.NVelocity.TokenType.NVFalse:
                case Castle.NVelocity.TokenType.NVIn:
                case Castle.NVelocity.TokenType.NVWith:
                    color = NVelocityTokenColor.NVKeyword;
                    break;
                case Castle.NVelocity.TokenType.NVSingleLineComment:
                case Castle.NVelocity.TokenType.NVMultilineCommentStart:
                case Castle.NVelocity.TokenType.NVMultilineCommentEnd:
                case Castle.NVelocity.TokenType.NVMultilineComment:
                    color = NVelocityTokenColor.NVComment;
                    break;
                case Castle.NVelocity.TokenType.NVDollar:
                case Castle.NVelocity.TokenType.NVIdentifier:
                case Castle.NVelocity.TokenType.NVReferenceLCurly:
                case Castle.NVelocity.TokenType.NVReferenceRCurly:
                case Castle.NVelocity.TokenType.NVReferenceSilent:
                case Castle.NVelocity.TokenType.NVDot:
                    color = NVelocityTokenColor.NVIdentifier;
                    break;
                case Castle.NVelocity.TokenType.NVStringLiteral:
                case Castle.NVelocity.TokenType.NVDoubleQuote:
                case Castle.NVelocity.TokenType.NVSingleQuote:
                    color = NVelocityTokenColor.NVString;
                    break;
                case Castle.NVelocity.TokenType.NVIntegerLiteral:
                //case Castle.NVelocity.TokenType.NVFloatingPoint:
                    color = NVelocityTokenColor.NVNumber;
                    break;
                case Castle.NVelocity.TokenType.NVDirectiveHash:
                case Castle.NVelocity.TokenType.NVDirectiveName:
                    color = NVelocityTokenColor.NVDirective;
                    break;
                case Castle.NVelocity.TokenType.NVEq:
                case Castle.NVelocity.TokenType.NVLte:
                case Castle.NVelocity.TokenType.NVLt:
                case Castle.NVelocity.TokenType.NVGt:
                case Castle.NVelocity.TokenType.NVGte:
                case Castle.NVelocity.TokenType.NVEqEq:
                case Castle.NVelocity.TokenType.NVNeq:
                case Castle.NVelocity.TokenType.NVPlus:
                case Castle.NVelocity.TokenType.NVMinus:
                case Castle.NVelocity.TokenType.NVMul:
                case Castle.NVelocity.TokenType.NVDiv:
                case Castle.NVelocity.TokenType.NVMod:
                case Castle.NVelocity.TokenType.NVAnd:
                case Castle.NVelocity.TokenType.NVOr:
                case Castle.NVelocity.TokenType.NVNot:
                    color = NVelocityTokenColor.NVOperator;
                    break;
                case Castle.NVelocity.TokenType.NVLParen:
                case Castle.NVelocity.TokenType.NVRParen:
                case Castle.NVelocity.TokenType.NVLBrack:
                case Castle.NVelocity.TokenType.NVRBrack:
                case Castle.NVelocity.TokenType.NVLCurly:
                case Castle.NVelocity.TokenType.NVRCurly:
                    color = NVelocityTokenColor.NVBracket;
                    break;
                case Castle.NVelocity.TokenType.NVDictionaryPercent:
                case Castle.NVelocity.TokenType.NVDictionaryLCurly:
                case Castle.NVelocity.TokenType.NVDictionaryRCurly:
                    color = NVelocityTokenColor.NVDictionaryDelimiter;
                    break;
                case Castle.NVelocity.TokenType.NVDictionaryKey:
                    color = NVelocityTokenColor.NVDictionaryKey;
                    break;
                case Castle.NVelocity.TokenType.NVDictionaryEquals:
                    color = NVelocityTokenColor.NVDictionaryEquals;
                    break;

                // +====================+
                // |     XML Tokens     |
                // +====================+
                case Castle.NVelocity.TokenType.XmlText:
                    color = NVelocityTokenColor.XmlText;
                    break;
                case Castle.NVelocity.TokenType.XmlComment:
                case Castle.NVelocity.TokenType.XmlCommentStart:
                case Castle.NVelocity.TokenType.XmlCommentEnd:
                    color = NVelocityTokenColor.XmlComment;
                    break;
                case Castle.NVelocity.TokenType.XmlTagName:
                    color = NVelocityTokenColor.XmlTagName;
                    break;
                case Castle.NVelocity.TokenType.XmlAttributeName:
                    color = NVelocityTokenColor.XmlAttributeName;
                    break;
                case Castle.NVelocity.TokenType.XmlAttributeText:
                    color = NVelocityTokenColor.XmlAttributeValue;
                    break;
                case Castle.NVelocity.TokenType.XmlTagStart:
                case Castle.NVelocity.TokenType.XmlTagEnd:
                    color = NVelocityTokenColor.XmlTagDelimiter;
                    break;
                case Castle.NVelocity.TokenType.XmlForwardSlash:
                case Castle.NVelocity.TokenType.XmlQuestionMark:
                case Castle.NVelocity.TokenType.XmlExclaimationMark:
                case Castle.NVelocity.TokenType.XmlEquals:
                case Castle.NVelocity.TokenType.XmlDoubleQuote:
                    color = NVelocityTokenColor.XmlOperator;
                    break;
                //case ???
                //    color = NVelocityTokenColor.XmlEntity;
                //    break;
                case Castle.NVelocity.TokenType.XmlCDataStart:
                case Castle.NVelocity.TokenType.XmlCDataEnd:
                //case Castle.NVelocity.TokenType.XmlCData:
                    color = NVelocityTokenColor.XmlCDataSection;
                    break;
                //case ???
                //    color = NVelocityTokenColor.XmlProcessingInstruction;
                //    break;
            }

            tokenInfo.Color = (TokenColor)color;

            tokenInfo.StartIndex = token.Position.StartPos - 1;
            tokenInfo.EndIndex = token.Position.EndPos - 2;

            return true;
        }
    }
}