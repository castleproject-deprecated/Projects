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
	using System.Xml.Xsl;
	using Castle.MonoRail.Framework.View.Xslt;
	using Castle.MonoRail.Framework.View.Xslt.NativeEngine.AdapterBuilder;

	public class XsltEngine : IXsltEngine
	{
		private DynamicAdapterCachingBuilder _DynamicAdapterStore = new DynamicAdapterCachingBuilder();

		/// <summary>
		/// Will store and generate the necessary adapters for the extensions objects
		/// in a transformation
		/// </summary>
		public DynamicAdapterCachingBuilder DynamicAdapterStore
		{
			get { return _DynamicAdapterStore; }
		}

		/// <summary>
		/// Loads the transformation from the specified stream
		/// </summary>
		/// <param name="store">The store to be used when resolving includes in the xslt stylesheet</param>
		/// <param name="stream">The stream to load the transformation from.</param>
		/// <param name="arguments">The arguments to arguments to use in the transformation.</param>
		/// <returns></returns>
		public IXsltTransform LoadTransform(IXsltTemplateResolver resolver, System.IO.Stream stream, XsltTransformationArguments arguments)
		{
			XslCompiledTransform transformer = new XslCompiledTransform();
			using (XmlReader reader = new XmlTextReader(stream))
			{
				XsltSettings settings = new XsltSettings(false,false);
				transformer.Load(reader, settings,
					new HelperTransformationResolver(resolver,
						XsltTransform.BuildTransformationHelper(arguments)));
			}
			return new XsltTransform(transformer, this);
		}

	}
}
