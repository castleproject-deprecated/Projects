using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MonoRail.Rest.Mime;
using System.IO;
namespace Castle.MonoRail.Rest
{
    public class ControllerBridge : IControllerBridge
    {
        private RestfulController _controller;
        private string _controllerAction;

        public ControllerBridge(RestfulController controller, string controllerAction)
        {
            _controller = controller;
            _controllerAction = controllerAction;
        }

        public void SetResponseType(MimeType mime)
        {
            _controller.Response.ContentType = mime.MimeString;
        }

        public void SendRenderView(string view)
        {
            _controller.RenderView(view);
        }

        public void SendCancelLayoutAndView()
        {
            _controller.CancelLayout();
            _controller.CancelView();
        }

        public void UseResponseWriter(Action<TextWriter> writerAction)
        {
            writerAction(_controller.Response.Output);
        }

        public void SetResponseCode(int code)
        {
            _controller.Response.StatusCode = code;
        }

        public void AppendResponseHeader(string headerName, string value)
        {
            _controller.Response.AppendHeader(headerName, value);
        }

        public void SendRenderText(string text)
        {
            _controller.RenderText(text);
        }

        #region IControllerBridge Members


        public string ControllerAction
        {
            get { return _controllerAction; }
        }

        public bool IsFormatDefined
        {
            get {
                if (String.IsNullOrEmpty(_controller.Params["format"]))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public string GetFormat()
        {
            return _controller.Params["format"];
        }

        #endregion
    }
}
