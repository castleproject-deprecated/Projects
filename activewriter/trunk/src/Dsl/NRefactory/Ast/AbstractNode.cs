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
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Text;

	public abstract class AbstractNode : INode
	{
		List<INode> children = new List<INode>();
		
		public INode Parent { get; set; }
		public Location StartLocation { get; set; }
		public Location EndLocation { get; set; }
		public object UserData { get; set; }
		
		public List<INode> Children {
			get {
				return children;
			}
			set {
				Debug.Assert(value != null);
				children = value;
			}
		}
		
		public virtual void AddChild(INode childNode)
		{
			Debug.Assert(childNode != null);
			children.Add(childNode);
		}
		
		public abstract object AcceptVisitor(IAstVisitor visitor, object data);
		
		public virtual object AcceptChildren(IAstVisitor visitor, object data)
		{
			foreach (INode child in children) {
				Debug.Assert(child != null);
				child.AcceptVisitor(visitor, data);
			}
			return data;
		}
		
		public static string GetCollectionString(ICollection collection)
		{
			StringBuilder output = new StringBuilder();
			output.Append('{');
			
			if (collection != null) {
				IEnumerator en = collection.GetEnumerator();
				bool isFirst = true;
				while (en.MoveNext()) {
					if (!isFirst) {
						output.Append(", ");
					} else {
						isFirst = false;
					}
					output.Append(en.Current == null ? "<null>" : en.Current.ToString());
				}
			} else {
				return "null";
			}
			
			output.Append('}');
			return output.ToString();
		}
	}
}
