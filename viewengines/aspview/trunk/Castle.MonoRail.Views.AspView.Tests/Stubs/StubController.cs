using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Internal;

namespace Castle.MonoRail.Views.AspView.Tests.Stubs
{
	public class StubController : IController
	{
		IDictionary propertyBag;
		readonly Flash flash;
		readonly IRequest request;
		readonly IResponse response;

		public StubController(IDictionary propertyBag, Flash flash, IRequest request, IResponse response)
		{
			this.propertyBag = propertyBag;
			this.flash = flash;
			this.request = request;
			this.response = response;
		}

		#region IController Members

		public string Action
		{
			get { return "Stub"; }
		}

		public Flash Flash
		{
			get { return flash; }
		}

		public NameValueCollection Form
		{
			get { return new NameValueCollection(); }
		}

		public IDictionary Helpers
		{
			get { return new Hashtable(); }
		}

		public void InPlaceRenderSharedView(TextWriter output, string name)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public string LayoutName
		{
			get { return ""; }
			set { throw new Exception("The method or operation is not implemented."); }
		}

		public string Name
		{
			get { return "Stub"; }
		}

		public NameValueCollection Params
		{
			get { return new NameValueCollection(); }
		}

		public void PostSendView(object view)
		{
		}

		public void PreSendView(object view)
		{
		}

		public IDictionary PropertyBag
		{
			get { return propertyBag; }
			set { propertyBag = value; }
		}

		public NameValueCollection Query
		{
			get { return new NameValueCollection(); }
		}

		public IRequest Request
		{
			get { return request; }
		}

		public ResourceDictionary Resources
		{
			get { return new ResourceDictionary(); }
		}

		public IResponse Response
		{
			get { return response; }
		}

		public void Send(string action, IDictionary actionArgs)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void Send(string action)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public string ViewFolder
		{
			get { return ""; }
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
}
