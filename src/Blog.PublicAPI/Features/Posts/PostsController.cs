using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blog.PublicAPI.Features.Posts;

[Route("api/[controller]")]
[ApiController]
public class PostsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PostsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(
        [Required][FromBody] CreatePostRequest command,
        CancellationToken cancellationToken)
    {
        var respose = await _mediator.Send(command, cancellationToken);
        return respose.IsSuccess ? Ok() : BadRequest(respose.Errors);
    }
}
