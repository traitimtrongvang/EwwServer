using Application.Domain.Services;
using Application.Driving.UseCases.Commons;
using Application.Driving.UseCases.User;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ServiceCollectionExt
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        // services
        services.AddScoped<IConversationService, ConversationService>();
        services.AddScoped<IMessageService, MessageService>();
        
        // fluent validation
        services.AddValidatorsFromAssemblyContaining<CreateConversationReqValidator>();
        
        // use cases
        services.AddScoped<ICreateConversation, CreateConversation>();
        services.AddScoped<IReadAccessToken, ReadAccessToken>();
        services.AddScoped<ISendMessage, SendMessage>();
    }
}