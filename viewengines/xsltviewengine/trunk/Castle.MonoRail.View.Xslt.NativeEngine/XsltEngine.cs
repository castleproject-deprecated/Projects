namespace Castle.MonoRail.View.Xslt.NativeEngine
{

	using System;
	using System.Collections.Generic;
	using System.Text;
	using Castle.MonoRail.Framework.View.Xslt;
	using System.Xml.Xsl;
	using System.Xml;
	using Castle.MonoRail.View.Xslt.NativeEngine.AdapterBuilder;
	using System.IO;

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
		/// <param name="stream">The stream to load the transformation from.</param>
		/// <param name="arguments">The arguments to arguments to use in the transformation.</param>
		/// <returns></returns>
		public IXsltTransform LoadTransform(System.IO.Stream stream, XsltTransformationArguments arguments)
		{
			XslCompiledTransform transformer = new XslCompiledTransform();
			using (XmlReader reader = new XmlTextReader(stream))
			{
				transformer.Load(reader, null, 
					new HelperTransformationResolver(
						XsltTransform.BuildTransformationHelper(arguments)));
			}
			return new XsltTransform(transformer, this);
		}

	}
}
