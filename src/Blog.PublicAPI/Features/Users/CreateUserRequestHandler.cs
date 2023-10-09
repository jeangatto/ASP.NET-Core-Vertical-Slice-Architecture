using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using AutoMapper;
using BCrypt.Net;
using Blog.PublicAPI.Data;
using Blog.PublicAPI.Domain.PostAggregate;
using Blog.PublicAPI.Domain.UserAggregate;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Blog.PublicAPI.Features.Users;

public class CreateUserRequestHandler : IRequestHandler<CreateUserRequest, Result<UserResponse>>
{
    private readonly BlogContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateUserRequest> _validator;

    public CreateUserRequestHandler(BlogContext context, IMapper mapper, IValidator<CreateUserRequest> validator)
    {
        _context = context;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<UserResponse>> Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _validator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
        {
            return Result<UserResponse>.Invalid(result.AsErrors());
        }

        var email = request.Email.ToLowerInvariant();

        if (await _context.Users.AsNoTracking().AnyAsync(user => user.Email == email, cancellationToken))
        {
            return Result<UserResponse>.Conflict("The email address provided is already in use.");
        }

        var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Password.Trim(), HashType.SHA512);

        var user = User.Create(request.Name, email, hashedPassword);

        _context.Add(user);

        var author = Author.Create(user.Id, request.Name);

        _context.Add(author);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(_mapper.Map<UserResponse>(user));
    }
}