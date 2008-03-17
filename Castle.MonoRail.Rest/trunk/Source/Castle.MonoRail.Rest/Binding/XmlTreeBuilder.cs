// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Rest.Binding
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.IO;
	using Castle.Components.Binder;
	using System.Xml.Linq;
	using System.Xml;

	public class XmlTreeBuilder
	{
	   
		public CompositeNode BuildNode(XDocument doc)
		{
			var rootNode = new CompositeNode("root");
			rootNode.AddChildNode(ProcessElement(doc.Root));
			return rootNode;

		}

		public void AddToRoot(CompositeNode rootNode, XDocument doc)
		{
			var top = ProcessElement(doc.Root);
			rootNode.AddChildNode(top);
		}

		public Node ProcessElement(XElement startEl)
		{
			if (IsComplex(startEl))
			{
				CompositeNode top = new CompositeNode(startEl.Name.LocalName);
				foreach (var attr in startEl.Attributes())
				{
					var leaf = new LeafNode(typeof(String), attr.Name.LocalName, attr.Value);
					top.AddChildNode(leaf);
				}
				foreach (var childEl in startEl.Elements())
				{
					var childNode = ProcessElement(childEl);
					top.AddChildNode(childNode);
				}

				return top;
			}
			else
			{
				LeafNode top = new LeafNode(typeof(String), "", "");
				return top;
			}

			
		}

		public bool IsComplex(XElement element)
		{
			if (element.HasElements || element.Attributes().Count() > 0)
			{
				if (element.Elements().Count() == 1 && element.FirstNode.NodeType == XmlNodeType.Text)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			else
			{
				return false;
			}
		}
	}
}
