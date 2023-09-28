using System;

namespace Blog.PublicAPI.Features.Users;

public record UserResponse(Guid Id, string Name, string UserName, string Email, DateTime CreatedAt);