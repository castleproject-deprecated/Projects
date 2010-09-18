#region License
//  Copyright 2004-2010 Castle Project - http:www.castleproject.org/
//  
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//      http:www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// 
#endregion

namespace ICSharpCode.NRefactory.PrettyPrinter
{
	using System.Collections;
	using Parser.CSharp;

	public sealed class CSharpOutputFormatter : AbstractOutputFormatter
	{
		PrettyPrintOptions prettyPrintOptions;
		
		bool          emitSemicolon  = true;
		
		public bool EmitSemicolon {
			get {
				return emitSemicolon;
			}
			set {
				emitSemicolon = value;
			}
		}
		
		public CSharpOutputFormatter(PrettyPrintOptions prettyPrintOptions) : base(prettyPrintOptions)
		{
			this.prettyPrintOptions = prettyPrintOptions;
		}
		
		public override void PrintToken(int token)
		{
			if (token == Tokens.Semicolon && !EmitSemicolon) {
				return;
			}
			PrintText(Tokens.GetTokenString(token));
		}
		
		Stack braceStack = new Stack();
		
		public void BeginBrace(BraceStyle style, bool indent)
		{
			switch (style) {
				case BraceStyle.EndOfLine:
					if (!LastCharacterIsWhiteSpace) {
						Space();
					}
					PrintToken(Tokens.OpenCurlyBrace);
					NewLine();
					if (indent)
						++IndentationLevel;
					break;
				case BraceStyle.EndOfLineWithoutSpace:
					PrintToken(Tokens.OpenCurlyBrace);
					NewLine();
					if (indent)
						++IndentationLevel;
					break;
				case BraceStyle.NextLine:
					NewLine();
					Indent();
					PrintToken(Tokens.OpenCurlyBrace);
					NewLine();
					if (indent)
						++IndentationLevel;
					break;
				case BraceStyle.NextLineShifted:
					NewLine();
					if (indent)
						++IndentationLevel;
					Indent();
					PrintToken(Tokens.OpenCurlyBrace);
					NewLine();
					break;
				case BraceStyle.NextLineShifted2:
					NewLine();
					if (indent)
						++IndentationLevel;
					Indent();
					PrintToken(Tokens.OpenCurlyBrace);
					NewLine();
					++IndentationLevel;
					break;
			}
			braceStack.Push(style);
		}
		
		public void EndBrace (bool indent)
		{
			EndBrace (indent, true);
		}
		
		public void EndBrace (bool indent, bool emitNewLine)
		{
			BraceStyle style = (BraceStyle)braceStack.Pop();
			switch (style) {
				case BraceStyle.EndOfLine:
				case BraceStyle.EndOfLineWithoutSpace:
				case BraceStyle.NextLine:
					if (indent)
						--IndentationLevel;
					Indent();
					PrintToken(Tokens.CloseCurlyBrace);
					if (emitNewLine)
						NewLine();
					break;
				case BraceStyle.NextLineShifted:
					Indent();
					PrintToken(Tokens.CloseCurlyBrace);
					if (emitNewLine)
						NewLine();
					if (indent)
						--IndentationLevel;
					break;
				case BraceStyle.NextLineShifted2:
					if (indent)
						--IndentationLevel;
					Indent();
					PrintToken(Tokens.CloseCurlyBrace);
					if (emitNewLine)
						NewLine();
					--IndentationLevel;
					break;
			}
		}
		
		public override void PrintIdentifier(string identifier)
		{
			if (Keywords.GetToken(identifier) >= 0)
				PrintText("@");
			PrintText(identifier);
		}
		
		public override void PrintComment(Comment comment, bool forceWriteInPreviousBlock)
		{
			switch (comment.CommentType) {
				case CommentType.Block:
					if (forceWriteInPreviousBlock) {
						WriteInPreviousLine("/*" + comment.CommentText + "*/", forceWriteInPreviousBlock);
					} else {
						PrintSpecialText("/*" + comment.CommentText + "*/");
					}
					break;
				case CommentType.Documentation:
					WriteLineInPreviousLine("///" + comment.CommentText, forceWriteInPreviousBlock);
					break;
				default:
					WriteLineInPreviousLine("//" + comment.CommentText, forceWriteInPreviousBlock);
					break;
			}
		}
	}
}
