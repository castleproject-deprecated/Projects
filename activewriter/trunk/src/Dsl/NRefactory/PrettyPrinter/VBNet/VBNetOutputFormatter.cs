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
	using System;
	using Parser.VB;

	/// <summary>
	/// Description of VBNetOutputFormatter.
	/// </summary>
	public sealed class VBNetOutputFormatter : AbstractOutputFormatter
	{
		public VBNetOutputFormatter(VBNetPrettyPrintOptions prettyPrintOptions) : base(prettyPrintOptions)
		{
		}
		
		public override void PrintToken(int token)
		{
			PrintText(Tokens.GetTokenString(token));
		}
		
		public override void PrintIdentifier(string identifier)
		{
			if (Keywords.IsNonIdentifierKeyword(identifier)) {
				PrintText("[");
				PrintText(identifier);
				PrintText("]");
			} else {
				PrintText(identifier);
			}
		}
		
		public override void PrintComment(Comment comment, bool forceWriteInPreviousBlock)
		{
			switch (comment.CommentType) {
				case CommentType.Block:
					WriteLineInPreviousLine("'" + comment.CommentText.Replace("\n", "\n'"), forceWriteInPreviousBlock);
					break;
				case CommentType.Documentation:
					WriteLineInPreviousLine("'''" + comment.CommentText, forceWriteInPreviousBlock);
					break;
				default:
					WriteLineInPreviousLine("'" + comment.CommentText, forceWriteInPreviousBlock);
					break;
			}
		}
		
		public override void PrintPreprocessingDirective(PreprocessingDirective directive, bool forceWriteInPreviousBlock)
		{
			if (IsInMemberBody
			    && (string.Equals(directive.Cmd, "#Region", StringComparison.InvariantCultureIgnoreCase)
			        || string.Equals(directive.Cmd, "#End", StringComparison.InvariantCultureIgnoreCase)
			        && directive.Arg.StartsWith("Region", StringComparison.InvariantCultureIgnoreCase)))
			{
				WriteLineInPreviousLine("'" + directive.Cmd + " " + directive.Arg, forceWriteInPreviousBlock);
			} else if (!directive.Expression.IsNull) {
				VBNetOutputVisitor visitor = new VBNetOutputVisitor();
				directive.Expression.AcceptVisitor(visitor, null);
				WriteLineInPreviousLine(directive.Cmd + " " + visitor.Text + " Then", forceWriteInPreviousBlock);
			} else {
				base.PrintPreprocessingDirective(directive, forceWriteInPreviousBlock);
			}
		}
		
		public void PrintLineContinuation()
		{
			if (!LastCharacterIsWhiteSpace)
				Space();
			PrintText("_" + Environment.NewLine);
		}
	}
}
