namespace Castle.MonoRail.Rest
{
    using System;
    using System.IO;
    using Mime;

    public interface IControllerBridge
    {
        string ControllerAction { get; }
        bool IsFormatDefined { get; }
        void SetResponseType(MimeType mime);
        void SendRenderView(string view);
        void SendCancelLayoutAndView();
        void UseResponseWriter(Action<TextWriter> writerAction);
        void SetResponseCode(int code);
        void AppendResponseHeader(string headerName, string value);
        void SendRenderText(string text);
        string GetFormat();
    }
}