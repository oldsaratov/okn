public class BaseObjectException : System.Exception
{
    public BaseObjectException() { }
    public BaseObjectException(string message) : base(message) { }
    public BaseObjectException(string message, System.Exception inner) : base(message, inner) { }
}

public class ObjectNotExistException : BaseObjectException
{
    public ObjectNotExistException(string message) : base(message) { }
}

public class ObjectEventNotExistException : BaseObjectException
{
    public ObjectEventNotExistException(string message) : base(message) { }
}