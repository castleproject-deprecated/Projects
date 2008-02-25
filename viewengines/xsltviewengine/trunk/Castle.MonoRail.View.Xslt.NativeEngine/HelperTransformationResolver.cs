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
	using System.Xml;
	using Castle.MonoRail.Framework.View.Xslt;

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
		public HelperTransformationResolver(IXsltTemplateResolver resolver, HelperTransformationBuilder helpers)
		{
			_TemplateResolver = resolver;
			_Helper = helpers;
		}

		private IXsltTemplateResolver _TemplateResolver;
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

		/// <summary>
		/// Resolves the absolute URI from the base and relative URIs.
		/// </summary>
		/// <param name="baseUri">The base URI used to resolve the relative URI.</param>
		/// <param name="relativeUri">The URI to resolve. The URI can be absolute or relative. If absolute, this value effectively replaces the <paramref name="baseUri"/> value. If relative, it combines with the <paramref name="baseUri"/> to make an absolute URI.</param>
		/// <returns>
		/// A <see cref="T:System.Uri"/> representing the absolute URI or null if the relative URI cannot be resolved.
		/// </returns>
		/// <exception cref="T:System.ArgumentException">
		/// 	<paramref name="relativeUri"/> is null</exception>
		public override Uri ResolveUri(Uri baseUri, string relativeUri)
		{
			if (baseUri == null && relativeUri.ToLower().Contains(".xslt"))
			{
				return _TemplateResolver.ResolveTemplate(relativeUri);
			}
			else
			return base.ResolveUri(baseUri, relativeUri);
		}
	}
}
