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

namespace Castle.MonoRail.Framework.View.Xslt.NativeEngine
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Xml;
	using System.Reflection;
	using System.IO;


	/// <summary>
	/// This class builds a XSLT transformation that supplies templates for
	/// all methods in registered extension objects (Helper objects) in the
	/// XslArgumentList
	/// </summary>
	internal class HelperTransformationBuilder
	{
		private static Dictionary<Type, List<XmlElement>> _typeCache = new Dictionary<Type, List<XmlElement>>();
		private static Dictionary<string, Type> _typeMap = new Dictionary<string, Type>();
		private static object _typeCacheLock = new object();
		private XmlDocument _generatingDocument = new XmlDocument();

		internal HelperTransformationBuilder()
		{

		}
		
		/// <summary>
		/// Returns a list of XmlElements belonging to the specified XmlDocument.
		/// Each XmlElement holds a template for each method in the type
		/// with the specified name.
		/// </summary>
		/// <param name="typeName">The name of the type</param>
		/// <param name="styleSheet">The XmlDocument to build XmlElements in</param>
		/// <returns>A list of the request XmlElements</returns>
		private List<XmlElement> GetTypeElements(string typeName, XmlDocument styleSheet)
		{
			//methods
			List<XmlElement> elements;

			//Get the required list of template-elements
			lock (_typeCacheLock)
			{
				Type type;
				if (!_typeMap.TryGetValue(typeName, out type))
					throw new InvalidOperationException(String.Format("Helper with name '{0}' not found", typeName));

				//Get from cache
				elements = _typeCache[type];

				//Import them into the current stylesheet
				for (int i = 0; i < elements.Count; i++)
				{
					elements[i] = styleSheet.ImportNode(elements[i], true) as XmlElement;
				}

			}
			return elements;
		}

		/// <summary>
		/// Adds the specified type to the HelperTransformationBuilder.
		/// The specified type will be analysed and XmlElements containing
		/// xsl templates for each
		/// public, declared, instance method in that type will precached.
		/// </summary>
		/// <param name="type">The type to add</param>
		public void AddType(Type type)
		{
			lock (_typeCacheLock)
			{
				if (!_typeCache.ContainsKey(type))
				{
					//Create & cache
					List<XmlElement> elements = CreateTypeElements(type);

					_typeCache.Add(type, elements);
					_typeMap.Add(type.Name.ToLowerInvariant(), type);
				}
			}
		}

		/// <summary>
		/// This method actually analyses the specified type and
		/// returns a list of XmlElements containing xsl templates 
		/// for each public, declared, instance method in the type.
		/// </summary>
		/// <param name="type">The type to analyse</param>
		/// <returns>A list of X</returns>
		private List<XmlElement> CreateTypeElements(Type type)
		{
			List<XmlElement> elements = new List<XmlElement>();

			Dictionary<string, List<MethodInfo>> dictionary = new Dictionary<string, List<MethodInfo>>();

			//Group methods by methodname in dictionary.
			foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
			{
				string key = method.Name;
				if (!dictionary.ContainsKey(key))
				{
					dictionary.Add(key, new List<MethodInfo>());
				}

				dictionary[key].Add(method);
			}

			//For each methodgroup, create a template
			foreach (string key in dictionary.Keys)
			{
				XmlElement template = CreateTemplate(type.Name, key,dictionary[key]);
				if (template != null)
					elements.Add(template);
			}
			
		
			return elements;
		}

		/// <summary>
		/// Creates an XmlElement containing an xsl template for 
		/// the specified list of MethodInfos. All MethodInfos should have
		/// the same name.
		/// </summary>
		/// <param name="typeName">The name of the type</param>
		/// <param name="name">The commen name of all methodinfos</param>
		/// <param name="methodInfos">The list of methodinfos to create a XmlElement for</param>
		/// <returns>A XmlElement in the _generatingDocument document</returns>
		private XmlElement CreateTemplate(string typeName, string name, List<MethodInfo> methodInfos)
		{
			XmlAttribute methodName = _generatingDocument.CreateAttribute("name");
			//Todo: maybe delete the 'Helper'-part form the typeName
			methodName.InnerText = typeName + "-" + name;

			XmlElement template = _generatingDocument.CreateElement("xsl", "template", "http://www.w3.org/1999/XSL/Transform");
			template.Attributes.Append(methodName);

			List<string> parameters = new List<string>();
			XmlElement choiceElement = _generatingDocument.CreateElement("xsl", "choose", "http://www.w3.org/1999/XSL/Transform");
			template.AppendChild(choiceElement);
			//Build an xsl:if and xsl:value-of statement for each methodinfo.
			foreach (MethodInfo methodInfo in methodInfos)
			{
				StringBuilder testString = new StringBuilder();
				StringBuilder valueOf = new StringBuilder();
				valueOf.Append("<xsl:value-of select='");
				valueOf.Append(methodInfo.DeclaringType.Name);
				valueOf.Append(":");
				valueOf.Append(methodInfo.Name);
				valueOf.Append("(");


				ParameterInfo[] mParameters = methodInfo.GetParameters();
				for (int i = 0; i < mParameters.Length; i++)
				{
					string mParametersName = mParameters[i].Name;
					if (!parameters.Contains(mParametersName))
					{
						parameters.Add(mParametersName);
					}
					//xsl:if test
					testString.Append("$");
					testString.Append(mParametersName);
					if (i != mParameters.Length - 1)
						testString.Append(" and ");

					//xsl:value-of
					valueOf.Append("$");
					valueOf.Append(mParametersName);
					if (i != mParameters.Length - 1)
						valueOf.Append(",");
					
				}
				
				XmlElement methodElement = _generatingDocument.CreateElement("xsl", "when", "http://www.w3.org/1999/XSL/Transform");
				XmlAttribute testAttribute = _generatingDocument.CreateAttribute("test");
				testAttribute.Value = testString.ToString();
				methodElement.Attributes.Append(testAttribute);
				choiceElement.AppendChild(methodElement);

				//Hack because i couldn't find out how to do it with the dom.		
				valueOf.Append(")' xmlns:");
				valueOf.Append(methodInfo.DeclaringType.Name);
				valueOf.Append("='urn:");
				valueOf.Append(methodInfo.DeclaringType.Name);
				valueOf.Append("' xmlns:xsl='http://www.w3.org/1999/XSL/Transform' disable-output-escaping='yes'/>");

				methodElement.InnerXml = valueOf.ToString();

				//If there is only one method, skip the xsl:if statement
				if (methodInfos.Count == 1)
					template.InnerXml = valueOf.ToString();
			}		
			
			//Insert xsl:param declarations at the top of the template
			XmlNode valueOfNode = template.ChildNodes[0];
			foreach (string parameter in parameters)
			{
				XmlElement param = _generatingDocument.CreateElement("xsl", "param", "http://www.w3.org/1999/XSL/Transform");
				XmlAttribute paramName = _generatingDocument.CreateAttribute("name");
				paramName.InnerText = parameter;
				param.Attributes.Append(paramName);
				template.InsertBefore(param, valueOfNode);
			}

			return template;
		}

		/// <summary>
		/// Starts a new empty stylesheet
		/// </summary>
		/// <param name="styleSheet">The stylesheet document, containing a xsl:stylesheet element</param>
		/// <param name="styleSheetRoot">The xsl:stylesheet root element.</param>
		private static void StartStyleSheet(out XmlDocument styleSheet, out XmlElement styleSheetRoot)
		{
			styleSheet = new XmlDocument();
			styleSheet.AppendChild(styleSheet.CreateXmlDeclaration("1.0", "UTF-8", "yes"));
			styleSheetRoot = styleSheet.CreateElement("xsl", "stylesheet", "http://www.w3.org/1999/XSL/Transform");
			XmlAttribute version = styleSheet.CreateAttribute("version");
			version.InnerText = "1.0";
			styleSheetRoot.Attributes.Append(version);
			styleSheet.AppendChild(styleSheetRoot);
		}

		/// <summary>
		/// Adds the templates that have been cached for the 
		/// specified typename to the specified xmldocument.
		/// </summary>
		/// <param name="typeName">The type to add the templates for</param>
		/// <param name="styleSheet">The stylesheet to add the templates to</param>
		/// <param name="styleSheetRoot">The element in the stylesheet to add the elements to</param>
		private void AddTypeTemplates(string typeName, XmlDocument styleSheet, XmlElement styleSheetRoot)
		{
			foreach (XmlElement element in GetTypeElements(typeName.Trim().ToLowerInvariant(), styleSheet))
			{
				styleSheetRoot.AppendChild(element);
			}
		}

		/// <summary>
		/// Returns a stylesheet containing the necessary
		/// xsl templates for the helper with the specified 
		/// typename.
		/// </summary>
		/// <param name="typeName">The name of the type to build the stylesheet for</param>
		/// <returns>A stream of the generated stylesheet.</returns>
		public Stream GetHelperStylesheet(string typeName)
		{
			XmlDocument styleSheet;
			XmlElement styleSheetRoot;
			StartStyleSheet(out styleSheet, out styleSheetRoot);

			AddTypeTemplates(typeName, styleSheet, styleSheetRoot);

			MemoryStream stream = new MemoryStream();
			styleSheet.Save(stream);
			stream.Position = 0;
			return stream;
		}

		internal XmlDocument GetHelperStylesheetFor(IList<string> objects)
		{
			XmlDocument styleSheet;
			XmlElement styleSheetRoot;
			StartStyleSheet(out styleSheet, out styleSheetRoot);

			foreach (string s in objects)
			{
				AddTypeTemplates(s, styleSheet, styleSheetRoot);
			}

			return styleSheet;
		}
	}
}
