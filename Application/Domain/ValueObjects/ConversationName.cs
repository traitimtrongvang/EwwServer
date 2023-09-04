using System.Diagnostics.CodeAnalysis;
using Application.Domain.Exceptions;

namespace Application.Domain.ValueObjects;

public record ConversationName
{
    public required string? Val { get; init; }

    /// <exception cref="InvalidConversationNameExc"></exception>
    [SetsRequiredMembers]
    public ConversationName(string? val)
    {
        ThrowIfInvalid(val);
        Val = val;
    }
    
    /// <exception cref="InvalidConversationNameExc"></exception>
    public static void ThrowIfInvalid(string? val)
    {
        if (val is null)
            return;

        if (string.IsNullOrEmpty(val))
            throw new InvalidConversationNameExc("{{ConversationName}} can't be a empty, but not required");
        
        if (val.StartsWith(" ") || val.EndsWith(" "))
            throw new InvalidConversationNameExc("{{ConversationName}} can't start or end with whitespace");
    }
}