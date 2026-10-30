using Application.Recordings.Commands.UploadRecordingChunk;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("recordings")]
public class RecordingsController : ControllerBase
{
    [HttpPost("{sessionId:guid}/chunks")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(50_000_000)]
    public async Task<IActionResult> UploadChunk(
        [FromServices] UploadRecordingChunkCommandHandler handler,
        [FromRoute] Guid sessionId,
        [FromForm] int chunkIndex,
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
    {
        await using var content = file.OpenReadStream();

        var created = await handler.Handle(new UploadRecordingChunkCommand
        {
            SessionId = sessionId,
            ChunkIndex = chunkIndex,
            Content = content,
            ContentType = file.ContentType,
            Length = file.Length
        }, cancellationToken);

        // An identical retry of an already stored chunk is idempotent: 200 instead of 201.
        return StatusCode(created ? StatusCodes.Status201Created : StatusCodes.Status200OK, new { sessionId, chunkIndex });
    }
}
