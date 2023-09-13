using Application.Domain.Entities;
using Application.Domain.Exceptions;
using Application.Domain.ValueObjects;
using Application.Driven.EwwDatabase.Repositories;
using Application.Driven.UserStorage;
using Application.Driven.UserStorage.Dtos;

namespace Application.Domain.Services;

public interface IConversationService
{
    // TODO unit test for this shit
    // TODO complete docs for this shit
    /// <param name="creatorUserId"></param>
    /// <param name="memberUserId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidNumberOfMemberOfCoupleConversationExc"></exception>
    /// <exception cref="CoupleConversationAlreadyExitsExc"></exception>
    /// <exception cref="NotFoundCreatorUserExc"></exception>
    /// <exception cref="NotFoundMemberUserExc"></exception>
    Task<Conversation> CreateCoupleConversationAsync(
        UserId creatorUserId, 
        UserId memberUserId, 
        CancellationToken cancellationToken = default);
    
    // TODO unit test for this shit
    // TODO complete docs for this shit
    /// <param name="creatorUserId"></param>
    /// <param name="memberUserIdSet"></param>
    /// <param name="name"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidNumberOfMemberOfGroupConversationExc"></exception>
    /// <exception cref="NotFoundCreatorUserExc"></exception>
    /// <exception cref="InvalidConversationNameExc"></exception>
    Task<Conversation> CreateGroupConversationAsync(
        UserId creatorUserId, 
        HashSet<UserId> memberUserIdSet,
        ConversationName? name = null, 
        CancellationToken cancellationToken = default);
}

public record ConversationService : IConversationService
{
    private readonly IUserStorage _userStorage;
    private readonly IConversationRepository _conversationRepo;
    
    public ConversationService(IUserStorage userStorage, IConversationRepository conversationRepo)
    {
        _userStorage = userStorage;
        _conversationRepo = conversationRepo;
    }
    
    public async Task<Conversation> CreateCoupleConversationAsync(
        UserId creatorUserId, 
        UserId memberUserId,
        CancellationToken cancellationToken = default)
    {
        var memberUserIdSet = new HashSet<UserId>{creatorUserId, memberUserId};
        if (memberUserIdSet.Count != 2)
            throw new InvalidNumberOfMemberOfCoupleConversationExc();
        
        var isCoupleExist = await _conversationRepo.IsCoupleExistAsync(memberUserIdSet, cancellationToken);
        if (isCoupleExist)
            throw new CoupleConversationAlreadyExitsExc();
        
        var memberUserDic = await _userStorage
            .FindManyByIdSetAsync(memberUserIdSet.Select(id => id.Val).ToHashSet(), cancellationToken);
        
        if (!memberUserDic.TryGetValue(creatorUserId.Val, out _))
            throw new NotFoundCreatorUserExc();
        
        if (!memberUserDic.TryGetValue(memberUserId.Val, out _))
            throw new NotFoundMemberUserExc();

        return HelperCreateConversation(creatorUserId, ConversationTypeEnum.Couple, memberUserDic);
    }
    
    public async Task<Conversation> CreateGroupConversationAsync(
        UserId creatorUserId, 
        HashSet<UserId> memberUserIdSet, 
        ConversationName? name = null,
        CancellationToken cancellationToken = default)
    {
        memberUserIdSet.Add(creatorUserId);
        if (memberUserIdSet.Count <= 2)
            throw new InvalidNumberOfMemberOfGroupConversationExc();
        
        var memberUserDic = await _userStorage
            .FindManyByIdSetAsync(memberUserIdSet.Select(m => m.Val).ToHashSet(), cancellationToken);
        
        if (memberUserDic.Count <= 2)
            throw new InvalidNumberOfMemberOfGroupConversationExc();
        
        if (memberUserDic[creatorUserId.Val] is null)
            throw new NotFoundCreatorUserExc();
        
        return HelperCreateConversation(creatorUserId, ConversationTypeEnum.Group, memberUserDic, name);
    }
    
    /// <warning>
    ///     This is just a helper function, make sure all params are valid before calling this
    /// </warning>
    private Conversation HelperCreateConversation(
        UserId creatorUserId, 
        ConversationTypeEnum type,
        IDictionary<string, UserRes> memberUserDic, 
        ConversationName? name = null)
    {
        var conversation = new Conversation
        {
            CreatorUserId = creatorUserId,
            Type = type,
            Name = name ?? new (null),
            MemberList = new (),
            Id = new (Guid.NewGuid())
        };
        
        conversation.MemberList.AddRange(memberUserDic.Values.Select(memberUser => new Member
        {
            UserId = new (memberUser.Id),
            ConversationId = conversation.Id,
            NickName = new (memberUser.Name),
            IsAccepted = creatorUserId.Val == memberUser.Id,
            Id = new (Guid.NewGuid())
        }));
        
        return conversation;
    }
}