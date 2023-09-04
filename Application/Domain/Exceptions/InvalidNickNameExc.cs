namespace Application.Domain.Exceptions;

public class InvalidNickNameExc : Exception
{
    public InvalidNickNameExc()
    {
    }

    public InvalidNickNameExc(string? message) : base(message)
    {
    }
}