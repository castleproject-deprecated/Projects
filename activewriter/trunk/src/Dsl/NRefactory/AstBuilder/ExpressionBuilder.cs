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
	using System.Collections.Generic;
	using Ast;

	//#if NET40
	/// <summary>
	/// Extension methods for NRefactory.Ast.Expression.
	/// </summary>
	public static class ExpressionBuilder
	{
		public static IdentifierExpression Identifier(string identifier)
		{
			return new IdentifierExpression(identifier);
		}
		
		public static MemberReferenceExpression Member(this Expression targetObject, string memberName)
		{
			if (targetObject == null)
				throw new ArgumentNullException("targetObject");
			return new MemberReferenceExpression(targetObject, memberName);
		}
		
		public static InvocationExpression Call(this Expression callTarget, string methodName, params Expression[] arguments)
		{
			if (callTarget == null)
				throw new ArgumentNullException("callTarget");
			return Call(Member(callTarget, methodName), arguments);
		}
		
		public static InvocationExpression Call(this Expression callTarget, params Expression[] arguments)
		{
			if (callTarget == null)
				throw new ArgumentNullException("callTarget");
			if (arguments == null)
				throw new ArgumentNullException("arguments");
			return new InvocationExpression(callTarget, new List<Expression>(arguments));
		}
		
		public static ObjectCreateExpression New(this TypeReference createType, params Expression[] arguments)
		{
			if (createType == null)
				throw new ArgumentNullException("createType");
			if (arguments == null)
				throw new ArgumentNullException("arguments");
			return new ObjectCreateExpression(createType, new List<Expression>(arguments));
		}
		
		public static Expression CreateDefaultValueForType(TypeReference type)
		{
			if (type != null && !type.IsArrayType) {
				switch (type.Type) {
					case "System.SByte":
					case "System.Byte":
					case "System.Int16":
					case "System.UInt16":
					case "System.Int32":
					case "System.UInt32":
					case "System.Int64":
					case "System.UInt64":
					case "System.Single":
					case "System.Double":
						return new PrimitiveExpression(0, "0");
					case "System.Char":
						return new PrimitiveExpression('\0', "'\\0'");
					case "System.Object":
					case "System.String":
						return new PrimitiveExpression(null, "null");
					case "System.Boolean":
						return new PrimitiveExpression(false, "false");
					default:
						return new DefaultValueExpression(type);
				}
			} else {
				return new PrimitiveExpression(null, "null");
			}
		}
		
		/// <summary>
		/// Just calls the BinaryOperatorExpression constructor,
		/// but being an extension method; this allows for a nicer
		/// infix syntax in some cases.
		/// </summary>
		public static BinaryOperatorExpression Operator(this Expression left, BinaryOperatorType op, Expression right)
		{
			return new BinaryOperatorExpression(left, op, right);
		}
	}
	//#endif
}
