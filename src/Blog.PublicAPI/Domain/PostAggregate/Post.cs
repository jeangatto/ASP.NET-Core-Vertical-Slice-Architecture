using System;
using System.Collections.Generic;
using System.Linq;
using Blog.PublicAPI.Extensions;

namespace Blog.PublicAPI.Domain.PostAggregate;

public class Post : IEntity<Guid>, IAggregateRoot
{
    private readonly List<Tag> _tags = new();

    public Post()
    {
    }

    private Post(Guid authorId, string title, string content, string[] tags)
    {
        Id = Guid.NewGuid();
        AuthorId = authorId;
        Title = title;
        TitleUrlFriendly = title.UrlFriendly();
        Content = content;
        CreatedAt = DateTime.UtcNow;

        AddTags(tags);
    }

    public Guid Id { get; }
    public Guid AuthorId { get; private set; }
    public string Title { get; private set; }
    public string TitleUrlFriendly { get; private set; }
    public string Content { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public Author Author { get; private set; }
    public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();

    public static Post Create(Guid authorId, string title, string content, string[] tags)
    {
        ArgumentNullException.ThrowIfNull(authorId, nameof(authorId));
        ArgumentException.ThrowIfNullOrEmpty(title, nameof(title));
        ArgumentException.ThrowIfNullOrEmpty(content, nameof(title));
        ArgumentNullException.ThrowIfNull(tags, nameof(tags));

        return new Post(authorId, title, content, tags);
    }

    public void Update(string title, string content, string[] tags)
    {
        ArgumentException.ThrowIfNullOrEmpty(title, nameof(title));
        ArgumentException.ThrowIfNullOrEmpty(content, nameof(title));
        ArgumentNullException.ThrowIfNull(tags, nameof(tags));

        Title = title;
        Content = content;
        UpdatedAt = DateTime.UtcNow;

        AddTags(tags);
    }

    private void AddTags(string[] tags)
    {
        _tags.Clear();

        var newTags = tags
            .Distinct()
            .OrderBy(tag => tag)
            .Select(tag => Tag.Create(Id, tag))
            .ToList();

        _tags.AddRange(newTags);
    }

    public override string ToString() => $"Id = {Id}, Title = {Title}, Tags = {string.Join(",", Tags)}";
}