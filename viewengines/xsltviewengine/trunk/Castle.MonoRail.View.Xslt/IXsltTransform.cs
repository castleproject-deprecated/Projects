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
	/// Base interface for all single XsltTransformations
	/// </summary>
	public interface IXsltTransform
	{
		/// <summary>
		/// Gets the IXsltEngine that was used to created this IXsltTransform.
		/// </summary>
		/// <value>The engine.</value>
		IXsltEngine Engine { get; }

		/// <summary>
		/// Transforms the specified input to the specified output using the specified arguments.
		/// </summary>
		/// <param name="input">The input document.</param>
		/// <param name="output">The output document to write to.</param>
		/// <param name="arguments">The arguments of the transformation.</param>
		void Transform(XmlReader input, XmlWriter output, XsltTransformationArguments arguments);
	}
}