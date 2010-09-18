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

namespace ICSharpCode.NRefactory.Parser.CSharp
{
	using System.Collections.Generic;
	using Ast;
	using Visitors;

	public sealed class ConditionalCompilation : AbstractAstVisitor
	{
		static readonly object SymbolDefined = new object();
		Dictionary<string, object> symbols = new Dictionary<string, object>();
		
		public IDictionary<string, object> Symbols { 
			get { return symbols; }
		}
		
		public void Define(string symbol)
		{
			symbols[symbol] = SymbolDefined;
		}
		
		public void Undefine(string symbol)
		{
			symbols.Remove(symbol);
		}
		
		public bool Evaluate(Expression condition)
		{
			return condition.AcceptVisitor(this, null) == SymbolDefined;
		}
		
		public override object VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, object data)
		{
			if (primitiveExpression.Value is bool)
				return (bool)primitiveExpression.Value ? SymbolDefined : null;
			else
				return null;
		}
		
		public override object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			return symbols.ContainsKey(identifierExpression.Identifier) ? SymbolDefined : null;
		}
		
		public override object VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, object data)
		{
			if (unaryOperatorExpression.Op == UnaryOperatorType.Not) {
				return unaryOperatorExpression.Expression.AcceptVisitor(this, data) == SymbolDefined ? null : SymbolDefined;
			} else {
				return null;
			}
		}
		
		public override object VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			bool lhs = binaryOperatorExpression.Left.AcceptVisitor(this, data) == SymbolDefined;
			bool rhs = binaryOperatorExpression.Right.AcceptVisitor(this, data) == SymbolDefined;
			bool result;
			switch (binaryOperatorExpression.Op) {
				case BinaryOperatorType.LogicalAnd:
					result = lhs && rhs;
					break;
				case BinaryOperatorType.LogicalOr:
					result = lhs || rhs;
					break;
				case BinaryOperatorType.Equality:
					result = lhs == rhs;
					break;
				case BinaryOperatorType.InEquality:
					result = lhs != rhs;
					break;
				default:
					return null;
			}
			return result ? SymbolDefined : null;
		}
		
		public override object VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, object data)
		{
			return parenthesizedExpression.Expression.AcceptVisitor(this, data);
		}
	}
}
