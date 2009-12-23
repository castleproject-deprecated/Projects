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

namespace SolutionTransform.ProjectFile
{
	using System.Xml;

	public abstract class MSBuild2003Transform : ITransform
	{
		protected static string build2003 = "http://schemas.microsoft.com/developer/msbuild/2003";
		protected XmlNamespaceManager namespaces = null;

        public void ApplyTransform(XmlFile xmlFile)
		{
			namespaces = new XmlNamespaceManager(xmlFile.Document.NameTable);
			namespaces.AddNamespace("x", build2003);
			DoApplyTransform(xmlFile);
		}

		public virtual void DoApplyTransform(XmlFile xmlFile)
		{
            DoApplyTransform(xmlFile.Document);
		}

		public abstract void DoApplyTransform(XmlDocument document);


		protected XmlElement SetElement(XmlNode parent, string name, string value)
		{
			var element = (XmlElement)parent.SelectSingleNode("x:" + name, namespaces);
			if (element == null) {
				return AddElement(parent, name, value);
			}
			element.InnerText = value;
			return element;
		}

		protected XmlElement SetElement(XmlNode parent, string name, bool value) {
			return SetElement(parent, name, value ? "true" : "false");
			
		}

		protected XmlElement AddElement(XmlNode parent, string name, bool value) {
			return AddElement(parent, name, value ? "true" : "false");
		}

		protected XmlElement AddElement(XmlNode parent, string name, string value) {
			var result = AddElement(parent, name);
			result.InnerText = value;
			return result;
		}

		protected XmlElement AddElement(XmlNode parent, string name) {
			var result = parent.OwnerDocument.CreateElement(name, build2003);
			parent.AppendChild(result);
			return result;
		}

		protected void Delete(XmlNode parent, string xpath) {
			foreach (XmlNode node in parent.SelectNodes(xpath, namespaces)) {
				Delete(node);
			}
		}

		protected void Delete(XmlNode node)
		{
			node.ParentNode.RemoveChild(node);
		}
	}
}
