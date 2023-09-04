using System.Diagnostics.CodeAnalysis;
using Application.Domain.Exceptions;

namespace Application.Domain.ValueObjects;

public record MessageContent
{
    public required string Val { get; init; }
    
    /// <exception cref="InvalidMessageContentExc"></exception>
    [SetsRequiredMembers]
    public MessageContent(string val)
    {
        ThrowIfInvalid(val);
        Val = val;
    }
    
    /// <exception cref="InvalidMessageContentExc"></exception>
    public static void ThrowIfInvalid(string val)
    {
        if (string.IsNullOrEmpty(val))
            throw new InvalidMessageContentExc("{{MessageContent}} is required");

        if (val.StartsWith(" ") || val.EndsWith(" "))
            throw new InvalidMessageContentExc("{{MessageContent}} can't start or end with white-space");
    }
}