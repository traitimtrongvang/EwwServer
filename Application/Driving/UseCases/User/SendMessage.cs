using Application.Domain.Exceptions;
using Application.Domain.Services;
using Application.Driven.AuthService;
using Application.Driven.EwwDatabase.Repositories;
using Application.Driving.Exceptions;

namespace Application.Driving.UseCases.User;

public record SendMessageReq
{
    public required string ConversationId { get; init; }

    public required string Content { get; init; }

    public required DateTime CreatedAt { get; init; }
}

public record SendMessageRes
{
    
}

public interface ISendMessage
{
    Task<SendMessageRes> HandleAsync(string accessToken, SendMessageReq req, CancellationToken cancellationToken = default);
}

public record SendMessage : ISendMessage
{
    private readonly IMessageService _messageService;
    private readonly IAuthService _authService;
    private readonly IMessageRepository _messageRepository;

    public SendMessage(IMessageService messageService, IAuthService authService, IMessageRepository messageRepository)
    {
        _messageService = messageService;
        _authService = authService;
        _messageRepository = messageRepository;
    }

    public async Task<SendMessageRes> HandleAsync(string accessToken, SendMessageReq req, CancellationToken cancellationToken = default)
    {
        // TODO this block will be duplicated
        var (isAuthorized, accessTokenPayload) = _authService.Authorize(accessToken);
        if (!isAuthorized || accessTokenPayload is null)
            throw new UnauthorizedExc();

        try
        {
            // call business function
            var message = await _messageService.CreateMessageAsync(
                senderUserId: new(accessTokenPayload.UserId),
                conversationId: new(Guid.Parse(req.ConversationId)),
                content: new(req.Content),
                createdAt: req.CreatedAt,
                cancellationToken);
            
            // save to db, but actually there is a cache behind
            await _messageRepository.InsertAsync(message, cancellationToken);
            
            // TODO send notification, research Slack notification decision alg
            
            return new SendMessageRes();
        }
        catch (NotFoundMemberExc)
        {
            // because authorize is passed then the reason is because req.ConversationId not found
            throw new NotFoundConversationExc();
        }
    }
}