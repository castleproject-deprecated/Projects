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
	using Castle.MonoRail.View.Xslt.NativeEngine.AdapterBuilder;

	/// <summary>
	/// Implementation of the IXsltTransform interface that
	/// uses standard .NET API to make an XsltTransformation
	/// </summary>
	internal class XsltTransform : IXsltTransform
	{
		private XslCompiledTransform _transformer;
		private XsltEngine _Engine;

		internal XsltTransform(XslCompiledTransform transformer, XsltEngine engine)
		{
			_transformer = transformer;

			_Engine = engine;
		}

		/// <summary>
		/// Converts the XsltTransformationArguments to a XsltArgumentList instance
		/// </summary>
		/// <param name="arguments">The arguments.</param>
		/// <returns></returns>
		private XsltArgumentList BuildXslArgumentList(XsltTransformationArguments arguments)
		{
			XsltArgumentList list = new XsltArgumentList();

			foreach (XsltTransformationParameter param in arguments.Parameters)
			{
				list.AddParam(param.Name, param.NameSpace, param.Value);
			}

			
			foreach (object exO in arguments.ExtensionObjects)
			{
				list.AddExtensionObject("urn:" + exO.GetType().Name,
					_Engine.DynamicAdapterStore.Adapt(exO));
			}
			return list;
			
		}
		/// <summary>
		/// Registers the extensions objects in the specified
		/// transformation arguments to a new HelperTransformationBuilder.
		/// </summary>
		/// <param name="arguments">The arguments.</param>
		/// <returns></returns>
		internal static HelperTransformationBuilder BuildTransformationHelper(XsltTransformationArguments arguments)
		{
			HelperTransformationBuilder hp = new HelperTransformationBuilder();

			foreach (object exO in arguments.ExtensionObjects)
			{
				hp.AddType(exO.GetType());
			}
			return hp;
		}

		#region IXsltTransform Members

		/// <summary>
		/// Transforms the specified input to the specified output using the specified arguments.
		/// </summary>
		/// <param name="input">The input document.</param>
		/// <param name="output">The output document to write to.</param>
		/// <param name="arguments">The arguments of the transformation.</param>
		public void Transform(System.Xml.XmlReader input, System.Xml.XmlWriter output, XsltTransformationArguments arguments)
		{
			XsltArgumentList xslArguments = BuildXslArgumentList(arguments);

			_transformer.Transform(input, xslArguments, output);
		}

		/// <summary>
		/// Gets the IXsltEngine that was used to created this IXsltTransform.
		/// </summary>
		/// <value>The engine.</value>
		public IXsltEngine Engine
		{
			get { return _Engine; }
		}

		#endregion
	}
}
