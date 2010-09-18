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

namespace ICSharpCode.NRefactory.Ast
{
	using System;
	using System.Collections;

	public class CompilationUnit : AbstractNode
	{
		// Children in C#: UsingAliasDeclaration, UsingDeclaration, AttributeSection, NamespaceDeclaration
		// Children in VB: OptionStatements, ImportsStatement, AttributeSection, NamespaceDeclaration
		
		Stack blockStack = new Stack();
		
		public CompilationUnit()
		{
			blockStack.Push(this);
		}
		
		public void BlockStart(INode block)
		{
			blockStack.Push(block);
		}
		
		public void BlockEnd()
		{
			blockStack.Pop();
		}
		
		public INode CurrentBock {
			get {
				return blockStack.Count > 0 ? (INode)blockStack.Peek() : null;
			}
		}
		
		public override void AddChild(INode childNode)
		{
			if (childNode != null) {
				INode parent = (INode)blockStack.Peek();
				parent.Children.Add(childNode);
				childNode.Parent = parent;
			}
		}
		
		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitCompilationUnit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[CompilationUnit]");
		}
	}
}
