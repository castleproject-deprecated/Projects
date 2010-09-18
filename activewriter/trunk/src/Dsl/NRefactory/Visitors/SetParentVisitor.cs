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
	/// Sets the parent property on all nodes in the tree.
	/// </summary>
	public class SetParentVisitor : NodeTrackingAstVisitor
	{
		Stack<INode> nodeStack = new Stack<INode>();
		
		public SetParentVisitor()
		{
			nodeStack.Push(null);
		}
		
		protected override void BeginVisit(INode node)
		{
			node.Parent = nodeStack.Peek();
			nodeStack.Push(node);
		}
		
		protected override void EndVisit(INode node)
		{
			nodeStack.Pop();
		}
	}
}
