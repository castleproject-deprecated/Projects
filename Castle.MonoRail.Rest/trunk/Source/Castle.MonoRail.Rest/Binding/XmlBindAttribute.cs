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

using System.Reflection;

namespace Castle.MonoRail.Rest.Binding
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Xml;
	using System.IO;
	using System.Xml.XPath;
	using Castle.MonoRail.Framework;
	using System.Xml.Linq;
	using System.Xml.Serialization;

	public class XmlBindAttribute : System.Attribute, IParameterBinder
	{
		private Dictionary<Type,Func<Stream,Object>> _factories;

		public XmlBindAttribute() {

			_factories = new Dictionary<Type, Func<Stream, object>>();

			_factories[typeof(XmlReader)] = inputStream => XmlReader.Create(inputStream);
			_factories[typeof(String)] = inputStream => new StreamReader(inputStream).ReadToEnd();
			_factories[typeof(XPathNavigator)] = inputStream => { var doc = new XPathDocument(inputStream); return doc.CreateNavigator(); };
			_factories[typeof(XDocument)] = inputStream => XDocument.Load(XmlReader.Create(inputStream)); 
		}

		public Object CreateValueFromInputStream(Type valueType, Stream inputStream)
		{
			if (_factories.ContainsKey(valueType))
			{
				return _factories[valueType](inputStream);
			}
			else
			{
				XmlSerializer serial = new XmlSerializer(valueType);
				return serial.Deserialize(inputStream);
			}
			
		}

		#region IParameterBinder Members

		public object Bind(IEngineContext context, IController controller, IControllerContext controllerContext,
		                   ParameterInfo parameterInfo)
		{
			var inputStream = context.Request.InputStream;
			return CreateValueFromInputStream(parameterInfo.ParameterType, inputStream);
		}

		public int CalculateParamPoints(IEngineContext context, IController controller, IControllerContext controllerContext,
		                                ParameterInfo parameterInfo)
		{
			return 10; 
		}

		#endregion
	}
}
