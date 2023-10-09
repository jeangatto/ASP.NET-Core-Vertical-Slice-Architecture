using System;

namespace Blog.PublicAPI.Domain.PostAggregate;

public class Author : IEntity<Guid>
{
    private Author(Guid userId, string name)
    {
        Id = userId;
        Name = name;
    }

    public Author()
    {
    }

    public Guid Id { get; }
    public string Name { get; private init; }

    public static Author Create(Guid userId, string name)
    {
        ArgumentNullException.ThrowIfNull(userId, nameof(userId));
        ArgumentException.ThrowIfNullOrEmpty(name, nameof(name));

        return new(userId, name);
    }

    public override string ToString() => Name;
}