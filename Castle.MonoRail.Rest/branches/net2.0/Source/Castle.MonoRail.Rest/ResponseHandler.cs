
namespace Castle.MonoRail.Rest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Castle.MonoRail.Rest.Mime;

    public class ResponseHandler
    {

        public IControllerBridge ControllerBridge { get; set; }
        public MimeType[] AcceptedMimes { get; set; }
        public ResponseFormat Format { get; set; }



        public void Respond()
        {
            ResponseFormatInternal format = (ResponseFormatInternal)Format;
            if (ControllerBridge.IsFormatDefined)
            {
                format.RespondWith(ControllerBridge.GetFormat(), ControllerBridge);
            }
            else
            {
                bool responded = false;
                foreach (var mime in AcceptedMimes)
                {
                    if (format.RespondWith(mime.Symbol, ControllerBridge))
                    {
                        responded = true;
                        break;
                    }
                }

            }
        }
    }
   
}
