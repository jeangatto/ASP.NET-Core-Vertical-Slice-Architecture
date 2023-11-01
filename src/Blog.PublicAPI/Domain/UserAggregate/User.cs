using System;

namespace Blog.PublicAPI.Domain.UserAggregate;

public class User : IEntity<Guid>, IAggregateRoot
{
    public User(string name, string email, string hashedPassword)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email.ToLowerInvariant();
        HashedPassword = hashedPassword;
        State = UserState.Active;
        CreatedAt = DateTime.UtcNow;
    }

    public User()
    {
    }

    public Guid Id { get; }
    public string Name { get; private init; }
    public string Email { get; private init; }
    public string HashedPassword { get; private init; }
    public UserState State { get; private init; }
    public DateTime CreatedAt { get; private init; }
}