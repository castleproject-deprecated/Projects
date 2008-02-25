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
	/// Class encapsulating a single transformation parameter
	/// </summary>
	public class XsltTransformationParameter
	{
		/// <summary>
		/// Initializes a new instance of the XsltTransformationParameter class.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="namespaceUri"></param>
		/// <param name="value"></param>
		public XsltTransformationParameter(string name, string namespaceUri, object value)
		{
			_Name = name;
			_NameSpace = namespaceUri;
			_Value = value;
		}

		private string _Name;
		public string Name
		{
			get { return _Name; }
			private set
			{
				_Name = value;
			}
		}

		private string _NameSpace;
		public string NameSpace
		{
			get { return _NameSpace; }
			private set
			{
				_NameSpace = value;
			}
		}

		private object _Value;
		public object Value
		{
			get { return _Value; }
			private set
			{
				_Value = value;
			}
		}
	}
}
