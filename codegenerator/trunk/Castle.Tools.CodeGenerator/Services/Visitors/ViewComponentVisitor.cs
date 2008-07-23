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

namespace Castle.Tools.CodeGenerator.Services.Visitors
{
	using Model.TreeNodes;
	using Visitors;
	using ICSharpCode.NRefactory.Ast;

	public class ViewComponentVisitor : TypeResolvingVisitor
	{
		private readonly ITreeCreationService treeService;

		public ViewComponentVisitor(ILogger logger, ITypeResolver typeResolver, ITreeCreationService treeService)
			: base(typeResolver)
		{
			this.treeService = treeService;
		}

		public override object VisitCompilationUnit(CompilationUnit compilationUnit, object data)
		{
			treeService.PushArea("Components");
			base.VisitCompilationUnit(compilationUnit, data);
			treeService.PopNode();
			
			return base.VisitCompilationUnit(compilationUnit, data);
		}

		public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			if (!IsViewComponent(typeDeclaration))
				return null;
			
			var typeNamespace = GetNamespace(typeDeclaration);
			var node = new ViewComponentTreeNode(typeDeclaration.Name, typeNamespace);
			treeService.PushNode(node);

			var r = base.VisitTypeDeclaration(typeDeclaration, data);
			treeService.PopNode();

			return r;
		}

		protected virtual bool IsViewComponent(TypeDeclaration typeDeclaration)
		{
			return typeDeclaration.Name.EndsWith("Component");
		}
	}
}