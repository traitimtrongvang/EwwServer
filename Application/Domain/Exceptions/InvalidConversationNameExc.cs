namespace Application.Domain.Exceptions;

public class InvalidConversationNameExc : Exception
{
    public InvalidConversationNameExc()
    {
    }

    public InvalidConversationNameExc(string? message) : base(message)
    {
    }
}