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

namespace Castle.Tools.CodeGenerator.Model.TreeNodes
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	public class TreeNode
	{
		public static readonly string RootName = "Root";

		public TreeNode(string name)
		{
			Name = name;
			Children = new List<TreeNode>();
		}

		public List<TreeNode> Children { get; private set; }
		public string Name { get; private set; }
		public TreeNode Parent { get; protected set; }

		public string Path
		{
			get { return CalculatePath(n => n == null); }
		}

		public string PathNoSlashes
		{
			get { return Path.Replace("/", ""); }
		}

		public void AddChild(TreeNode node)
		{
			AddChild(node, false);
		}

		public void AddChild(TreeNode node, bool force)
		{
			if (!force && Children.Contains(node))
				return;
			
			node.Parent = this;
			Children.Add(node);
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var node = obj as TreeNode;
			
			if (node == null)
				return base.Equals(obj);
			
			if (node.GetType() != GetType())
				return base.Equals(obj);
			
			return node.Name.Equals(Name);
		}

		public override string ToString()
		{
			return String.Format("Node<{0}>", Name);
		}

		protected string CalculatePath(Func<TreeNode, bool> nodeIteratorStopCondition)
		{
			var parts = new List<string>();
			var node = Parent;
			var sb = new StringBuilder();

			while (!nodeIteratorStopCondition(node))
			{
				parts.Add(node.Name);
				node = node.Parent;
			}

			parts.Reverse();

			foreach (var part in parts)
			{
				if (sb.Length != 0)
					sb.Append("/");

				sb.Append(part);
			}

			return sb.ToString();
		}
	}
}