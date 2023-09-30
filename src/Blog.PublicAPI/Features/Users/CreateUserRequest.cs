using Ardalis.Result;
using MediatR;

namespace Blog.PublicAPI.Features.Users;

public record CreateUserRequest(string Name, string Email, string Password) : IRequest<Result<UserResponse>>;