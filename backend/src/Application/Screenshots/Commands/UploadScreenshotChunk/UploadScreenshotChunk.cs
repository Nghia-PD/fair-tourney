
using Domain.ValueObjects;
using Application.Common.Interfaces;

namespace Application.Screenshots.Commands.UploadScreenshotChunk;


public record UploadScreenshotChunkCommand
{
    public Guid SessionId {get; init;}
    public int ChunkIndex {get; init;}
    public required Stream Content { get; init;}
    public required string ContentType {get; init;}
    public long Length {get; init;}
}
public class UploadScreenshotChunkCommandHandler
{
    private readonly IValidator<UploadScreenshotChunkCommand> _validator;
    private readonly IFileStorage _fileStorage;

    public UploadScreenshotChunkCommandHandler(IValidator<UploadScreenshotChunkCommand> validator, IFileStorage fileStorage)
    {
        _validator = validator;
        _fileStorage = fileStorage;
    }

    public async Task Handle(UploadScreenshotChunkCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var chunk = new ScreenshotChunk(request.SessionId, request.ChunkIndex);
        var extension = ScreenshotContentTypes.GetExtension(request.ContentType);
        var key = $"screenshots/{chunk.SessionId}/chunk_{chunk.ChunkIndex:D4}.{extension}";

        await _fileStorage.UploadAsync(key, request.Content, request.ContentType, cancellationToken);
    }
}