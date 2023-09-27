using System;

namespace Blog.PublicAPI.Domain.PostAggregate;

public class Author : IEntity<Guid>
{

    public Author(Guid userId, string name)
    {
        Id = userId;
        Name = name;
    }

    public Author()
    {
    }

    public Guid Id { get; }
    public string Name { get; private init; }

    public override string ToString() => Name;
}