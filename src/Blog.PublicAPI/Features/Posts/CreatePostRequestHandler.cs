using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using AutoMapper;
using Blog.PublicAPI.Data;
using Blog.PublicAPI.Domain.PostAggregate;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Blog.PublicAPI.Features.Posts;

public class CreatePostRequestHandler : IRequestHandler<CreatePostRequest, Result<PostResponse>>
{
    private readonly BlogContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<CreatePostRequest> _validator;

    public CreatePostRequestHandler(BlogContext context, IMapper mapper, IValidator<CreatePostRequest> validator)
    {
        _context = context;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<PostResponse>> Handle(CreatePostRequest request, CancellationToken cancellationToken)
    {
        var result = await _validator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
        {
            return Result.Invalid(result.AsErrors());
        }

        if (await _context.Posts.AnyAsync(post => post.Title == request.Title, cancellationToken: cancellationToken))
        {
            var validationError = new ValidationError { ErrorMessage = "There is already a registered post with the given title" };
            return Result.Invalid(new List<ValidationError> { validationError });
        }

        var post = Post.Create(request.Title, request.Content, request.Tags);

        _context.Add(post);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(_mapper.Map<PostResponse>(post));
    }
}