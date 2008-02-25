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
	/// Base interfaces for all XsltEngines
	/// </summary>
    public interface IXsltEngine
    {
		/// <summary>
		/// Loads the transformation from the specified stream
		/// </summary>
		/// <param name="stream">The stream to load the transformation from.</param>
		/// <param name="arguments">The arguments to arguments to use in the transformation.</param>
		/// <returns></returns>
        IXsltTransform LoadTransform(Stream stream, XsltTransformationArguments arguments);

    }
}
