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

namespace Castle.NVelocity
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class Scanner
    {
        private static readonly char CR = '\r';
        private static readonly char LF = '\n';

        private Stack<ScannerState> state = new Stack<ScannerState>();

        private char ch;
        private string _source;
        private int pos = 1;
        private bool eof = false;
        private int lineNo = 1, linePos = 1;
        private ArrayList linestarts;
        private Position currPos = new Position(0, 0);
        private Position prevPos = new Position(0, 0);

        private readonly Dictionary<string, TokenType> nvKeywords = new Dictionary<string, TokenType>();

        private bool isLineScanner = false;

        public Scanner()
        {
            SetUpReservedWords();
        }

        public bool IsLineScanner
        {
            get { return isLineScanner; }
            set { isLineScanner = value; }
        }

        public void RestoreState(Stack<ScannerState> restoredState)
        {
            if (restoredState != null)
            {
                state = new Stack<ScannerState>(restoredState.ToArray());
            }
        }

        public Stack<ScannerState> RetrieveState()
        {
            return new Stack<ScannerState>(state.ToArray());
        }

        public void SetSource(string source)
        {
            if (!isLineScanner || state.Count == 0)
            {
                state.Clear();
                state.Push(ScannerState.Default);
            }

            ch = default(char);
            _source = source;
            pos = 1;
            eof = false;
            lineNo = 1;
            linePos = 1;
            linestarts = new ArrayList();

            if (source.Length == 0)
            {
                eof = true;
                return;
            }

            ch = source[0];
            linestarts.Add(0);
        }
    
        public Position CurrentPos
        {
            get { return currPos; }
        }

        public Position PreviousPos
        {
            get { return prevPos; }
        }

        public ScannerState CurrentState
        {
            get { return state.Peek(); }
        }

        public bool EOF
        {
            get { return eof; }
        }

        private void GetCh()
        {
            if (pos >= _source.Length)
            {
                eof = true;
                ch = default(char);
                
                // Increment linePos and pos so Positions are correct
                if (pos == _source.Length)
                {
                    linePos++;
                    pos++;
                }
            }
            else
            {
                if ((ch == CR) || (ch == LF))
                {
                    if ((ch != LF) || (pos < 1) || (_source[pos - 2] != CR))
                    {
                        lineNo++;
                        linePos = 1;
                        linestarts.Add(pos);
                    }
                }
                else
                    linePos++;
                ch = _source[pos++];
            }
        }

        private void GetCh(int count)
        {
            for (int i = 0; i < count; i++)
                GetCh();
        }

        private char LookAhead(int lookAhead)
        {
            if (pos+lookAhead > _source.Length)
                return default(char);
            return _source[pos-1 + lookAhead];
        }

        public Token GetToken()
        {
            if (ch == default(char))
                return null;

            Token token;

            // Save the current position to the previous position
            prevPos = currPos;

            ScannerState currentState = state.Peek();
            switch (currentState)
            {
                case ScannerState.Default:
                    token = ScanTokenDefault();
                    break;
                case ScannerState.XmlComment:
                    token = ScanTokenXmlComment();
                    break;
                case ScannerState.XmlTag:
                case ScannerState.XmlTagAttributes:
                    token = ScanTokenXmlTag();
                    break;
                case ScannerState.XmlTagAttributeValue:
                    token = ScanTokenXmlTagAttributeValue();
                    break;
                case ScannerState.XmlCData:
                    token = ScanTokenXmlCData();
                    break;
                case ScannerState.XmlScriptElementContent:
                    token = ScanTokenXmlScriptElementContent();
                    break;
                case ScannerState.NVMultilineComment:
                    token = ScanTokenNVMultilineComment();
                    break;
                case ScannerState.NVPreDirective:
                    token = ScanTokenNVPreDirective();
                    break;
                case ScannerState.NVDirective:
                    token = ScanTokenNVDirective();
                    break;
                case ScannerState.NVDirectiveParams:
                    token = ScanTokenNVDirectiveParams();
                    break;
                case ScannerState.NVReference:
                    token = ScanTokenNVReference();
                    break;
                case ScannerState.NVReferenceSelectors:
                    token = ScanTokenNVReferenceSelectors();
                    break;
                case ScannerState.NVStringLiteralSingle:
                    token = ScanTokenNVStringLiteralSingle();
                    break;
                case ScannerState.NVStringLiteralDouble:
                    token = ScanTokenNVStringLiteralDouble();
                    break;
                case ScannerState.NVDictionary:
                    token = ScanTokenNVDictionary();
                    break;
                case ScannerState.NVDictionaryInner:
                    token = ScanTokenNVDictionaryInner();
                    break;
                case ScannerState.NVParens:
                    token = ScanTokenNVParens();
                    break;
                case ScannerState.NVBracks:
                    token = ScanTokenNVBrack();
                    break;
                default:
                    throw new ScannerError("Unknown state '" + currentState + "'");
            }

            // Store the current position
            currPos.StartLine = prevPos.EndLine;
            currPos.StartPos = prevPos.EndPos;
            currPos.EndLine = lineNo;
            currPos.EndPos = linePos;

            if (token.Type == TokenType.Error)
            {
                throw new ScannerError(string.Format("Unknown symbol '{0}' in state {1}",
                    ch, ScannerStateToString(state.Peek())));
            }

            return token;
        }

        private Token ScanTokenDefault()
        {
            Token token = new Token();

            token.SetStartPosition(lineNo, linePos);

            bool scanXmlText = true;

            if (ch == '<')
            {
                if (LookAhead(1) == '!')
                {
                    if (LookAhead(2) == '-' && LookAhead(3) == '-')
                    {
                        // It is an XML comment
                        int startCommentPos = pos;
                        GetCh(4);
                        if (isLineScanner)
                        {
                            token.Type = TokenType.XmlCommentStart;
                            state.Push(ScannerState.XmlComment);
                        }
                        else
                        {
                            token.Type = TokenType.XmlComment;
                            ReadXmlComment();
                            token.Image = _source.Substring(startCommentPos - 1, pos - startCommentPos);
                        }
                        token.SetEndPosition(lineNo, linePos);
                        return token;
                    }
                    else if (LookAhead(2) == '[' && LookAhead(3) == 'C' && LookAhead(4) == 'D' &&
                             LookAhead(5) == 'A' && LookAhead(6) == 'T' && LookAhead(7) == 'A' && LookAhead(8) == '[')
                    {
                        // It is an XML CData section
                        GetCh(9);
                        token.Type = TokenType.XmlCDataStart;
                        state.Push(ScannerState.XmlCData);
                        token.SetEndPosition(lineNo, linePos);
                        return token;
                    }
                }

                // It is an XML element
                token.Type = TokenType.XmlTagStart;
                state.Push(ScannerState.XmlTag);
                GetCh();
                scanXmlText = false;
            }
            else if (ch == '#')
            {
                if (LookAhead(1) == '#')
                {
                    GetCh();
                    token.Type = TokenType.NVSingleLineComment;
                    int prevCommentPos = pos - 2;
                    GetCh();
                    while (ch != CR && ch != LF && !eof)
                        GetCh();
                    if (ch == CR)
                        GetCh();
                    if (ch == LF)
                        GetCh();
                    token.Image = _source.Substring(prevCommentPos, pos - prevCommentPos - 1);
                    scanXmlText = false;
                }
                else if (LookAhead(1) == '*')
                {
                    GetCh();
                    if (isLineScanner)
                    {
                        token.Type = TokenType.NVMultilineCommentStart;
                        state.Push(ScannerState.NVMultilineComment);
                        GetCh();
                    }
                    else
                    {
                        token = ReadNVelocityMultiLineComment();
                    }
                    scanXmlText = false;
                }
                else if (NVDirectiveFollows())
                {
                    GetCh();
                    token.Type = TokenType.NVDirectiveHash;
                    state.Push(ScannerState.NVPreDirective);
                    scanXmlText = false;
                }
            }
            else if (ch == '$' && NVReferenceFollows())
            {
                token.Type = TokenType.NVDollar;
                state.Push(ScannerState.NVReference);
                GetCh();
                scanXmlText = false;
            }

            // If it is not any other type of parseable syntax
            if (scanXmlText)
            {
                int startPos = pos;
                bool isText = true;
                while (isText && !eof)
                {
                    if (ch == '#' && NVDirectiveFollows())
                    {
                        isText = false;
                    }
                    else if (ch == '$' && NVReferenceFollows())
                    {
                        isText = false;
                    }
                    else if (ch == '<')
                    {
                        isText = false;
                    }
                    else
                    {
                        GetCh();
                    }
                }
                token.Type = TokenType.XmlText;
                token.Image = _source.Substring(startPos - 1, pos - startPos); 
            }

            token.SetEndPosition(lineNo, linePos);

            return token;
        }

        private Token ScanTokenXmlComment()
        {
            Token token = new Token();

            token.SetStartPosition(lineNo, linePos);

            if (ch == '-' && LookAhead(1) == '-' && LookAhead(2) == '>')
            {
                token.Type = TokenType.XmlCommentEnd;
                state.Pop(); // XmlComment
                GetCh(); // Skip over '-'
                GetCh(); // Skip over '-'
                GetCh(); // Skip over '>'
            }
            else
            {
                int startPos = pos;
                bool endFound = false;
                while (!eof && !endFound)
                {
                    if (ch == '-' && LookAhead(1) == '-' && LookAhead(2) == '>')
                        endFound = true;
                    else
                        GetCh();
                }

                token.Type = TokenType.XmlComment;
                token.Image = _source.Substring(startPos - 1, pos - startPos);
            }

            token.SetEndPosition(lineNo, linePos);

            return token;
        }

        private Token ScanTokenXmlTag()
        {
            Token token = new Token();

            ConsumeWhiteSpace();

            token.SetStartPosition(lineNo, linePos);

            if (char.IsLetter(ch) || ch == '_' || ch == ':')
            {
                int startPos = pos;
                GetCh();
                while (char.IsLetterOrDigit(ch) || ch == '.' || ch == '-' || ch == '_' || ch == ':')
                    GetCh();

                token.Image = _source.Substring(startPos - 1, pos - startPos);

                if (state.Peek() == ScannerState.XmlTag)
                {
                    token.Type = TokenType.XmlTagName;

                    if (token.Image == "script")
                    {
                        // Pop off the current tag and push on a script element content section so
                        // when this tag finishes the scanner will be in the script element content.

                        state.Pop(); // Pop XmlTag

                        // If the scanner is already in a script element content when it gets to this
                        // tag then this must be the end tag, so we can pop off the script element
                        // content and push back on the XmlTag
                        if (state.Peek() == ScannerState.XmlScriptElementContent)
                            state.Pop(); // XmlScriptElementContent
                        else
                            state.Push(ScannerState.XmlScriptElementContent);

                        state.Push(ScannerState.XmlTag); // Put it back
                    }

                    state.Push(ScannerState.XmlTagAttributes); // It is now in the attributes section
                }
                else
                {
                    token.Type = TokenType.XmlAttributeName;
                }
            }
            else
            {
                switch (ch)
                {
                    case '>':
                        token.Type = TokenType.XmlTagEnd;
                        if (state.Peek() == ScannerState.XmlTagAttributes)
                            state.Pop(); // Pop XmlTagAttributes
                        state.Pop(); // Pop XmlTag
                        break;
                    case '/':
                        token.Type = TokenType.XmlForwardSlash;
                        break;
                    case '=':
                        token.Type = TokenType.XmlEquals;
                        break;
                    case '"':
                        token.Type = TokenType.XmlDoubleQuote;
                        state.Push(ScannerState.XmlTagAttributeValue);
                        break;
                    case '?':
                        token.Type = TokenType.XmlQuestionMark;
                        break;
                    case '!':
                        token.Type = TokenType.XmlExclaimationMark;
                        break;
                    case '#':
                        token.Type = TokenType.NVDirectiveHash;
                        state.Push(ScannerState.NVPreDirective);
                        break;
                    case '$':
                        token.Type = TokenType.NVDollar;
                        state.Push(ScannerState.NVReference);
                        break;
                    default:
                        token.Type = TokenType.Error;
                        break;
                }

                if (token.Type != TokenType.Error)
                    GetCh();
            }

            token.SetEndPosition(lineNo, linePos);
            
            return token;
        }

        private Token ScanTokenXmlCData()
        {
            Token token = new Token();

            token.SetStartPosition(lineNo, linePos);

            if (ch == ']' && LookAhead(1) == ']' && LookAhead(2) == '>')
            {
                token.Type = TokenType.XmlCDataEnd;
                GetCh(3);
                state.Pop(); // Pop NVXmlCData
                token.SetEndPosition(lineNo, linePos);
                return token;
            }

            int startPos = pos;
            while (!(ch == ']' && LookAhead(1) == ']' && LookAhead(2) == '>'))
                GetCh();

            token.Type = TokenType.XmlText;
            token.Image = _source.Substring(startPos - 1, pos - startPos);

            token.SetEndPosition(lineNo, linePos);

            return token;
        }

        private Token ScanTokenXmlTagAttributeValue()
        {
            Token token = new Token();

            token.SetStartPosition(lineNo, linePos);

            bool scanXmlText = true;

            if (ch == '"')
            {
                token.Type = TokenType.XmlDoubleQuote;
                GetCh();
                state.Pop(); // Pop XmlTagAttributeValue
                scanXmlText = false;
            }
            else if (ch == '#' && NVDirectiveFollows())
            {
                token.Type = TokenType.NVDirectiveHash;
                state.Push(ScannerState.NVPreDirective);
                GetCh();
                scanXmlText = false;
            }
            else if (ch == '$' && NVReferenceFollows())
            {
                token.Type = TokenType.NVDollar;
                state.Push(ScannerState.NVReference);
                GetCh();
                scanXmlText = false;
            }
            
            if (scanXmlText)
            {
                int startPos = pos;
                bool isText = true;
                while (isText && !eof)
                {
                    if (ch == '#' && NVDirectiveFollows())
                    {
                        isText = false;
                    }
                    else if (ch == '$' && NVReferenceFollows())
                    {
                        isText = false;
                    }
                    else if (ch == '"')
                    {
                        isText = false;
                    }
                    else
                    {
                        GetCh();
                    }
                }

                if (eof && !isLineScanner)
                    throw new ScannerError("End-of-file found but quoted string literal was not closed");
                token.Type = TokenType.XmlAttributeText;
                token.Image = _source.Substring(startPos - 1, pos - startPos);
            }

            token.SetEndPosition(lineNo, linePos);

            return token;
        }

        private Token ScanTokenXmlScriptElementContent()
        {
            Token token = new Token();

            token.SetStartPosition(lineNo, linePos);

            if (ch == '<' && LookAhead(1) == '/' &&
                LookAhead(2) == 's' && LookAhead(3) == 'c' && LookAhead(4) == 'r' &&
                LookAhead(5) == 'i' && LookAhead(6) == 'p' && LookAhead(7) == 't' &&
                LookAhead(8) == '>')
            {
                token.Type = TokenType.XmlTagStart;
                GetCh();
                state.Push(ScannerState.XmlTag);
                token.SetEndPosition(lineNo, linePos);
                return token;
            }

            int startPos = pos;
            while (!(ch == '<' && LookAhead(1) == '/' &&
                LookAhead(2) == 's' && LookAhead(3) == 'c' && LookAhead(4) == 'r' &&
                LookAhead(5) == 'i' && LookAhead(6) == 'p' && LookAhead(7) == 't' &&
                LookAhead(8) == '>') && !eof)
            {
                GetCh();
            }

            if (eof)
                throw new ScannerError("Expected closing 'script' element");

            token.Type = TokenType.XmlText;
            token.Image = _source.Substring(startPos - 1, pos - startPos);

            token.SetEndPosition(lineNo, linePos);

            return token;
        }

        private Token ScanTokenNVMultilineComment()
        {
            Token token = new Token();

            token.SetStartPosition(lineNo, linePos);
            
            if (ch == '*' && LookAhead(1) == '#')
            {
                token.Type = TokenType.NVMultilineCommentEnd;
                state.Pop(); // NVMultilineComment
                GetCh(); // Skip over '*'
                GetCh(); // Skip over '#'
            }
            else
            {
                int startPos = pos;
                bool endFound = false;
                while (!eof && !endFound)
                {
                    if (ch == '*' && LookAhead(1) == '#')
                        endFound = true;
                    else
                        GetCh();
                }

                token.Type = TokenType.NVMultilineComment;
                token.Image = _source.Substring(startPos - 1, pos - startPos);
            }

            token.SetEndPosition(lineNo, linePos);

            return token;
        }

        private Token ScanTokenNVPreDirective()
        {
            Token token = new Token();

            token.SetStartPosition(lineNo, linePos);

            bool hasBraces = false;
            if (ch == '{')
            {
                hasBraces = true;
                GetCh();
            }

            if (char.IsLetter(ch))
            {
                int startPos = pos;
                while (char.IsLetter(ch))
                    GetCh();
                token.Type = TokenType.NVDirectiveName;
                token.Image = _source.Substring(startPos - 1, pos - startPos);

                if (hasBraces)
                {
                    if (ch == '}')
                        GetCh();
                    else
                        throw new ScannerError("Expected '}' for closing directive name");
                }

                state.Pop(); // Pop NVPreDirective
                state.Push(ScannerState.NVDirective);
            }
            else
                token.Type = TokenType.Error;

            token.SetEndPosition(lineNo, linePos);

            return token;
        }

        private Token ScanTokenNVDirective()
        {
            Token token = new Token();

            token.SetStartPosition(lineNo, linePos);

            if (NextCharAfterWhiteSpace() == '(')
            {
                ConsumeSingleLineWhiteSpace();
                token.Type = TokenType.NVDirectiveLParen;
                GetCh();

                state.Push(ScannerState.NVDirectiveParams);
            }
            else
            {
                state.Pop(); // Pop NVDirective
                token = GetToken();
            }

            token.SetEndPosition(lineNo, linePos);

            return token;
        }

        private Token ScanTokenNVDirectiveParams()
        {
            Token token = new Token();

            ConsumeSingleLineWhiteSpace();

            token.SetStartPosition(lineNo, linePos);

            if (ch == ')')
            {
                token.Type = TokenType.NVDirectiveRParen;
                GetCh();
                state.Pop(); // Pop NVDirectiveParams
                state.Pop(); // Pop NVDirective
            }
            else
            {
                token = ReadNVelocityToken();
            }

            token.SetEndPosition(lineNo, linePos);

            return token;
        }

        private Token ScanTokenNVReference()
        {
            Token token = new Token();

            token.SetStartPosition(lineNo, linePos);

            if (char.IsLetter(ch) || ch == '_')
            {
                token = ReadNVelocityReference();
            }
            else if (ch == '!')
            {
                token.Type = TokenType.NVReferenceSilent;
                GetCh();
            }
            else if (ch == '{')
            {
                token.Type = TokenType.NVReferenceLCurly;
                GetCh();
            }
            else if (ch == '"')
            {
                token.Type = TokenType.NVDoubleQuote;
                state.Pop(); // Pop NVReference
                state.Pop(); // Pop NVStringLiteralDouble
                GetCh();
            }
            else
            {
                throw new ScannerError("Expected reference identifier");
            }

            token.SetEndPosition(lineNo, linePos);

            return token;
        }

        private Token ScanTokenNVReferenceSelectors()
        {
            Token token = new Token();

            token.SetStartPosition(lineNo, linePos);

            if (char.IsLetter(ch))
            {
                int startPos = pos;
                GetCh();
                while (char.IsLetterOrDigit(ch) || ch == '_' || ch == '-')
                    GetCh();
                token.Type = TokenType.NVIdentifier;
                token.Image = _source.Substring(startPos - 1, pos - startPos);
            }
            else if (ch == '.')
            {
                token.Type = TokenType.NVDot;
                GetCh();
            }
            else if (ch == '(')
            {
                token.Type = TokenType.NVLParen;
                state.Push(ScannerState.NVParens);
                GetCh();
            }
            else if (ch == '}')
            {
                token.Type = TokenType.NVReferenceRCurly;
                if (state.Peek() == ScannerState.NVReferenceSelectors)
                {
                    state.Pop(); // Pop NVReferenceSelectors
                    state.Pop(); // Pop NVReference
                }
                GetCh();
            }
            else
            {
                state.Pop(); // Pop NVReferenceSelectors
                state.Pop(); // Pop NVReference
                token = GetToken();
            }

            token.SetEndPosition(lineNo, linePos);

            return token;
        }

        private Token ScanTokenNVStringLiteralSingle()
        {
            Token token = new Token();

            token.SetStartPosition(lineNo, linePos);

            if (ch == '\'')
            {
                state.Pop(); // Pop NVStringLiteralSingle
                token.Type = TokenType.NVSingleQuote;
                GetCh();
                token.SetEndPosition(lineNo, linePos);
                return token;
            }

            int startPos = pos;
            while (ch != '\'' && !eof)
                GetCh();

            if (ch == '\'')
            {
                token.Type = TokenType.NVStringLiteral;
                token.Image = _source.Substring(startPos - 1, pos - startPos);
            }
            else
                throw new ScannerError("Expected end of string literal");

            token.SetEndPosition(lineNo, linePos);

            return token;
        }

        private Token ScanTokenNVStringLiteralDouble()
        {
            Token token = new Token();

            token.SetStartPosition(lineNo, linePos);

            if (ch == '"')
            {
                state.Pop(); // Pop NVStringLiteralDouble
                token.Type = TokenType.NVDoubleQuote;
                GetCh();
                token.SetEndPosition(lineNo, linePos);
                return token;
            }
            else if (ch == '$')
            {
                if (NVReferenceFollows())
                {
                    token.Type = TokenType.NVDollar;
                    state.Push(ScannerState.NVReference);
                }
                else
                {
                    token.Type = TokenType.NVStringLiteral;
                    token.Image = "$";
                }
                GetCh();
                token.SetEndPosition(lineNo, linePos);
                return token;
            }

            int startPos = pos;
            while (ch != '"' && ch != '$' && !eof)
                GetCh();

            if (!eof)
            {
                token.Type = TokenType.NVStringLiteral;
                token.Image = _source.Substring(startPos - 1, pos - startPos);
            }
            else
                throw new ScannerError("Expected end of string literal");

            token.SetEndPosition(lineNo, linePos);

            return token;
        }

        private Token ScanTokenNVDictionary()
        {
            Token token = new Token();

            token.SetStartPosition(lineNo, linePos);

            if (ch == '%')
            {
                token.Type = TokenType.NVDictionaryPercent;
                GetCh();
            }
            else if (ch == '{')
            {
                token.Type = TokenType.NVDictionaryLCurly;
                GetCh();
                state.Push(ScannerState.NVDictionaryInner);
            }
            else if (ch == '"')
            {
                token.Type = TokenType.NVDoubleQuote;
                state.Pop(); // Pop NVDictionary
                GetCh();
            }
            else
            {
                throw new ScannerError("Expected opening dictionary declaration");
            }

            token.SetEndPosition(lineNo, linePos);

            return token;
        }

        private Token ScanTokenNVDictionaryInner()
        {
            Token token = new Token();

            ConsumeSingleLineWhiteSpace();

            token.SetStartPosition(lineNo, linePos);

            if (ch == '}')
            {
                token.Type = TokenType.NVDictionaryRCurly;
                GetCh();
                state.Pop(); // Pop NVDictionaryInner
            }
            else if (char.IsLetter(ch))
            {
                int startPos = pos;
                GetCh();
                while (char.IsLetter(ch))
                    GetCh();
                token.Type = TokenType.NVDictionaryKey;
                token.Image = _source.Substring(startPos - 1, pos - startPos);
            }
            else if (ch == '=')
            {
                token.Type = TokenType.NVDictionaryEquals;
                GetCh();
            }
            else if (ch == ',')
            {
                token.Type = TokenType.NVComma;
                GetCh();
            }
            else
            {
                token = ReadNVelocityToken();
            }

            token.SetEndPosition(lineNo, linePos);

            return token;
        }

        private Token ScanTokenNVParens()
        {
            Token token = new Token();

            ConsumeSingleLineWhiteSpace();

            token.SetStartPosition(lineNo, linePos);

            if (ch == ')')
            {
                token.Type = TokenType.NVRParen;
                GetCh();
                state.Pop(); // Pop NVParens

                // Pop out of the reference if the closing parenthesis isn't followed by a '.'.
                // For example: $var.Method()text
                if (state.Peek() == ScannerState.NVReferenceSelectors && ch != '.')
                {
                    state.Pop(); // Pop NVReferenceSelectors
                    state.Pop(); // Pop NVReference
                }
            }
            else
            {
                token = ReadNVelocityToken();
            }

            token.SetEndPosition(lineNo, linePos);

            return token;
        }

        private Token ScanTokenNVBrack()
        {
            Token token = new Token();

            ConsumeSingleLineWhiteSpace();

            token.SetStartPosition(lineNo, linePos);

            if (ch == ']')
            {
                token.Type = TokenType.NVRBrack;
                GetCh();
                state.Pop(); // Pop NVBrack
            }
            else
            {
                token = ReadNVelocityToken();
            }

            token.SetEndPosition(lineNo, linePos);

            return token;
        }

        private void ConsumeWhiteSpace()
        {
            while (char.IsWhiteSpace(ch) || ch == CR || ch == LF)
            {
                GetCh();
            }
        }

        private void ConsumeSingleLineWhiteSpace()
        {
            while (char.IsWhiteSpace(ch))
            {
                GetCh();
            }
        }

        //private void ConsumeNewLines()
        //{
        //    while (ch == CR || ch == LF)
        //    {
        //        GetCh();
        //    }
        //}

        private char NextCharAfterWhiteSpace()
        {
            int offset = 0;
            while ((pos + offset < _source.Length) && char.IsWhiteSpace(_source[pos - 1 + offset]))
            {
                offset++;
            }
            return _source[pos-1 + offset];
        }

        private void ReadXmlComment()
        {
            bool endFound = false;
            while (!eof && !endFound)
            {
                if (ch == '-')
                {
                    GetCh();
                    if (ch == '-')
                    {
                        GetCh();
                        if (ch == '>')
                        {
                            endFound = true;
                            GetCh();
                        }
                        else
                            throw new ScannerError("Expected end of XML comment");
                    }
                }
                else
                    GetCh();
            }
            if (!endFound && eof)
                throw new ScannerError("Expected end of XML comment");
        }

        private Token ReadNVelocityMultiLineComment()
        {
            Token token = new Token();
            int startPos = pos - 1; // '- 1' to include '#'

            bool endFound = false;
            while (!eof && !endFound)
            {
                if (ch == '*')
                {
                    GetCh();
                    if (ch == '#')
                    {
                        endFound = true;
                    }
                }
                else
                    GetCh();
            }
            if (!endFound && eof)
                throw new ScannerError("Expected end of NVelocity comment");

            token.Type = TokenType.NVSingleLineComment;
            token.Image = _source.Substring(startPos - 1, pos - startPos + 1);
            GetCh();

            return token;
        }

        private Token ReadNVelocityReference()
        {
            Token token = ReadNVelocityIdentifier();
            
            if (token.Type == TokenType.NVIdentifier)
                state.Push(ScannerState.NVReferenceSelectors);

            return token;
        }

        private bool NVDirectiveFollows()
        {
            char lookAhead = LookAhead(1);
            return (char.IsLetter(lookAhead) || lookAhead == '{' || lookAhead == '#' || lookAhead == '*');
        }

        private bool NVReferenceFollows()
        {
            char lookAhead = LookAhead(1);
            return (char.IsLetter(lookAhead) || lookAhead == '_' || lookAhead == '!' || lookAhead == '{');
        }

        private Token ReadNVelocityIdentifier()
        {
            Token token = new Token();

            int startPos = pos;
            GetCh();
            while (char.IsLetterOrDigit(ch) || ch == '_' || ch == '-')
                GetCh();

            string ident = _source.Substring(startPos - 1, pos - startPos);
            if (nvKeywords.ContainsKey(ident))
            {
                token.Type = nvKeywords[ident];
            }
            else
            {
                token.Type = TokenType.NVIdentifier;
                token.Image = ident;
            }

            return token;
        }

        private Token ReadNVelocityToken()
        {
            Token token = new Token();
            
            ConsumeSingleLineWhiteSpace();

            if (char.IsLetter(ch) || ch == '_')
            {
                if (state.Peek() == ScannerState.NVReference)
                    token = ReadNVelocityReference();
                else
                    token = ReadNVelocityIdentifier();
            }
            else if (char.IsNumber(ch))
            {
                int startPos = pos;
                GetCh();
                while (char.IsDigit(ch))
                    GetCh();
                token.Type = TokenType.NVIntegerLiteral;
                token.Image = _source.Substring(startPos - 1, pos - startPos);
            }
            else
            {
                switch (ch)
                {
                    case '(':
                        token.Type = TokenType.NVLParen;
                        state.Push(ScannerState.NVParens);
                        break;
                    case '[':
                        token.Type = TokenType.NVLBrack;
                        state.Push(ScannerState.NVBracks);
                        break;
                    case '$':
                        token.Type = TokenType.NVDollar;
                        state.Push(ScannerState.NVReference);
                        break;
                    case '=':
                        if (LookAhead(1) == '=')
                        {
                            token.Type = TokenType.NVEqEq;
                            GetCh();
                        }
                        else
                            token.Type = TokenType.NVEq;
                        break;
                    case '>':
                        if (LookAhead(1) == '=')
                        {
                            token.Type = TokenType.NVGte;
                            GetCh();
                        }
                        else
                            token.Type = TokenType.NVGt;
                        break;
                    case '<':
                        if (LookAhead(1) == '=')
                        {
                            token.Type = TokenType.NVLte;
                            GetCh();
                        }
                        else
                            token.Type = TokenType.NVLt;
                        break;
                    case '+':
                        token.Type = TokenType.NVPlus;
                        break;
                    case '-':
                        token.Type = TokenType.NVMinus;
                        break;
                    case '*':
                        token.Type = TokenType.NVMul;
                        break;
                    case '/':
                        token.Type = TokenType.NVDiv;
                        break;
                    case '%':
                        token.Type = TokenType.NVMod;
                        break;
                    case '&':
                        if (LookAhead(1) == '&')
                        {
                            token.Type = TokenType.NVAnd;
                            GetCh();
                        }
                        else
                            token.Type = TokenType.Error;
                        break;
                    case '|':
                        if (LookAhead(1) == '|')
                        {
                            token.Type = TokenType.NVOr;
                            GetCh();
                        }
                        else
                            token.Type = TokenType.Error;
                        break;
                    case '!':
                        token.Type = TokenType.NVNot;
                        break;
                    case ',':
                        token.Type = TokenType.NVComma;
                        break;
                    case '.':
                        if (LookAhead(1) == '.')
                        {
                            token.Type = TokenType.NVDoubleDot;
                            GetCh();
                        }
                        else
                            token.Type = TokenType.NVDot;
                        break;
                    case '"':
                        token.Type = TokenType.NVDoubleQuote;
                        if (LookAhead(1) == '%')
                            state.Push(ScannerState.NVDictionary);
                        else
                            state.Push(ScannerState.NVStringLiteralDouble);
                        break;
                    case '\'':
                        token.Type = TokenType.NVSingleQuote;
                        state.Push(ScannerState.NVStringLiteralSingle);
                        break;
                    default:
                        token.Type = TokenType.Error;
                        break;
                }

                if (token.Type != TokenType.Error)
                    GetCh();
            }
            return token;
        }

        private void SetUpReservedWords()
        {
            nvKeywords.Add("true", TokenType.NVTrue);
            nvKeywords.Add("false", TokenType.NVFalse);
            nvKeywords.Add("in", TokenType.NVIn);
            nvKeywords.Add("with", TokenType.NVWith);

            nvKeywords.Add("lt", TokenType.NVLt);
            nvKeywords.Add("le", TokenType.NVLte);
            nvKeywords.Add("gt", TokenType.NVGt);
            nvKeywords.Add("ge", TokenType.NVGte);
            nvKeywords.Add("eq", TokenType.NVEqEq);
            nvKeywords.Add("ne", TokenType.NVNeq);
            nvKeywords.Add("and", TokenType.NVAnd);
            nvKeywords.Add("or", TokenType.NVOr);
            nvKeywords.Add("not", TokenType.NVNot);
        }

        private static string ScannerStateToString(ScannerState state)
        {
            switch(state)
            {
                case ScannerState.Default:
                    return "Default";
                case ScannerState.XmlComment:
                    return "XML Comment";
                case ScannerState.XmlTag:
                    return "XML Tag";
                case ScannerState.XmlTagAttributes:
                    return "XML Tag Attributes";
                case ScannerState.XmlTagAttributeValue:
                    return "XML Tag Attribute Value";
                case ScannerState.XmlCData:
                    return "XML CData";
                case ScannerState.XmlScriptElementContent:
                    return "XML Script Element Content";
                case ScannerState.NVMultilineComment:
                    return "NVelocity Multiline Comment";
                case ScannerState.NVPreDirective:
                    return "NVelocity Pre-Directive";
                case ScannerState.NVDirective:
                    return "NVelocity Directive";
                case ScannerState.NVDirectiveParams:
                    return "NVelocity Directive Parameters";
                case ScannerState.NVReference:
                    return "NVelocity Reference";
                case ScannerState.NVReferenceSelectors:
                    return "NVelocity Reference Selectors";
                case ScannerState.NVStringLiteralSingle:
                    return "NVelocity String Literal";
                case ScannerState.NVStringLiteralDouble:
                    return "NVelocity Parsed String Literal";
                case ScannerState.NVDictionary:
                    return "NVelocity Dictionary";
                case ScannerState.NVDictionaryInner:
                    return "NVelocity Dictionary Key/Value Pairs";
                case ScannerState.NVParens:
                    return "NVelocity Parentheses";
                case ScannerState.NVBracks:
                    return "NVelocity Brackets";
                default:
                    return "Unknown State";
            }
        }
    }
}