using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MonoRail.Rest.Mime;
using System.IO;
namespace Castle.MonoRail.Rest
{
    public interface IControllerBridge
    {
        void SetResponseType(MimeType mime);
        void SendRenderView(string view);
        void SendCancelLayoutAndView();
        void UseResponseWriter(Action<TextWriter> writerAction);
        void SetResponseCode(int code);
        void AppendResponseHeader(string headerName, string value);
        void SendRenderText(string text);
        string ControllerAction { get; }
        bool IsFormatDefined { get; }
        string GetFormat();
    }
}
