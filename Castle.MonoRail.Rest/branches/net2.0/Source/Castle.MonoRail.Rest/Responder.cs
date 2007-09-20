namespace Castle.MonoRail.Rest
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    public class Responder
    {
        private readonly string _controllerAction;
        private readonly IControllerBridge _controllerBridge;
        private string _format;

        public Responder(IControllerBridge controllerBridge, string controllerAction)
        {
            _controllerBridge = controllerBridge;
            _controllerAction = controllerAction;
        }

        public string Format
        {
            get { return _format; }
            set { _format = value; }
        }


        public void DefaultResponse()
        {
            _controllerBridge.SendRenderView(_controllerAction + "_" + Format);
        }

        public void Serialize(Object obj)
        {
            _controllerBridge.SendCancelLayoutAndView();

            XmlSerializer serial = new XmlSerializer(obj.GetType());
            _controllerBridge.UseResponseWriter(delegate(TextWriter writer) { serial.Serialize(writer, obj); });
        }

        public void Empty(int statusCode, Action<IDictionary<string, string>> addHeaders)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();

            if (addHeaders != null)
            {
                addHeaders(headers);
            }
            _controllerBridge.SetResponseCode(statusCode);

            foreach(KeyValuePair<string, string> pair in headers)
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