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
        /// <summary>
        /// Represents the method that defines a set of criteria and determines
        /// whether the scanner should stop scanning when it meets those criteria.
        /// </summary>
        /// <returns>
        /// Returns true if the scanner should stop scanning at the current character
        /// because it meets the criteria defined within the method represented by this
        /// delegate; otherwise, false.
        /// </returns>
        private delegate bool StopScanningDelegate();

        private static readonly char CR = '\r';
        private static readonly char LF = '\n';

        private Stack<ScannerState> state = new Stack<ScannerState>();
        private Queue<Token> _prereadTokens = new Queue<Token>();

        private Token _currentToken;

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
        private bool splitTextTokens = false;
        private ScannerOptions _options = new ScannerOptions();

        /// <summary>
        /// Creates a new Scanner.
        /// </summary>
        public Scanner()
        {
            SetUpReservedWords();
        }

        /// <summary>
        /// Sets the stack of state information to the scanner. The scanner copies the data from
        /// the stack and does not use the passed instance of a stack.
        /// </summary>
        /// <param name="restoredState"></param>
        public void RestoreState(Stack<ScannerState> restoredState)
        {
            if (restoredState != null)
            {
                state = new Stack<ScannerState>(restoredState.ToArray());
            }
        }

        /// <summary>
        /// Returns the stack of state information from the scanner. The returned stack a new
        /// stack created from the data in the state.
        /// </summary>
        /// <returns></returns>
        public Stack<ScannerState> RetrieveState()
        {
            return new Stack<ScannerState>(state.ToArray());
        }

        /// <summary>
        /// Initialises the scanner with an input ready for scanner.
        /// </summary>
        /// <param name="source">The input template source.</param>
        public void SetSource(string source)
        {
            if (!isLineScanner || state.Count == 0)
            {
                state.Clear();
                state.Push(ScannerState.Default);
            }

            _prereadTokens.Clear();
            _source = source;
            pos = 1;
            eof = false;
            lineNo = 1;
            linePos = 1;
            linestarts = new ArrayList();
            prevPos = new Position(1, 1, 1, 1);
            currPos = new Position(1, 1, 1, 1);

            if (source.Length == 0)
            {
                eof = true;
                return;
            }

            ch = source[0];
            linestarts.Add(0);
        }

        /// <summary>
        /// Returns the current position of the scanner.
        /// </summary>
        public Position CurrentPos
        {
            get { return currPos; }
        }

        /// <summary>
        /// Returns the previous position of the scanner.
        /// </summary>
        public Position PreviousPos
        {
            get { return prevPos; }
        }

        /// <summary>
        /// Returns the current state of the scanner.
        /// </summary>
        public ScannerState CurrentState
        {
            get { return state.Peek(); }
        }

        /// <summary>
        /// Returns whether the scanner is at the end of the input.
        /// </summary>
        public bool EOF
        {
            get
            {
                if (_currentToken == null)
                {
                    eof = true;
                }
                return eof;
            }
        }

        /// <summary>
        /// Specifies whether the scanner should return error when reaching the end of the input
        /// with unclosed syntax.
        /// </summary>
        public bool IsLineScanner
        {
            get { return isLineScanner; }
            set { isLineScanner = value; }
        }

        /// <summary>
        /// Specifies whether XML and NVelocity text tokens should be returned as multiple tokens
        /// based on word boundaries.
        /// </summary>
        public bool SplitTextTokens
        {
            get { return splitTextTokens; }
            set { splitTextTokens = value; }
        }

        /// <summary>
        /// Moves the current position to the next character in the input.
        /// </summary>
        private void GetCh()
        {
            if (pos >= _source.Length)
            {
                eof = true;
                ch = default(char);
                
                // Increment linePos and _pos so Positions are correct
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

        /// <summary>
        /// Moves the current position the specified number of characters ahead.
        /// </summary>
        /// <param name="count">The number of characters to move over.</param>
        private void GetCh(int count)
        {
            for (int i = 0; i < count; i++)
                GetCh();
        }

        /// <summary>
        /// Returns the character at the specified number of characters ahead from the current position.
        /// </summary>
        /// <param name="lookAhead">The number of character to look ahead.</param>
        /// <returns>The character at the specified number of characters ahead from the current position.</returns>
        private char LookAhead(int lookAhead)
        {
            if (pos+lookAhead > _source.Length)
            {
                return default(char);
            }

            return _source[pos-1 + lookAhead];
        }

        /// <summary>
        /// Returns the next token from the current position.
        /// </summary>
        /// <returns>The next token from the current position.</returns>
        public Token GetToken()
        {
            if (_prereadTokens.Count > 0)
            {
                _currentToken = _prereadTokens.Dequeue();
            }
            else
            {
                _currentToken = GetNextToken();
            }

            if (_currentToken != null)
            {
                eof = false;
            }

            return _currentToken;
        }

        /// <summary>
        /// Returns the current token from the scanner, it does not scan ahead for the token.
        /// </summary>
        public Token CurrentToken
        {
            get { return _currentToken; }
        }

        public ScannerOptions Options
        {
            get { return _options; }
            set { _options = value; }
        }

        /// <summary>
        /// Returns the token at the specificed lookahead count.
        /// </summary>
        /// <param name="lookAhead">The number of tokens to lookahead.</param>
        /// <returns>The token at the specified lookahead count.</returns>
        public Token PeekToken(int lookAhead)
        {
            // Keep scanning and storing tokens until it has 'lookAhead' number of tokens
            while (_prereadTokens.Count < lookAhead && !eof)
            {
                Token token = GetNextToken();
                _prereadTokens.Enqueue(token);
            }

            // Return the token from the queue
            if (_prereadTokens.Count >= lookAhead)
            {
                return _prereadTokens.ToArray()[lookAhead - 1];
            }
            return null;
        }

        /// <summary>
        /// Returns the next token from the scanner beginning from the current internal position,
        /// which ignores the preread tokens.
        /// </summary>
        /// <returns>The next token from the scanner.</returns>
        private Token GetNextToken()
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

            if (token != null && token.Type == TokenType.Error)
            {
                throw new ScannerError(string.Format("Unknown symbol '{0}' in state {1}",
                                                     ch, ScannerStateToString(state.Peek())));
            }

            return token;
        }

        /// <summary>
        /// Scans the input and returns XML text tokens.
        /// </summary>
        /// <returns>XML text tokens.</returns>
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
                             LookAhead(5) == 'A' && LookAhead(6) == 'T' && LookAhead(7) == 'A' &&
                             LookAhead(8) == '[')
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
                token = ReadText(TokenType.XmlText, delegate
                {
                    return ch == '<' ||
                           (ch == '#' && NVDirectiveFollows()) ||
                           (ch == '$' && NVReferenceFollows());
                });
            }

            token.SetEndPosition(lineNo, linePos);

            return token;
        }

        private Token ReadText(TokenType tokenType, StopScanningDelegate stopScanningDelegate)
        {
            Token token = new Token();
            token.SetStartPosition(lineNo, linePos);

            int startPos = pos;
            bool isText = true;
            bool inWord = false;
            bool inSpace = false;
            bool hasTokenToReturn = false;

            while (isText && !hasTokenToReturn && !eof)
            {
                // The predicate is used to decide whether the current character is text
                // or it belongs to something else (eg. directive or reference).
                if (stopScanningDelegate.Invoke())
                {
                    isText = false;
                }
                else
                {
                    if (splitTextTokens)
                    {
                        if (char.IsLetter(ch))
                        {
                            if (inSpace)
                            {
                                // End of space found
                                inSpace = false;
                                hasTokenToReturn = true;
                            }
                            else
                            {
                                if (!inWord)
                                {
                                    // Entering a new word
                                    inWord = true;
                                }
                                GetCh();
                            }
                        }
                        else if (char.IsWhiteSpace(ch))
                        {
                            if (inWord)
                            {
                                // End of word found
                                inWord = false;
                                hasTokenToReturn = true;
                            }
                            else
                            {
                                if (!inSpace)
                                {
                                    // Entering new space block
                                    inSpace = true;
                                }
                                GetCh();
                            }
                        }
                        else
                        {
                            if (inWord)
                            {
                                // End of word found
                                inWord = false;
                                hasTokenToReturn = true;
                            }
                            else if (inSpace)
                            {
                                // End of space block found
                                inSpace = false;
                                hasTokenToReturn = true;
                            }
                            else
                            {
                                hasTokenToReturn = true;
                                GetCh();
                            }
                        }
                    }
                    else
                    {
                        GetCh();
                    }
                }
            }
            
            token.SetEndPosition(lineNo, linePos);
            token.Type = tokenType;
            token.Image = _source.Substring(startPos - 1, pos - startPos);

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
                {
                    GetCh();
                }

                token.Image = _source.Substring(startPos - 1, pos - startPos);

                if (state.Peek() == ScannerState.XmlTag)
                {
                    token.Type = TokenType.XmlTagName;

                    // This is commented out to disable the script element content state, script
                    // elements will be treated just like normal XML elements

                    //if (token.Image == "script")
                    //{
                    //    // Pop off the current tag and push on a script element content section so
                    //    // when this tag finishes the scanner will be in the script element content.

                    //    state.Pop(); // Pop XmlTag

                    //    // If the scanner is already in a script element content when it gets to this
                    //    // tag then this must be the end tag, so we can pop off the script element
                    //    // content and push back on the XmlTag
                    //    if (state.Peek() == ScannerState.XmlScriptElementContent)
                    //    {
                    //        state.Pop(); // XmlScriptElementContent
                    //    }
                    //    else
                    //    {
                    //        state.Push(ScannerState.XmlScriptElementContent);
                    //    }

                    //    state.Push(ScannerState.XmlTag); // Put it back
                    //}

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

            token = ReadText(TokenType.XmlCDataSection, delegate
            {
                return (ch == ']' && LookAhead(1) == ']' && LookAhead(2) == '>') || eof;
            });

            if (eof && !isLineScanner)
            {
                throw new ScannerError("End-of-file found but CData section was not closed");
            }

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
                token = ReadText(TokenType.XmlAttributeText, delegate
                {
                    return ch == '"' ||
                           (ch == '#' && NVDirectiveFollows()) ||
                           (ch == '$' && NVReferenceFollows());
                });
            }
            
            if (eof && !isLineScanner)
            {
                throw new ScannerError("End-of-file found but quoted string literal was not closed");
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

            // Scan for text
            token = ReadText(TokenType.XmlText, delegate
            {
                return ch == '<' && LookAhead(1) == '/' && LookAhead(2) == 's' &&
                       LookAhead(3) == 'c' && LookAhead(4) == 'r' && LookAhead(5) == 'i' &&
                       LookAhead(6) == 'p' && LookAhead(7) == 't' && LookAhead(8) == '>';
            });

            if (eof && !isLineScanner)
            {
                throw new ScannerError("Expected closing 'script' element");
            }

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
                {
                    GetCh();
                }
                token.Type = TokenType.NVDirectiveName;
                token.Image = _source.Substring(startPos - 1, pos - startPos);

                if (hasBraces)
                {
                    if (ch == '}')
                    {
                        GetCh();
                    }
                    else
                    {
                        throw new ScannerError("Expected '}' for closing directive name");
                    }
                }

                state.Pop(); // Pop NVPreDirective
                state.Push(ScannerState.NVDirective);
            }
            else
            {
                token.Type = TokenType.Error;
            }

            token.SetEndPosition(lineNo, linePos);

            return token;
        }

        private Token ScanTokenNVDirective()
        {
            Token token = new Token();

            if (NextCharAfterSingleLineWhiteSpace() == '(')
            {
                ConsumeSingleLineWhiteSpace();
                token.SetStartPosition(lineNo, linePos);
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

            if (ch == ')')
            {
                token.SetStartPosition(lineNo, linePos);
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
            else if (ch == '.' && LookAhead(1) != '.') // Do not scan a double dot as a dot
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

            token = ReadText(TokenType.NVStringLiteral, delegate
            {
                return ch == '\'';
            });

            if (eof && !isLineScanner)
            {
                throw new ScannerError("Expected end of string literal");
            }

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
            else if (ch == '#' && NVDirectiveFollows())
            {
                token.Type = TokenType.NVDirectiveHash;
                state.Push(ScannerState.NVPreDirective);
                
                GetCh();
                token.SetEndPosition(lineNo, linePos);
                return token;
            }
            else if (ch == '$' && NVReferenceFollows())
            {
                token.Type = TokenType.NVDollar;
                state.Push(ScannerState.NVReference);

                GetCh();
                token.SetEndPosition(lineNo, linePos);
                return token;
            }

            token = ReadText(TokenType.NVStringLiteral, delegate
            {
                return ch == '"' ||
                       (ch == '#' && NVDirectiveFollows()) ||
                       (ch == '$' && NVReferenceFollows());
            });

            if (eof && !isLineScanner)
            {
                throw new ScannerError("Expected end of string literal");
            }

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

        private char NextCharAfterSingleLineWhiteSpace()
        {
            if (_source.Length == 0)
            {
                return default(char);
            }

            int offset = 0;
            char currCh = _source[pos - 1 + offset];

            while ((pos + offset < _source.Length) && char.IsWhiteSpace(currCh) && currCh != '\n')
            {
                offset++;
                currCh = _source[pos - 1 + offset];
            }

            if (currCh == '\n')
            {
                return default(char);
            }
            else
            {
                return currCh;
            }
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
            if (_options.EnableIntelliSenseTriggerTokens)
                return true;

            char lookAhead = LookAhead(1);
            return (char.IsLetter(lookAhead) || lookAhead == '{' || lookAhead == '#' || lookAhead == '*');
        }

        private bool NVReferenceFollows()
        {
            if (_options.EnableIntelliSenseTriggerTokens)
                return true;

            char lookAhead = LookAhead(1);
            return (char.IsLetter(lookAhead) || lookAhead == '_' || lookAhead == '!' || lookAhead == '{');
        }

        private Token ReadNVelocityIdentifier()
        {
            Token token = new Token();
            token.SetStartPosition(lineNo, linePos);

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

            token.SetStartPosition(lineNo, linePos);

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
