namespace Castle.MonoRail.Rest
{
    using System;
    using System.Collections.Generic;
    using Mime;

    public interface ResponseFormatInternal
    {
        void AddRenderer(string formatSymbol, ResponderDelegate renderer);
        bool RespondWith(string format, IControllerBridge bridge);
    }

    public class ResponseFormat : ResponseFormatInternal
    {
        private readonly List<string> _order;
        private readonly Dictionary<string, ResponderDelegate> _renderers;

        public ResponseFormat()
        {
            _renderers = new Dictionary<string, ResponderDelegate>();
            _order = new List<string>();
        }

        #region ResponseFormatInternal Members

        void ResponseFormatInternal.AddRenderer(string formatSymbol, ResponderDelegate renderer)
        {
            _renderers[formatSymbol] = renderer;
            _order.Add(formatSymbol);
        }

        bool ResponseFormatInternal.RespondWith(string format, IControllerBridge bridge)
        {
            if (_renderers.ContainsKey(format))
            {
                DoResponse(format, bridge);
                return true;
            }
            else
            {
                if (String.Equals("all", format, StringComparison.InvariantCultureIgnoreCase))
                {
                    DoResponse(_order[0], bridge);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion

        public void Xml(ResponderDelegate renderer)
        {
            AddRenderer("xml", renderer);
        }

        public void Html(ResponderDelegate renderer)
        {
            AddRenderer("html", renderer);
        }

        // couldn't get Xml and Html to talk to ResponseFormatInternal.AddRenderer directly
        protected void AddRenderer(string formatSymbol, ResponderDelegate renderer)
        {
            _renderers[formatSymbol] = renderer;
            _order.Add(formatSymbol);
        }

        private void DoResponse(string format, IControllerBridge bridge)
        {
            Responder hander = new Responder(bridge, bridge.ControllerAction);
            hander.Format = format;
            _renderers[format](hander);

            MimeTypes types = new MimeTypes();
            types.RegisterBuiltinTypes();

//            MimeType usedType = types.Where(mime => mime.Symbol == format).First();
            MimeType usedType = types.FindAll(delegate(MimeType mime) { return mime.Symbol == format; })[0];
            bridge.SetResponseType(usedType);
        }
    }
}