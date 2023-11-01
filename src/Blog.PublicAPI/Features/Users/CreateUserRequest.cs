using Ardalis.Result;
using MediatR;

namespace Blog.PublicAPI.Features.Users;

public class CreateUserRequest : IRequest<Result<UserResponse>>
{
    public CreateUserRequest(string name, string email, string password)
    {
        Name = name;
        Email = email;
        Password = password;
    }

    public CreateUserRequest()
    {
    }

    public string Name { get; init; }
    public string Email { get; init; }
    public string Password { get; init; }
}