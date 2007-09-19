using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MonoRail.Framework;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using Castle.Components.Binder;
using System.Xml.Serialization;
using Castle.MonoRail.Rest.Binding;
using System.Xml.Linq;
using System.Xml;
using Castle.MonoRail.Rest.Mime;
namespace Castle.MonoRail.Rest
{
    public class RestfulController : SmartDispatcherController 
    {
        private CompositeNode _formNode;
        private CompositeNode _paramsNode;
        private string _controllerAction;

        protected override System.Reflection.MethodInfo SelectMethod(string action, System.Collections.IDictionary actions, IRequest request, System.Collections.IDictionary actionArgs)
        {
            if (String.Equals("collection", action, StringComparison.InvariantCultureIgnoreCase) || String.IsNullOrEmpty(action))
            {
                switch (request.HttpMethod.ToUpper())
                {
                    case "GET":
                        _controllerAction = "Index";
                        return (MethodInfo)actions["Index"];
                    case "POST":
                        _controllerAction = "Create";
                        return (MethodInfo)actions["Create"];
                    default:
                        return base.SelectMethod(action, actions, request, actionArgs);
                }
            }
            else
            {

                if (String.Equals("new", action, StringComparison.InvariantCultureIgnoreCase))
                {
                    _controllerAction = "New";
                    return (MethodInfo)actions["New"];
                }

                if (!actions.Contains(action))
                {
                    MethodInfo selectedMethod = null;
                    switch (request.HttpMethod.ToUpper())
                    {
                        case "GET":
                            _controllerAction = "Show";
                            selectedMethod = (MethodInfo)actions["Show"];
                            break;
                        case "PUT":
                            _controllerAction = "Update";
                            selectedMethod = (MethodInfo)actions["Update"];
                            break;
                        case "DELETE":
                            _controllerAction = "Destroy";
                            selectedMethod = (MethodInfo)actions["Destroy"];
                            break;
                        default:
                            //Should maybe just throw here.
                            return base.SelectMethod(action, actions, request, actionArgs);

                    }

                    if (selectedMethod != null)
                    {
                        LeafNode n = new LeafNode(typeof(String), "ID", action);
                        ParamsNode.AddChildNode(n);
                    }
                    return selectedMethod;
                }
                else
                {
                    return base.SelectMethod(action, actions, request, actionArgs);
                }
            }
        }

        
        protected override CompositeNode FormNode
        {
            get
            {

                string ct = this.Context.UnderlyingContext.Request.ContentType;
                if (String.Equals("application/xml", ct, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (_formNode == null)
                    {
                        XmlTreeBuilder builder = new XmlTreeBuilder();                        
                        _formNode = builder.BuildNode(GetDocFromRequest());
                    }
                    return _formNode;
                }
                else
                {
                    return base.FormNode;
                }
            }
        }


        protected override CompositeNode ParamsNode
        {
            get
            {
                string ct = this.Context.UnderlyingContext.Request.ContentType;
                if (String.Equals("application/xml", ct, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (_paramsNode == null)
                    {
                        CompositeNode rootNode = base.ParamsNode;
                        XmlTreeBuilder builder = new XmlTreeBuilder();
                        builder.AddToRoot(rootNode, GetDocFromRequest());
                        _paramsNode = rootNode;
                    }
                    return _paramsNode;
                }
                else
                {
                    return base.ParamsNode;
                }
            }
        }

        private XDocument GetDocFromRequest()
        {
            var inputStream = Context.UnderlyingContext.Request.InputStream;
            inputStream.Position = 0;
            var reader = XmlReader.Create(inputStream);
            return XDocument.Load(reader);
        }
	
       
        protected void RespondTo(Action<ResponseFormat> collectFormats)
        {
            MimeTypes registeredMimes = new MimeTypes();
            registeredMimes.RegisterBuiltinTypes();

            ResponseHandler handler = new ResponseHandler()
            {
                ControllerBridge = new ControllerBridge(this, _controllerAction),
                AcceptedMimes = AcceptType.Parse((string)Request.Headers["Accept"], registeredMimes),
                Format = new ResponseFormat()
            };

            collectFormats(handler.Format);
            handler.Respond();
            
        }

        private bool IsFormatDefined()
        {
            return !String.IsNullOrEmpty(Params["format"]);
        }

        protected string UrlFor(IDictionary parameters)
        {
            return UrlBuilder.BuildUrl(this.Context.UrlInfo, parameters);
        }

        protected string UrlFor(string action)
        {
            return UrlBuilder.BuildUrl(Context.UrlInfo, this.Name, action);
        }
       
    }
}
