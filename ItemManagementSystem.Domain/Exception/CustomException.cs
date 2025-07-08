namespace ItemManagementSystem.Domain.Exception;
using System;

public class CustomException : Exception
{
    public CustomException(string message) : base(message) { }
}

public class NullObjectException : Exception
{
    public NullObjectException(string message) : base(message) { }
}

public class AlreadyExistsException : Exception
{
    public AlreadyExistsException(string message) : base(message) { }
}


