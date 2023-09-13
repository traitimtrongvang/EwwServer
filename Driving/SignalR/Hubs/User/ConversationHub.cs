using System.Net;
using Application.Domain.Exceptions;
using Application.Driving.UseCases.User;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using SignalR.Commons;
using SignalR.Hubs.User.Models;

namespace SignalR.Hubs.User;

public interface IConversationHub
{
    public Task CreateConversation(string accessToken, CreateConversationReq req);

    public Task JoinConversation(string accessToken, JoinConversationReq req);

    public Task SendMessage(string accessToken, SendMessageReq req);
}

public interface IConversationClientHub
{
    Task OnCreateConversationSuccess(ResponseWithPayload<CreateConversationRes> res);
    Task OnCreateConversationFailed(Response res);
    Task OnNewConversationRequested(ResponseWithPayload<CreateConversationRes> res);

    Task OnJoinConversationSuccess(Response res);

    Task OnSendMessageSuccess(ResponseWithPayload<SendMessageRes> res);
    Task OnReceiveMessage(ResponseWithPayload<SendMessageRes> res);
}

// TODO, we need to use Redis to map UserId to ConnectionId
public class ConversationHub : Hub<IConversationClientHub>, IConversationHub
{
    private readonly ICreateConversation _createConversation;
    private readonly ISendMessage _sendMessage;

    public ConversationHub(ICreateConversation createConversation, ISendMessage sendMessage)
    {
        _createConversation = createConversation;
        _sendMessage = sendMessage;
    }

    public async Task CreateConversation(string accessToken, CreateConversationReq req)
    {
        try
        {
            var payload = await _createConversation.HandleAsync(accessToken, req);

            var callerRes = new ResponseWithPayload<CreateConversationRes>()
            {
                StatusCode = HttpStatusCode.Created,
                Message = "Create Conversation Successfully",
                Payload = payload
            };

            var receiverRes = new ResponseWithPayload<CreateConversationRes>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "New Conversation Requested",
                Payload = payload
            };

            await Task.WhenAll(
                Clients.User(Context.UserIdentifier!).OnCreateConversationSuccess(callerRes),
                Clients.Users(req.MemberUserIdSet).OnNewConversationRequested(receiverRes));
        }
        catch (NotFoundMemberUserExc)
        {
            var res = new ResponseWithError
            {
                StatusCode = HttpStatusCode.UnprocessableEntity,
                Errors = new
                {
                    MemberUserIdSet = "Member is not found"
                },
            };

            await Clients.User(Context.UserIdentifier!).OnCreateConversationFailed(res);
        }
        catch (CoupleConversationAlreadyExitsExc)
        {
            var res = new ResponseWithError
            {
                StatusCode = HttpStatusCode.Conflict,
                Errors = new
                {
                    MemberUserIdSet = "Already in one of your Couple Conversations"
                },
            };

            await Clients.User(Context.UserIdentifier!).OnCreateConversationFailed(res);
        }
        catch (InvalidNumberOfMemberOfGroupConversationExc)
        {
            var res = new ResponseWithError
            {
                StatusCode = HttpStatusCode.UnprocessableEntity,
                Errors = new
                {
                    MemberUserIdSet = "Not enough Member to create a Group Conversation, some of them may be not found"
                },
            };

            await Clients.User(Context.UserIdentifier!).OnCreateConversationFailed(res);
        }
        catch (ValidationException exc)
        {
            var res = ResponseWithError.NewValidationErrorRes(exc);
            await Clients.User(Context.UserIdentifier!).OnCreateConversationFailed(res);
        }
    }

    public async Task JoinConversation(string accessToken, JoinConversationReq req)
    {
        // TODO, need to check if User have eligibility to join Conversation

        await Groups.AddToGroupAsync(Context.ConnectionId, req.ConversationId.ToString());

        var res = new Response
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Join Conversation Successfully"
        };

        await Clients.Caller.OnJoinConversationSuccess(res);
    }

    public async Task SendMessage(string accessToken, SendMessageReq req)
    {
        var payload = await _sendMessage.HandleAsync(accessToken, req); // TODO we need try catch here, but I'm too lazy
        var res = new ResponseWithPayload<SendMessageRes>
        {
            StatusCode = (HttpStatusCode)0,
            Payload = payload
        };
        await Task.WhenAll(
            Clients.Caller.OnSendMessageSuccess(res),
            Clients.GroupExcept(req.ConversationId, "").OnReceiveMessage(res));
    }
}