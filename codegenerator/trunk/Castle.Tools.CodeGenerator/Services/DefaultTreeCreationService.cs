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
	using System;
	using System.Collections.Generic;
	using Model.TreeNodes;

	public class DefaultTreeCreationService : ITreeCreationService
	{
		private readonly Stack<TreeNode> nodes = new Stack<TreeNode>();

		public DefaultTreeCreationService()
		{
			Root = new AreaTreeNode("Root");
			nodes.Push(Root);
		}

		public TreeNode Peek { get { return nodes.Peek(); } }
		public TreeNode Root { get; private set; }
		
		public void PushNode(TreeNode node)
		{
			nodes.Peek().AddChild(node);
			nodes.Push(node);
		}

		public TreeNode FindNode(string name)
		{
			foreach (var node in nodes.Peek().Children)
				if (string.Compare(node.Name, name, true) == 0)
					return node;
			
			return null;
		}

		public void PopNode()
		{
			if (nodes.Count == 1)
				throw new InvalidOperationException();

			nodes.Pop();
		}

		public void PopToRoot()
		{
			while (nodes.Count > 1)
				nodes.Pop();
		}

		public void PushArea(string name)
		{
			PushNode(FindNode(name) ?? new AreaTreeNode(name));
		}
	}
}