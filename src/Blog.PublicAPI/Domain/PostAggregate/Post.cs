using System;
using System.Collections.Generic;
using System.Linq;
using Blog.PublicAPI.Extensions;

namespace Blog.PublicAPI.Domain.PostAggregate;

public class Post : IEntity<Guid>, IAggregateRoot
{
    private readonly List<Tag> _tags = new();

    public Post(Guid authorId, string title, string content, string[] tags)
    {
        Id = Guid.NewGuid();
        AuthorId = authorId;
        Title = title;
        TitleUrlFriendly = title.ToUrlFriendly();
        Content = content;
        CreatedAt = DateTime.UtcNow;

        AddTags(tags);
    }

    public Post()
    {
    }

    public Guid Id { get; }
    public Guid AuthorId { get; private init; }
    public string Title { get; private init; }
    public string TitleUrlFriendly { get; private init; }
    public string Content { get; private init; }
    public DateTime CreatedAt { get; private init; }
    public DateTime? UpdatedAt { get; private init; }

    public Author Author { get; private init; }
    public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();

    private void AddTags(string[] tags)
    {
        _tags.Clear();

        var newTags = tags
            .Distinct()
            .OrderBy(tag => tag)
            .Select(tag => new Tag(Id, tag))
            .ToList();

        _tags.AddRange(newTags);
    }

    public override string ToString() =>
        $"Id = {Id}, Title = {Title}, TitleUrlFriendly = {TitleUrlFriendly}, Tags = {string.Join(",", Tags)}";
}