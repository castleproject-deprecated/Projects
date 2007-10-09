namespace Castle.NVelocity
{
    using System.Collections.Generic;

    public enum ErrorSeverity { Error, Warning, Message }

    public class ErrorHandler
    {
        private List<Error> _errors = new List<Error>();

        // string lineText,string fileName,int errorCode,
        public void AddError(string description, Position position, ErrorSeverity severity)
        {
            if (position == null)
            {
                position = new Position(1, 1);
            }
            
            _errors.Add(new Error(description, position, severity));
        }

        public int Count
        {
            get { return _errors.Count; }
        }

        public Error this[int index]
        {
            get { return _errors[index]; }
        }

        //public IEnumerator GetEnumerator()
        //{
        //    return new Enumerator(_errors);
        //}

        //public class Enumerator : IEnumerator
        //{
        //    private List<Error> _errors;
        //    private Error _current;

        //    public Enumerator(List<Error> errors)
        //    {
        //        _errors = errors;
        //    }

        //    public bool MoveNext()
        //    {

        //    }

        //    public void Reset()
        //    {

        //    }

        //    public object Current
        //    {
        //        get { return _current; }
        //    }
        //}
    }

    public class Error
    {
//        private string _fileName;
        private string _description;
        private Position _position;
//        private int _errorCode; // ?
        private ErrorSeverity _severity;

        public Error(string description, Position position, ErrorSeverity severity)
        {
            _description = description;
            _position = position;
            _severity = severity;
        }
        
        //        public string FileName
//        {
//            get { return _fileName; }
//            set { _fileName = value; }
//        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public Position Position
        {
            get { return _position; }
            set { _position = value; }
        }

//        public int ErrorCode
//        {
//            get { return _errorCode; }
//            set { _errorCode = value; }
//        }

        public ErrorSeverity Severity
        {
            get { return _severity; }
            set { _severity = value; }
        }
    }
}
