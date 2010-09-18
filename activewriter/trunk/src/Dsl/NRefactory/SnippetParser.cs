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

namespace ICSharpCode.NRefactory
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using Ast;
	using Parser;

	public enum SnippetType
	{
		None,
		CompilationUnit,
		Expression,
		Statements,
		TypeMembers
	}
	
	/// <summary>
	/// The snippet parser supports parsing code snippets that are not valid as a full compilation unit.
	/// </summary>
	public class SnippetParser
	{
		readonly SupportedLanguage language;
		
		public SnippetParser(SupportedLanguage language)
		{
			this.language = language;
		}
		
		/// <summary>
		/// Gets the errors of the last call to Parse(). Returns null if parse was not yet called.
		/// </summary>
		public Errors Errors { get; private set; }
		
		/// <summary>
		/// Gets the specials of the last call to Parse(). Returns null if parse was not yet called.
		/// </summary>
		public List<ISpecial> Specials { get; private set; }
		
		/// <summary>
		/// Gets the snippet type of the last call to Parse(). Returns None if parse was not yet called.
		/// </summary>
		public SnippetType SnippetType { get; private set; }
		
		/// <summary>
		/// Parse the code. The result may be a CompilationUnit, an Expression, a list of statements or a list of class
		/// members.
		/// </summary>
		public INode Parse(string code)
		{
			IParser parser = ParserFactory.CreateParser(language, new StringReader(code));
			parser.Parse();
			Errors = parser.Errors;
			Specials = parser.Lexer.SpecialTracker.RetrieveSpecials();
			SnippetType = SnippetType.CompilationUnit;
			INode result = parser.CompilationUnit;
			
			if (Errors.Count > 0) {
				if (language == SupportedLanguage.CSharp) {
					// SEMICOLON HACK : without a trailing semicolon, parsing expressions does not work correctly
					parser = ParserFactory.CreateParser(language, new StringReader(code + ";"));
				} else {
					parser = ParserFactory.CreateParser(language, new StringReader(code));
				}
				Expression expression = parser.ParseExpression();
				if (expression != null && parser.Errors.Count < Errors.Count) {
					Errors = parser.Errors;
					Specials = parser.Lexer.SpecialTracker.RetrieveSpecials();
					SnippetType = SnippetType.Expression;
					result = expression;
				}
			}
			if (Errors.Count > 0) {
				parser = ParserFactory.CreateParser(language, new StringReader(code));
				BlockStatement block = parser.ParseBlock();
				if (block != null && parser.Errors.Count < Errors.Count) {
					Errors = parser.Errors;
					Specials = parser.Lexer.SpecialTracker.RetrieveSpecials();
					SnippetType = SnippetType.Statements;
					result = block;
				}
			}
			if (Errors.Count > 0) {
				parser = ParserFactory.CreateParser(language, new StringReader(code));
				List<INode> members = parser.ParseTypeMembers();
				if (members != null && members.Count > 0 && parser.Errors.Count < Errors.Count) {
					Errors = parser.Errors;
					Specials = parser.Lexer.SpecialTracker.RetrieveSpecials();
					SnippetType = SnippetType.TypeMembers;
					result = new NodeListNode(members);
					result.StartLocation = members[0].StartLocation;
					result.EndLocation = members[members.Count - 1].EndLocation;
				}
			}
			Debug.Assert(result is CompilationUnit || !result.StartLocation.IsEmpty);
			Debug.Assert(result is CompilationUnit || !result.EndLocation.IsEmpty);
			return result;
		}
		
		sealed class NodeListNode : INode
		{
			List<INode> nodes;
			
			public NodeListNode(List<INode> nodes)
			{
				this.nodes = nodes;
			}
			
			public INode Parent {
				get { return null; }
				set { throw new NotSupportedException(); }
			}
			
			public List<INode> Children {
				get { return nodes; }
			}
			
			public Location StartLocation { get; set; }
			public Location EndLocation { get; set; }
			
			public object UserData { get; set; }
			
			public object AcceptChildren(IAstVisitor visitor, object data)
			{
				foreach (INode n in nodes) {
					n.AcceptVisitor(visitor, data);
				}
				return null;
			}
			
			public object AcceptVisitor(IAstVisitor visitor, object data)
			{
				return AcceptChildren(visitor, data);
			}
		}
	}
}
