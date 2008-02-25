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


	/// <summary>
	/// Class encapsulating the argument of an xslt transformation
	/// </summary>
	public class XsltTransformationArguments
	{

		private List<XsltTransformationParameter> _parameters = new List<XsltTransformationParameter>();
		private List<object> _extensionObjects = new List<object>();

		/// <summary>
		/// Add the parameter.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="namespaceUri">The namespace URI.</param>
		/// <param name="value">The value.</param>
		public void AddParam(string name, string namespaceUri, object value)
		{
			AddParam(new XsltTransformationParameter(name, namespaceUri, value));
		}

		/// <summary>
		/// Adds the parameter.
		/// </summary>
		/// <param name="param">The parameter.</param>
		public void AddParam(XsltTransformationParameter param)
		{
			_parameters.Add(param);
		}

		public IList<XsltTransformationParameter> Parameters
		{
			get { return _parameters.AsReadOnly(); }
		}

		public IList<object> ExtensionObjects
		{
			get
			{
				return _extensionObjects.AsReadOnly();
			}
		}

		/// <summary>
		/// Adds the specified extension object.
		/// </summary>
		/// <param name="o">The object.</param>
		public void AddExtensionObject(object o)
		{
			_extensionObjects.Add(o);
		}




	}
	
}
