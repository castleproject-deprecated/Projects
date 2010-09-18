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

namespace ICSharpCode.NRefactory.AstBuilder
{
	using System;
	using Ast;

	/// <summary>
	/// Extension methods for NRefactory.Ast.Expression.
	/// </summary>
	public static class StatementBuilder
	{
		public static void AddStatement(this BlockStatement block, Statement statement)
		{
			if (block == null)
				throw new ArgumentNullException("block");
			if (statement == null)
				throw new ArgumentNullException("statement");
			block.AddChild(statement);
			statement.Parent = block;
		}
		
		public static void AddStatement(this BlockStatement block, Expression expressionStatement)
		{
			if (expressionStatement == null)
				throw new ArgumentNullException("expressionStatement");
			AddStatement(block, new ExpressionStatement(expressionStatement));
		}
		
		public static void Throw(this BlockStatement block, Expression expression)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");
			AddStatement(block, new ThrowStatement(expression));
		}
		
		public static void Return(this BlockStatement block, Expression expression)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");
			AddStatement(block, new ReturnStatement(expression));
		}
		
		public static void Assign(this BlockStatement block, Expression left, Expression right)
		{
			if (left == null)
				throw new ArgumentNullException("left");
			if (right == null)
				throw new ArgumentNullException("right");
			AddStatement(block, new AssignmentExpression(left, AssignmentOperatorType.Assign, right));
		}
	}
}
