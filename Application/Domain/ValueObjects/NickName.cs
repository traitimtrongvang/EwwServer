using System.Diagnostics.CodeAnalysis;
using Application.Domain.Exceptions;

namespace Application.Domain.ValueObjects;

public record NickName
{
    public required string Val { get; init; }

    /// <exception cref="InvalidNickNameExc"></exception>
    [SetsRequiredMembers]
    public NickName(string val)
    {
        ThrowIfInvalid(val);
        Val = val;
    }
    
    public static void ThrowIfInvalid(string val)
    {
        if (string.IsNullOrEmpty(val))
            throw new InvalidNickNameExc("{{NickName}} is required");
        
        if (val.StartsWith(" ") || val.EndsWith(" "))
            throw new InvalidNickNameExc("{{NickName}} can't start or end with whitespace");
    }
}