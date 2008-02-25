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
	using System.Xml.Xsl;
	using Castle.MonoRail.Framework.View.Xslt;

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
