using System;

namespace Blog.PublicAPI.Domain.UserAggregate;

public class User : IEntity<Guid>, IAggregateRoot
{
    private User(string name, string userName, string email, string hashedPassword, EUserState state)
    {
        Id = Guid.NewGuid();
        Name = name;
        UserName = userName;
        Email = email;
        HashedPassword = hashedPassword;
        State = state;
        CreatedAt = DateTime.UtcNow;
    }

    public User()
    {
    }

    public Guid Id { get; }
    public string Name { get; private set; }
    public string UserName { get; private set; }
    public string Email { get; private set; }
    public string HashedPassword { get; private set; }
    public EUserState State { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static User Create(string name, string userName, string email, string hashedPassword)
    {
        ArgumentException.ThrowIfNullOrEmpty(name, nameof(name));
        ArgumentException.ThrowIfNullOrEmpty(userName, nameof(userName));
        ArgumentException.ThrowIfNullOrEmpty(email, nameof(email));
        ArgumentException.ThrowIfNullOrEmpty(hashedPassword, nameof(hashedPassword));

        return new User(name, userName, email, hashedPassword, EUserState.Active);
    }
}