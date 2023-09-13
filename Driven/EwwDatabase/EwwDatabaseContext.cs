using Application.Domain.Entities;
using Application.Driven.EwwDatabase;
using Microsoft.EntityFrameworkCore;

namespace EwwDatabase;

public class EwwDatabaseContext : DbContext, IEwwDatabaseContext
{
    public required DbSet<Conversation> Conversations { get; init; }
    
    public required DbSet<Member> Members { get; init; }
    
    public required DbSet<Message> Messages { get; init; }
    
    public EwwDatabaseContext(DbContextOptions options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EwwDatabaseContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}