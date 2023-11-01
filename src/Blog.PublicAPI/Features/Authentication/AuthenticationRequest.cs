using Ardalis.Result;
using MediatR;

namespace Blog.PublicAPI.Features.Authentication;

public class AuthenticationRequest : IRequest<Result<TokenResponse>>
{
    public AuthenticationRequest(string email, string password)
    {
        Email = email;
        Password = password;
    }

    public AuthenticationRequest()
    {
    }

    public string Email { get; init; }
    public string Password { get; init; }
}