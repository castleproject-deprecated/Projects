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

namespace Castle.MonoRail.Framework.View.Xslt.NativeEngine.AdapterBuilder
{
	using System.Xml.XPath;
	using System.Collections;
	using System;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	/// <summary>
	/// Helper base-class for every dynamically built adapter
	/// </summary>
	public abstract class AdapterBase
	{
		/// <summary>
		/// Converts an IXPathNavigable to an IDictionary
		/// </summary>
		/// <param name="s">The IXPathNavigable to convert.</param>
		/// <returns>A new IDictionary containing key value pairs for each child
		/// element of the XPathNavigable</returns>
		/// <example>
		/// This XML fragment:
		/// <code>
		///    &lt;key1&gt;value1&lt;/key1&gt;
		///    &lt;key2&gt;value2&lt;/key2&gt;
		/// </code> 
		/// 
		/// is converted to a Hashtable containing two keyvaluepairs (key1,value1) and (key2, value2)
		/// </example>
		public virtual IDictionary GetDictionary(IXPathNavigable s)
		{
			Hashtable table = new Hashtable();

			XPathNavigator navigator = s.CreateNavigator();

			if (navigator.MoveToFirstChild())
			{
				do
				{
					string key = navigator.Name;
					string value = navigator.Value;
					table.Add(key, value);
				}
				while (navigator.MoveToNext(XPathNodeType.Element));
			}

			return table;
		}
	}
}
