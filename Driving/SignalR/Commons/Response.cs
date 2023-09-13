using System.Net;
using Application.Driving.Helpers;
using FluentValidation;

namespace SignalR.Commons;

public record Response
{
    public required HttpStatusCode StatusCode { get; init; }
    
    public string? Message { get; init; }
}

public record ResponseWithPayload<T> : Response
{
    public required T Payload { get; init; }
}

public record ResponseWithError : Response
{
    public required object Errors { get; init; }

    public static ResponseWithError NewValidationErrorRes(ValidationException exc)
    {
        var errors = Helpers.ValidationFailureToDic(exc.Errors);
        
        return new ResponseWithError
        {
            StatusCode = HttpStatusCode.BadRequest,
            Errors = errors,
            Message = "Validation Error"
        };
    }
}