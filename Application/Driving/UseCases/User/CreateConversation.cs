using Application.Domain.Exceptions;
using Application.Domain.Services;
using Application.Domain.ValueObjects;
using Application.Driven.AuthService;
using Application.Driven.EwwDatabase.Repositories;
using Application.Driving.Exceptions;
using FluentValidation;

namespace Application.Driving.UseCases.User;

public record CreateConversationReq
{
    public required ConversationTypeEnum Type { get; init; }
    
    public required HashSet<string> MemberUserIdSet { get; init; }

    public required string? Name { get; init; }
}

public record CreateConversationRes
{
    public required Guid Id { get; init; }
}

public interface ICreateConversation
{
    // TODO docs and unit test for this shit
    Task<CreateConversationRes> HandleAsync(
        string accessToken, 
        CreateConversationReq req, 
        CancellationToken cancellationToken = default);
}

public record CreateConversation : ICreateConversation
{
    private readonly IConversationService _conversationService;
    private readonly IAuthService _authService;
    private readonly IValidator<CreateConversationReq> _validator;
    private readonly IConversationRepository _conversationRepo;
    
    public CreateConversation(IConversationService conversationService, IAuthService authService, IValidator<CreateConversationReq> validator, IConversationRepository conversationRepo)
    {
        _conversationService = conversationService;
        _authService = authService;
        _validator = validator;
        _conversationRepo = conversationRepo;
    }

    public async Task<CreateConversationRes> HandleAsync(string accessToken, CreateConversationReq req, CancellationToken cancellationToken = default)
    {
        // TODO this block will be duplicated
        var (isAuthorized, accessTokenPayload) = _authService.Authorize(accessToken);
        if (!isAuthorized || accessTokenPayload is null)
            throw new UnauthorizedExc();
        
        await _validator.ValidateAndThrowAsync(req, cancellationToken);
        
        var creatorUserId = new UserId(accessTokenPayload.UserId);
        
        var conversation = req.Type switch
        {
            ConversationTypeEnum.Couple 
                => await _conversationService.CreateCoupleConversationAsync(
                    creatorUserId: creatorUserId, 
                    memberUserId: new (req.MemberUserIdSet.ToList()[0]), 
                    cancellationToken),
            ConversationTypeEnum.Group
                => await _conversationService.CreateGroupConversationAsync(
                    creatorUserId: creatorUserId, 
                    memberUserIdSet: req.MemberUserIdSet.Select(m => new UserId(m)).ToHashSet(), 
                    name: new(req.Name), 
                    cancellationToken),
            _ 
                => throw new NotSupportedException("This will never happen")
        };

        await _conversationRepo.InsertAsync(conversation, cancellationToken);
        
        return new CreateConversationRes
        {
            Id = conversation.Id.Val
        };
    }
}

public class CreateConversationReqValidator : AbstractValidator<CreateConversationReq>
{
    public CreateConversationReqValidator()
    {
        {
            RuleFor(c => c.Type)
                .IsInEnum()
                .WithMessage("{{field}} is invalid");

            When(c => c.Type is ConversationTypeEnum.Couple, () =>
            {
                RuleFor(c => c.Name)
                    .Null()
                    .WithMessage("{{field}} are not allow in Couple type");

                RuleFor(c => c.MemberUserIdSet)
                    .Must(set => set.Count == 1)
                    .WithMessage("can invite one and only one Member in Couple Conversation");
            });
        
            When(c => c.Type is ConversationTypeEnum.Group, () =>
            {
                RuleFor(c => c.Name)
                    .Custom((name, context) =>
                    {
                        try
                        {
                            _ = new ConversationName(name);
                        }
                        catch (InvalidConversationNameExc exc)
                        {
                            context.AddFailure(exc.Message);
                        }
                    });

                RuleFor(c => c.MemberUserIdSet)
                    .Must(set => set.Count >= 2)
                    .WithMessage("invite at least tow Member in to create a Group Conversation");
            });
        }
    }
}