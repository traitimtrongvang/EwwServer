namespace Application.Domain.Exceptions;

public class InvalidMessageContentExc : Exception
{
    public InvalidMessageContentExc()
    {
    }

    public InvalidMessageContentExc(string? message) : base(message)
    {
    }
}