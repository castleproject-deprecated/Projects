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

using System.Linq;
using System.Text.RegularExpressions;

namespace SolutionTransform.ProjectFile
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Xml;

	public class ReferenceMapTransform : MSBuild2003Transform, IEnumerable
	{
		private Dictionary<string, IEnumerable<string>> map = new Dictionary<string, IEnumerable<string>>(StringComparer.InvariantCultureIgnoreCase);

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

        static string GetAssemblyName(string fullyQualifiedName)
        {
            int comma = fullyQualifiedName.IndexOf(',');
            if (comma > 0)
            {
                return fullyQualifiedName.Substring(0, comma);
            }
            return fullyQualifiedName;
        }

		public override void DoApplyTransform(XmlDocument document)
		{
			var source = document.SelectNodes("/*//x:Reference[@Include]", namespaces);
			foreach (XmlElement reference in source)
			{
                var original = GetAssemblyName(reference.GetAttribute("Include"));
				if (map.ContainsKey(original))  // Only change if registered
				{
					var replacement = map[original].ToList();
                    switch (replacement.Count)
                    {
                        case 0:
                            Delete(reference);
                            break;
                        case 1:
                            string replacementName = replacement[0];
                            reference.SetAttribute("Include", replacementName);
                            var hintPath = (XmlElement)reference.SelectSingleNode("x:HintPath", namespaces);
                            var path = hintPath.InnerText;
                            hintPath.InnerText = Regex.Replace(path, Regex.Escape(original), replacementName);
                            // NOTE that it's someone else's responsibility to actually rebase the DLLs
                            // Lord help me if the assembly name and the file name are different...
                            break;
                        default:
                            // This might be a bit too special case, but I think the code should
                            // stand until a better test case is encountered.
                            foreach (var item in replacement) {
                                var element = reference.OwnerDocument.CreateElement(reference.Name, reference.NamespaceURI);
                                element.SetAttribute("Include", item);
                                reference.ParentNode.InsertAfter(element, reference);
                            }
                            Delete(reference);
                            break;
                    }

					
				}
			}
		}
	}
}
