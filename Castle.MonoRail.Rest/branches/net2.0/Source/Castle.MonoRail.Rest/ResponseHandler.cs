
namespace Castle.MonoRail.Rest
{
    using Castle.MonoRail.Rest.Mime;

    public class ResponseHandler
    {
        private IControllerBridge _controllerBridge;
        private MimeType[] _acceptedMimes;
        private ResponseFormat _format;

        public IControllerBridge ControllerBridge
        {
            get { return _controllerBridge; }
            set { _controllerBridge = value; }
        }

        public MimeType[] AcceptedMimes
        {
            get { return _acceptedMimes; }
            set { _acceptedMimes = value; }
        }

        public ResponseFormat Format
        {
            get { return _format; }
            set { _format = value; }
        }

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
                foreach (MimeType mime in AcceptedMimes)
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
