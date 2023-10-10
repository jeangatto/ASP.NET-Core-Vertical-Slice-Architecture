using System;

namespace Blog.PublicAPI.Domain.UserAggregate;

public class User : IEntity<Guid>, IAggregateRoot
{
    public User()
    {
    }

    private User(string name, string email, string hashedPassword, UserState state)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        HashedPassword = hashedPassword;
        State = state;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; }
    public string Name { get; private init; }
    public string Email { get; private init; }
    public string HashedPassword { get; private init; }
    public UserState State { get; private init; }
    public DateTime CreatedAt { get; private init; }

    public static User Create(string name, string email, string hashedPassword)
    {
        ArgumentException.ThrowIfNullOrEmpty(name, nameof(name));
        ArgumentException.ThrowIfNullOrEmpty(email, nameof(email));
        ArgumentException.ThrowIfNullOrEmpty(hashedPassword, nameof(hashedPassword));

        return new User(name, email, hashedPassword, UserState.Active);
    }
}