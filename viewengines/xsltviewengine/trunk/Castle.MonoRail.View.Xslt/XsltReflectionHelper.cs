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


namespace Castle.MonoRail.Framework.View.Xslt
{
	using System;
	using System.Collections.Generic;
	using System.Xml.XPath;
	using System.Xml;
	using System.Reflection;
	using System.ComponentModel;

	public class XsltReflectionHelper
	{
		private static Dictionary<string, XmlDocument> _ReflectionCache = new Dictionary<string, XmlDocument>();
		private static object _SyncObject = new object();
		private static XmlDocument GetFromCache(string typeName)
		{
			lock (_SyncObject)
			{
				XmlDocument result;
				if (!_ReflectionCache.TryGetValue(typeName, out result))
				{
					return null;
				}
				return result;
			}
		}

		private static void StoreInCache(string typeName, XmlDocument doc)
		{
			lock (_SyncObject)
			{
				_ReflectionCache[typeName] = doc;
			}
		}

		public IXPathNavigable ReflectType(string typeName)
		{
			XmlDocument result = GetFromCache(typeName);

			if (result == null)
			{
				result = BuildXmlDocumentForType(typeName);
			}
			return result;

		}

		private static XmlDocument BuildXmlDocumentForType(string typeName)
		{
			XmlDocument result;
			Type type = Type.GetType(typeName);
			if (type == null) throw new ArgumentException(String.Format("Type '{0}' could not be found", typeName));
			result = new XmlDocument();
			XmlElement root = result.CreateElement("type");
			AddAttribute(root, "fullname", type.FullName);
			AddAttribute(root, "name", type.Name);
			AddAttribute(root, "namespace", type.Namespace);

			foreach (PropertyInfo propInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				XmlElement propertyNode = result.CreateElement("property");
				AddAttribute(propertyNode, "typeName", propInfo.PropertyType.Name);
				AddAttribute(propertyNode, "name", propInfo.Name);
				object[] nameAttributes = propInfo.GetCustomAttributes(typeof(DisplayNameAttribute), false);
				if (nameAttributes.Length == 1)
				{
					string name = (nameAttributes[0] as DisplayNameAttribute).DisplayName;
					AddAttribute(propertyNode, "displayName", name);
				}
				else
				{
					AddAttribute(propertyNode, "displayName", propInfo.Name);
				}
				AddAttribute(propertyNode, "canWrite", propInfo.CanWrite.ToString());
				AddAttribute(propertyNode, "canRead", propInfo.CanRead.ToString());
				root.AppendChild(propertyNode);
			}
			result.AppendChild(root);
			return result;
		}
		private static void AddAttribute(XmlElement element, string name, string value)
		{
			XmlAttribute attribute = element.OwnerDocument.CreateAttribute(name);
			attribute.Value = value;
			element.Attributes.Append(attribute);
		}
	}
}
