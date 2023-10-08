using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using Blog.PublicAPI.Data;
using Blog.PublicAPI.Domain.UserAggregate;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Blog.PublicAPI.Features.Auth;

public class LoginRequestHandler : IRequestHandler<LoginRequest, Result<TokenResponse>>
{
    private readonly BlogContext _context;
    private readonly IValidator<LoginRequest> _validator;

    public LoginRequestHandler(BlogContext context, IValidator<LoginRequest> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Result<TokenResponse>> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _validator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
        {
            return Result.Invalid(result.AsErrors());
        }

        var email = request.Email.ToLowerInvariant();

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Email == email && user.State == UserState.Active, cancellationToken);

        if (user == null)
        {
            return Result.NotFound("User not found");
        }

        if (!BCrypt.Net.BCrypt.EnhancedVerify(request.Password, user.HashedPassword))
        {
            var validatorError = new ValidationError { ErrorMessage = "Email or password is incorrect." };
            return Result.Invalid(new List<ValidationError> { validatorError });
        }

        var claims = GenerateClaims(user);


        throw new NotImplementedException();
    }

    private static Claim[] GenerateClaims(User user) => new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
        new Claim(JwtRegisteredClaimNames.UniqueName, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Sub, user.Name, ClaimValueTypes.String),
        new Claim(JwtRegisteredClaimNames.Email, user.Email, ClaimValueTypes.Email)
    };
}