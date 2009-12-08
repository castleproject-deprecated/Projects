// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace SolutionTransform
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Xml;

	public abstract class MapTransformBase : MSBuild2003Transform, IEnumerable
	{
		private Dictionary<string, IEnumerable<string>> map = new Dictionary<string, IEnumerable<string>>(StringComparer.InvariantCultureIgnoreCase);
		protected abstract string ItemXPath { get; }
		protected abstract string AttributeName { get; }

		public void Add(string name)
		{
			map.Add(name, new List<string>());
		}

		public void Add(string from, params string[] to)
		{
			map.Add(from, to);
		}

		public void Add(string from, IEnumerable<string> to) {
			map.Add(from, to);
		}

		public IEnumerator GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public override void DoApplyTransform(XmlDocument document)
		{
			var source = document.SelectNodes(ItemXPath, namespaces);
			foreach (XmlElement reference in source)
			{
				var original = reference.GetAttribute(AttributeName);
				if (map.ContainsKey(original))  // Only change if registered
				{
					var replacement = map[original];
					foreach (var item in replacement)
					{
						var element = reference.OwnerDocument.CreateElement(reference.Name, reference.NamespaceURI);
						element.SetAttribute(AttributeName, item);
						reference.ParentNode.InsertAfter(element, reference);
					}
					Delete(reference);
				}
			}
		}
	}
}
