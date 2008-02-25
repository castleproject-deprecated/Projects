namespace Castle.MonoRail.Framework.View.Xslt
{
	using System;
	using System.IO;
	using System.Xml;
	using System.Collections.Generic;
	using System.Xml.Serialization;
	using System.Reflection;
	using System.Collections;
	using System.Diagnostics;
	using System.Text;
	using System.Xml.XPath;


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
