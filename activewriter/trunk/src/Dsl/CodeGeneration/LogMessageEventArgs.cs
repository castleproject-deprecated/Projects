using System;

namespace Altinoren.ActiveWriter.CodeGeneration
{
    public class LogMessageEventArgs : EventArgs
    {
        private readonly string _message;

        public LogMessageEventArgs(string message)
        {
            _message = message;
        }

        public string Message
        {
            get { return _message; }
        }
    }
}