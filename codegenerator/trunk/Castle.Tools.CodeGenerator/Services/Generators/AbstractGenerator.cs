// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.Tools.CodeGenerator.Services.Generators
{
	using System.CodeDom;
	using System.Collections.Generic;
	using Model.TreeNodes;

	public abstract class AbstractGenerator : TreeWalker, IGenerator
	{
		protected Stack<CodeTypeDeclaration> typeStack = new Stack<CodeTypeDeclaration>();
		protected ILogger logger;
		protected ISourceGenerator source;
		protected INamingService naming;
		protected string @namespace;
		protected string serviceType;
		protected string serviceIdentifier;
		protected CodeConstructor constructor;

		public AbstractGenerator(ILogger logger, ISourceGenerator source, INamingService naming, string targetNamespace,
		                         string serviceType)
		{
			this.logger = logger;
			this.source = source;
			this.naming = naming;
			@namespace = targetNamespace;
			this.serviceType = serviceType;
			serviceIdentifier = "services";
		}

		public void Generate(TreeNode root)
		{
			Accept(root);
		}

		protected virtual CodeTypeDeclaration GenerateTypeDeclaration(string ns, string name)
		{
			var type = source.GenerateTypeDeclaration(ns, name);

			CreateMemberFields(type);

			constructor = CreateServicesConstructor();
			
			constructor.Statements.Add(
				new CodeAssignStatement(
					new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), naming.ToMemberVariableName(serviceIdentifier)),
					new CodeArgumentReferenceExpression(naming.ToVariableName(serviceIdentifier))));
			
			type.Members.Add(constructor);
			
			return type;
		}

		protected virtual void CreateMemberFields(CodeTypeDeclaration type)
		{
			var field = new CodeMemberField(source[serviceType], naming.ToMemberVariableName(serviceIdentifier)) {Attributes = MemberAttributes.Family};
			
			type.Members.Add(field);
		}

		protected virtual CodeConstructor CreateServicesConstructor()
		{
			var constructor = new CodeConstructor {Attributes = MemberAttributes.Public};
			
			constructor.Parameters.Add(new CodeParameterDeclarationExpression(serviceType, naming.ToVariableName(serviceIdentifier)));
			
			return constructor;
		}
	}
}