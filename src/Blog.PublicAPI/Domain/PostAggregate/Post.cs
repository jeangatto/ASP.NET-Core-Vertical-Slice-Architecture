using System;
using System.Collections.Generic;
using System.Linq;

namespace Blog.PublicAPI.Domain.PostAggregate;

public class Post : IEntity<Guid>, IAggregateRoot
{
    private readonly List<Tag> _tags = new();

    public Post(string title, string content, string[] tags)
    {
        ArgumentException.ThrowIfNullOrEmpty(title, nameof(title));
        ArgumentException.ThrowIfNullOrEmpty(content, nameof(title));
        ArgumentNullException.ThrowIfNull(tags, nameof(tags));

        Id = Guid.NewGuid();
        Title = title;
        Content = content;
        CreatedAt = DateTime.UtcNow;

        AddTags(tags);
    }

    public Post()
    {
    }

    public Guid Id { get; }
    public string Title { get; private set; }
    public string Content { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();

    public void Update(string title, string content, string[] tags)
    {
        ArgumentException.ThrowIfNullOrEmpty(title, nameof(title));
        ArgumentException.ThrowIfNullOrEmpty(content, nameof(title));
        ArgumentNullException.ThrowIfNull(tags, nameof(tags));

        Title = title;
        Content = content;
        UpdatedAt = DateTime.UtcNow;

        _tags.Clear();

        AddTags(tags);
    }

    private void AddTags(string[] tags)
    {
        var newTags = tags
            .Distinct()
            .OrderBy(tag => tag)
            .Select(tag => new Tag(Id, tag))
            .ToList();

        _tags.AddRange(newTags);
    }

    public override string ToString() => $"Id = {Id}, Title = {Title}, Tags = {string.Join(",", Tags)}";
}
