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

namespace Castle.Tools.CodeGenerator.Services
{
	using System.Collections.Generic;
	using Model.TreeNodes;

	public class TreeWalker
	{
		public virtual void Accept(TreeNode node)
		{
			var type = node.GetType();
			var method = GetType().GetMethod("Visit", new[] {type});
			
			if (method != null)
				method.Invoke(this, new object[] {node});
		}

		public virtual void Accept(ICollection<TreeNode> nodes)
		{
			foreach (var child in nodes)
				Accept(child);
		}

		public virtual void Visit(AreaTreeNode node)
		{
			Accept(node.Children);
		}

		public virtual void Visit(ControllerTreeNode node)
		{
			Accept(node.Children);
		}

		public virtual void Visit(ActionTreeNode node)
		{
			Accept(node.Children);
		}

		public virtual void Visit(StaticRouteTreeNode node)
		{
			Accept(node.Children);
		}

		public virtual void Visit(PatternRouteTreeNode node)
		{
			Accept(node.Children);
		}

		public virtual void Visit(RestRouteTreeNode node)
		{
			Accept(node.Children);
		}

		public virtual void Visit(ViewComponentTreeNode node)
		{
			Accept(node.Children);
		}

		public virtual void Visit(ViewTreeNode node)
		{
			Accept(node.Children);
		}

		public virtual void Visit(WizardControllerTreeNode node)
		{
			Accept(node.Children);
		}
	}
}