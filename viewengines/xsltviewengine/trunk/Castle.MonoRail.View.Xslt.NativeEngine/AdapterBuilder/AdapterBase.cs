

namespace Castle.MonoRail.View.Xslt.NativeEngine.AdapterBuilder
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
