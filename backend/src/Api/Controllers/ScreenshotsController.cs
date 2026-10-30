using Application.Screenshots.Commands.UploadScreenshotChunk;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("screenshots")]
public class ScreenshotsController : ControllerBase
{
    [HttpPost("{sessionId:guid}/chunks")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(10 * 1024 * 1024)] //10 mb limit

    public async Task<IActionResult> UploadChunk(
        [FromServices] UploadScreenshotChunkCommandHandler handler,
        [FromRoute] Guid sessionId,
        [FromForm] int chunkIndex,
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
    {
        await using var content = file.OpenReadStream();

        await handler.Handle(new UploadScreenshotChunkCommand
        {
            SessionId = sessionId,
            ChunkIndex = chunkIndex,
            Content = content,
            ContentType = file.ContentType,
            Length = file.Length
        }, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, new { sessionId, chunkIndex });
    }
}