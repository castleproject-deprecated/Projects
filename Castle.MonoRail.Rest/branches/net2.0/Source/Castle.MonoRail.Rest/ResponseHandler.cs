namespace Castle.MonoRail.Rest
{
    using Mime;

    public class ResponseHandler
    {
        private MimeType[] _acceptedMimes;
        private IControllerBridge _controllerBridge;
        private ResponseFormat _format;

    	public ResponseHandler()
    	{
    	}

    	public ResponseHandler(IControllerBridge bridge, ResponseFormat format)
    	{
    		_controllerBridge = bridge;
    		_format = format;
    	}

    	public ResponseHandler(IControllerBridge bridge, ResponseFormat format, params MimeType[] acceptedMimes)
    	{
    		_controllerBridge = bridge;
    		_acceptedMimes = acceptedMimes;
    		_format = format;
    	}

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
            ResponseFormatInternal format = Format;
            if (ControllerBridge.IsFormatDefined)
            {
                format.RespondWith(ControllerBridge.GetFormat(), ControllerBridge);
            }
            else
            {
                bool responded = false;
                foreach(MimeType mime in AcceptedMimes)
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