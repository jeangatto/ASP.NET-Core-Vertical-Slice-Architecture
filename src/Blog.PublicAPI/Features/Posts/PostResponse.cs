using System;
using System.Collections.Generic;

namespace Blog.PublicAPI.Features.Posts;

public record PostResponse(Guid Id, string Title, string Content, DateTime CreatedAt, DateTime? UpdatedAt, IEnumerable<string> Tags);