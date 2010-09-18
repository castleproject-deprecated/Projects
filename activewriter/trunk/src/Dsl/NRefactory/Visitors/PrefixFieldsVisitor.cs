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
	using System.Collections.Generic;
	using Ast;

	/// <summary>
	/// Prefixes the names of the specified fields with the prefix and replaces the use.
	/// </summary>
	public class PrefixFieldsVisitor : AbstractAstVisitor
	{
		List<VariableDeclaration> fields;
		List<string> curBlock = new List<string>();
		Stack<List<string>> blocks = new Stack<List<string>>();
		string prefix;
		
		public PrefixFieldsVisitor(List<VariableDeclaration> fields, string prefix)
		{
			this.fields = fields;
			this.prefix = prefix;
		}
		
		public void Run(INode typeDeclaration)
		{
			typeDeclaration.AcceptVisitor(this, null);
			foreach (VariableDeclaration decl in fields) {
				decl.Name = prefix + decl.Name;
			}
		}
		
		public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			Push();
			object result = base.VisitTypeDeclaration(typeDeclaration, data);
			Pop();
			return result;
		}
		
		public override object VisitBlockStatement(BlockStatement blockStatement, object data)
		{
			Push();
			object result = base.VisitBlockStatement(blockStatement, data);
			Pop();
			return result;
		}
		
		public override object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			Push();
			object result = base.VisitMethodDeclaration(methodDeclaration, data);
			Pop();
			return result;
		}
		
		public override object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
		{
			Push();
			object result = base.VisitPropertyDeclaration(propertyDeclaration, data);
			Pop();
			return result;
		}
		
		public override object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			Push();
			object result = base.VisitConstructorDeclaration(constructorDeclaration, data);
			Pop();
			return result;
		}
		
		private void Push()
		{
			blocks.Push(curBlock);
			curBlock = new List<string>();
		}
		
		private void Pop()
		{
			curBlock = blocks.Pop();
		}
		
		public override object VisitVariableDeclaration(VariableDeclaration variableDeclaration, object data)
		{
			// process local variables only
			if (fields.Contains(variableDeclaration)) {
				return null;
			}
			curBlock.Add(variableDeclaration.Name);
			return base.VisitVariableDeclaration(variableDeclaration, data);
		}
		
		public override object VisitParameterDeclarationExpression(ParameterDeclarationExpression parameterDeclarationExpression, object data)
		{
			curBlock.Add(parameterDeclarationExpression.ParameterName);
			//print("add parameter ${parameterDeclarationExpression.ParameterName} to block")
			return base.VisitParameterDeclarationExpression(parameterDeclarationExpression, data);
		}
		
		public override object VisitForeachStatement(ForeachStatement foreachStatement, object data)
		{
			curBlock.Add(foreachStatement.VariableName);
			return base.VisitForeachStatement(foreachStatement, data);
		}
		
		public override object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			string name = identifierExpression.Identifier;
			foreach (VariableDeclaration var in fields) {
				if (var.Name == name && !IsLocal(name)) {
					identifierExpression.Identifier = prefix + name;
					break;
				}
			}
			return base.VisitIdentifierExpression(identifierExpression, data);
		}
		
		public override object VisitMemberReferenceExpression(MemberReferenceExpression fieldReferenceExpression, object data)
		{
			if (fieldReferenceExpression.TargetObject is ThisReferenceExpression) {
				string name = fieldReferenceExpression.MemberName;
				foreach (VariableDeclaration var in fields) {
					if (var.Name == name) {
						fieldReferenceExpression.MemberName = prefix + name;
						break;
					}
				}
			}
			return base.VisitMemberReferenceExpression(fieldReferenceExpression, data);
		}
		
		bool IsLocal(string name)
		{
			foreach (List<string> block in blocks) {
				if (block.Contains(name))
					return true;
			}
			return curBlock.Contains(name);
		}
	}
}
