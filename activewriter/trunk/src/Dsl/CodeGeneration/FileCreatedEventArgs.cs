using System;
using System.IO;

namespace Altinoren.ActiveWriter.CodeGeneration
{
    public class FileCreatedEventArgs : EventArgs
    {
        private readonly string _filePath;
        private readonly string _fileName;

        public FileCreatedEventArgs(string filePath)
        {
            _filePath = filePath;
            _fileName = Path.GetFileName(filePath);
        }

        public string FilePath
        {
            get { return _filePath; }
        }

        public string FileName
        {
            get { return _fileName; }
        }
    }
}