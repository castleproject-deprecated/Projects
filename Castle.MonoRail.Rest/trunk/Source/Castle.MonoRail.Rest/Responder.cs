

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Castle.MonoRail.Rest.Mime;
using System.Xml.Serialization;
using System.Collections.Specialized;
namespace Castle.MonoRail.Rest
{
    public class Responder
    {
        private IControllerBridge _controllerBridge;
        private string _controllerAction;

        public String Format { get; set; }

        public Responder(IControllerBridge controllerBridge, string controllerAction)
        {
            _controllerBridge = controllerBridge;
            _controllerAction = controllerAction;
        }
    

        public void DefaultResponse()
        {
            _controllerBridge.SendRenderView(_controllerAction + "_" + Format);
        }

        public void Serialize(Object obj)
        {

            _controllerBridge.SendCancelLayoutAndView();

            XmlSerializer serial = new XmlSerializer(obj.GetType());
            _controllerBridge.UseResponseWriter(writer => serial.Serialize(writer, obj));

        }

        public void Empty(int statusCode, Action<IDictionary<string, string>> addHeaders)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            
            if (addHeaders != null)
            {
                addHeaders(headers);
            }
            _controllerBridge.SetResponseCode(statusCode);

            foreach (KeyValuePair<string, string> pair in headers)
            {
                _controllerBridge.AppendResponseHeader(pair.Key, pair.Value);

            }
            _controllerBridge.SendCancelLayoutAndView();
        }

        public void Empty(int statusCode)
        {
            Empty(statusCode, null);
        }

        public void Text(string text)
        {
            _controllerBridge.SendRenderText(text);
        }
    }
}
