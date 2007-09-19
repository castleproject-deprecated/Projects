using System;
using System.Collections.Generic;
using Castle.MonoRail.Rest.Mime;
namespace Castle.MonoRail.Rest
{
    public interface ResponseFormatInternal
    {
        void AddRenderer(string formatSymbol, ResponderDelegate renderer);
        bool RespondWith(string format, IControllerBridge bridge);
    }

    public class ResponseFormat : ResponseFormatInternal
    {
        private readonly Dictionary<string, ResponderDelegate> _renderers;
        private readonly List<String> _order;

        public ResponseFormat()
        {
            _renderers = new Dictionary<string, ResponderDelegate>();
            _order = new List<string>();
        }

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

        private void DoResponse(string format, IControllerBridge bridge)
        {
            Responder hander = new Responder(bridge, bridge.ControllerAction);
            hander.Format = format;
            _renderers[format](hander);

            MimeTypes types = new MimeTypes();
            types.RegisterBuiltinTypes();

//            MimeType usedType = types.Where(mime => mime.Symbol == format).First();
            MimeType usedType = types.FindAll( delegate( MimeType mime ) { return mime.Symbol == format; } )[ 0 ];
            bridge.SetResponseType(usedType);
        }
       
       
    }
}
