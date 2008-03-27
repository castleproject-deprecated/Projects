using System;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Castle.MonoRail.Framework.Adapters;
using Castle.Components.Binder;
using Castle.MonoRail.Rest.Binding;

namespace Castle.MonoRail.Rest
{
	public class RestfulRequestAdapter : RequestAdapter
	{
		public RestfulRequestAdapter(HttpRequest request) : base(request)
		{
		}

		public override CompositeNode FormNode
		{
			get
			{
				if (String.Equals("application/xml", ContentType, StringComparison.InvariantCultureIgnoreCase))
				{
					if (formCompositeNode == null)
					{
						XmlTreeBuilder builder = new XmlTreeBuilder();
						formCompositeNode = builder.BuildNode(GetDocFromRequest());
					}
					return formCompositeNode;
				}
				else
				{
					return base.FormNode;
				}
			}
		}


		public override CompositeNode ParamsNode
		{
			get
			{
				if (String.Equals("application/xml", ContentType, StringComparison.InvariantCultureIgnoreCase))
				{
					if (paramsCompositeNode == null)
					{
						CompositeNode rootNode = base.ParamsNode;
						XmlTreeBuilder builder = new XmlTreeBuilder();
						builder.AddToRoot(rootNode, GetDocFromRequest());
						paramsCompositeNode = rootNode;
					}
					return paramsCompositeNode;
				}
				else
				{
					return base.ParamsNode;
				}
			}
		}

		private XDocument GetDocFromRequest()
		{
			InputStream.Position = 0;
			var reader = XmlReader.Create(InputStream);
			return XDocument.Load(reader);
		}
	}
}
