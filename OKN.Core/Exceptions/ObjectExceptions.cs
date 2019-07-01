namespace OKN.Core.Exceptions
{
    public class BaseObjectException : System.Exception
    {
        protected BaseObjectException(string message) : base(message) { }
    }

    public class ObjectNotExistException : BaseObjectException
    {
        public ObjectNotExistException(string message) : base(message) { }
    }

    public class ObjectEventNotExistException : BaseObjectException
    {
        public ObjectEventNotExistException(string message) : base(message) { }
    }
}