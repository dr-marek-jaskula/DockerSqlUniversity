namespace SakilaWebApi.Exceptions;

public class ForbidException : Exception
{
    public ForbidException(string message) : base(message)
    {
    }
}