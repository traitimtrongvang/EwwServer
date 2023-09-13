using FluentValidation.Results;

namespace Application.Driving.Helpers;

public static class Helpers
{
    public static Dictionary<string, string> ValidationFailureToDic(IEnumerable<ValidationFailure> errors)
    {
        return errors.ToDictionary(
            err => char.ToLowerInvariant(err.PropertyName[0]) + err.PropertyName.Substring(1),
            err => err.ErrorMessage);
    }
}