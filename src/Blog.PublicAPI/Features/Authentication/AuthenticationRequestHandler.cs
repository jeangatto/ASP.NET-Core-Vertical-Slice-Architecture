using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using BCrypt.Net;
using Blog.PublicAPI.Data;
using Blog.PublicAPI.Domain.UserAggregate;
using Blog.PublicAPI.Shared;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Blog.PublicAPI.Features.Authentication;

public class AuthenticationRequestHandler(
    BlogDbContext dbContext,
    JwtOptions jwtOptions,
    IValidator<AuthenticationRequest> validator) : IRequestHandler<AuthenticationRequest, Result<TokenResponse>>
{
    private readonly BlogDbContext _dbContext = dbContext;
    private readonly JwtOptions _jwtOptions = jwtOptions;
    private readonly IValidator<AuthenticationRequest> _validator = validator;

    public async Task<Result<TokenResponse>> Handle(AuthenticationRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<TokenResponse>.Invalid(validationResult.AsErrors());
        }

        var email = request.Email.ToLowerInvariant();

        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Email == email && user.State == UserState.Active, cancellationToken);

        if (user == null)
        {
            return Result<TokenResponse>.NotFound("User not found.");
        }

        if (!BCrypt.Net.BCrypt.EnhancedVerify(request.Password, user.HashedPassword, HashType.SHA512))
        {
            return Result<TokenResponse>.Error("Email or password is incorrect.");
        }

        var claims = GenerateClaims(user);

        var accessToken = CreateAccessToken(claims);

        return Result.Success(new TokenResponse(accessToken, _jwtOptions.ExpirationSeconds));
    }

    private static Claim[] GenerateClaims(User user)
    {
        var identifier = user.Id.ToString();

        return
        [
            new Claim(ClaimTypes.NameIdentifier, identifier),
            new Claim(JwtRegisteredClaimNames.UniqueName, identifier),
            new Claim(JwtRegisteredClaimNames.Sub, user.Name, ClaimValueTypes.String),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];
    }

    private string CreateAccessToken(Claim[] claims)
    {
        var createdAt = DateTime.UtcNow;

        var expiresAt = createdAt.AddSeconds(_jwtOptions.ExpirationSeconds);

        var keyBytes = Encoding.UTF8.GetBytes(_jwtOptions.SigningKey);

        var symmetricSecurityKey = new SymmetricSecurityKey(keyBytes);

        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }
}