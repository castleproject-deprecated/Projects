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

namespace ICSharpCode.NRefactory.Parser
{
	using System;
	using System.Collections.Generic;
	using Ast;

	public abstract class AbstractParser : IParser
	{
		protected const int    MinErrDist   = 2;
		protected const string ErrMsgFormat = "-- line {0} col {1}: {2}";  // 0=line, 1=column, 2=text
		
		
		private Errors errors;
		private ILexer lexer;
		
		protected int    errDist = MinErrDist;
		
		[CLSCompliant(false)]
		protected CompilationUnit compilationUnit;
		
		bool parseMethodContents = true;
		
		public bool ParseMethodBodies {
			get {
				return parseMethodContents;
			}
			set {
				parseMethodContents = value;
			}
		}
		
		public ILexer Lexer {
			get {
				return lexer;
			}
		}
		
		public Errors Errors {
			get {
				return errors;
			}
		}
		
		public CompilationUnit CompilationUnit {
			get {
				return compilationUnit;
			}
		}
		
		internal AbstractParser(ILexer lexer)
		{
			errors = lexer.Errors;
			this.lexer  = lexer;
			errors.SynErr = new ErrorCodeProc(SynErr);
		}
		
		public abstract void Parse();
		
		public abstract TypeReference ParseTypeReference ();
		public abstract Expression ParseExpression();
		public abstract BlockStatement ParseBlock();
		public abstract List<INode> ParseTypeMembers();
		
		protected abstract void SynErr(int line, int col, int errorNumber);
		
		protected void SynErr(int n)
		{
			if (errDist >= MinErrDist) {
				errors.SynErr(lexer.LookAhead.line, lexer.LookAhead.col, n);
			}
			errDist = 0;
		}
		
		protected void SemErr(string msg)
		{
			if (errDist >= MinErrDist) {
				errors.Error(lexer.Token.line, lexer.Token.col, msg);
			}
			errDist = 0;
		}
		
		protected void Expect(int n)
		{
			if (lexer.LookAhead.kind == n) {
				lexer.NextToken();
			} else {
				SynErr(n);
			}
		}
		
		#region System.IDisposable interface implementation
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
		public void Dispose()
		{
			errors = null;
			if (lexer != null) {
				lexer.Dispose();
			}
			lexer = null;
		}
		#endregion
	}
}
