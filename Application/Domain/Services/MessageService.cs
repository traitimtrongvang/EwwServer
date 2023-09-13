using Application.Domain.Entities;
using Application.Domain.Exceptions;
using Application.Domain.ValueObjects;
using Application.Driven.EwwDatabase.Repositories;

namespace Application.Domain.Services;

public interface IMessageService
{
    Task<Message> CreateMessageAsync(
        UserId senderUserId,
        ConversationId conversationId,
        MessageContent content, 
        DateTime createdAt,
        CancellationToken cancellationToken = default);
}

public class MessageService : IMessageService
{
    private readonly IMemberRepository _memberRepository;

    public MessageService(IMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public async Task<Message> CreateMessageAsync(
        UserId senderUserId, 
        ConversationId conversationId, 
        MessageContent content, 
        DateTime createdAt,
        CancellationToken cancellationToken = default)
    {
        var member = await _memberRepository.FindByUserIdAndConversationIdAsync(senderUserId, conversationId, cancellationToken);
        if (member is null) 
            throw new NotFoundMemberExc();
        
        // TODO also check if member is blocked or not
        
        return new Message
        {
            Id = new (Guid.NewGuid()),
            SenderMemberId = member.Id,
            Content = content,
            CreatedAt = createdAt
        };
    }
}