namespace Castle.MonoRail.View.Xslt.NativeEngine
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using Castle.MonoRail.Framework.View.Xslt;
	using System.Xml.Xsl;
	using System.Xml;
	using System.Reflection;
	using System.IO;
	using System.Xml.XPath;
	/// <summary>
	/// This class resolves url references of the form helper://[typeName]
	/// to a xsl stylesheet containing templates for all methods in the
	/// specified typeName using the HelperTransformationBuilder specified
	/// at construction. The typeName should be registered with AddType to
	/// the HelperTransformationBuilder prior to trying to result a certain
	/// typeName using this Resolver.
	/// </summary>
	internal class HelperTransformationResolver : XmlUrlResolver
	{
		/// <summary>
		/// Initializes a new instance of the HelperTransformationResolver class.
		/// </summary>
		/// <param name="helpers"></param>
		public HelperTransformationResolver(HelperTransformationBuilder helpers)
		{
			_Helper = helpers;
		}

		private HelperTransformationBuilder _Helper;
		/// <summary>
		/// Gets the helper builder.
		/// </summary>
		/// <value>The HelperTransformationBuilder that is used for looking up the typereference.</value>
		public HelperTransformationBuilder HelperBuilder
		{
			get { return _Helper; }
		}

		/// <summary>
		/// Maps a URI to an object containing the actual resource.
		/// </summary>
		/// <param name="absoluteUri">The URI returned from <see cref="M:System.Xml.XmlResolver.ResolveUri(System.Uri,System.String)"/></param>
		/// <param name="role">The current implementation does not use this parameter when resolving URIs. This is provided for future extensibility purposes. For example, this can be mapped to the xlink:role and used as an implementation specific argument in other scenarios.</param>
		/// <param name="ofObjectToReturn">The type of object to return. The current implementation only returns System.IO.Stream objects.</param>
		/// <returns>
		/// A System.IO.Stream object or null if a type other than stream is specified.
		/// </returns>
		/// <exception cref="T:System.Xml.XmlException">
		/// 	<paramref name="ofObjectToReturn"/> is neither null nor a Stream type. </exception>
		/// <exception cref="T:System.UriFormatException">The specified URI is not an absolute URI. </exception>
		/// <exception cref="T:System.NullReferenceException">
		/// 	<paramref name="absoluteUri"/> is null. </exception>
		/// <exception cref="T:System.Exception">There is a runtime error (for example, an interrupted server connection). </exception>
		public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
		{
			if (absoluteUri.Scheme == "helper")
			{
				return HelperBuilder.GetHelperStylesheet(absoluteUri.Host);
			}
			else
				return base.GetEntity(absoluteUri, role, ofObjectToReturn);
		}
	}
}
