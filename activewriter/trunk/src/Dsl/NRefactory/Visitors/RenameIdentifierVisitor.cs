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

namespace ICSharpCode.NRefactory.Visitors
{
	using System;
	using Ast;

	class RenameIdentifierVisitor : AbstractAstVisitor
	{
		protected StringComparer nameComparer;
		protected string from, to;
		
		public RenameIdentifierVisitor(string from, string to, StringComparer nameComparer)
		{
			this.nameComparer = nameComparer;
			this.from = from;
			this.to = to;
		}
		
		public override object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			if (nameComparer.Equals(identifierExpression.Identifier, from)) {
				identifierExpression.Identifier = to;
			}
			return base.VisitIdentifierExpression(identifierExpression, data);
		}
	}
	
	sealed class RenameLocalVariableVisitor : RenameIdentifierVisitor
	{
		public RenameLocalVariableVisitor(string from, string to, StringComparer nameComparer)
			: base(from, to, nameComparer)
		{
		}
		
		public override object VisitVariableDeclaration(VariableDeclaration variableDeclaration, object data)
		{
			if (nameComparer.Equals(from, variableDeclaration.Name)) {
				variableDeclaration.Name = to;
			}
			return base.VisitVariableDeclaration(variableDeclaration, data);
		}
		
		public override object VisitParameterDeclarationExpression(ParameterDeclarationExpression parameterDeclarationExpression, object data)
		{
			if (nameComparer.Equals(from, parameterDeclarationExpression.ParameterName)) {
				parameterDeclarationExpression.ParameterName = to;
			}
			return base.VisitParameterDeclarationExpression(parameterDeclarationExpression, data);
		}
		
		public override object VisitForeachStatement(ForeachStatement foreachStatement, object data)
		{
			if (nameComparer.Equals(from, foreachStatement.VariableName)) {
				foreachStatement.VariableName = to;
			}
			return base.VisitForeachStatement(foreachStatement, data);
		}
	}
}
