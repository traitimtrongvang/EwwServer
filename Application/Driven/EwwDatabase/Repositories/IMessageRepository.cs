using Application.Domain.Entities;
using Application.Domain.ValueObjects;

namespace Application.Driven.EwwDatabase.Repositories;

public interface IMessageRepository : IBaseRepository<Message, MessageId>
{
    
}