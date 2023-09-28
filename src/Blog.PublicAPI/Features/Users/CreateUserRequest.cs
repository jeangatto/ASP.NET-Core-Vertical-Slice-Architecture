using Ardalis.Result;
using MediatR;

namespace Blog.PublicAPI.Features.Users;

public record CreateUserRequest(string Name, string UserName, string Email, string Password) : IRequest<Result<UserResponse>>;