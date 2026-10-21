using Application.Common.Interfaces;
using Domain.ValueObjects;
using ValidationException = Application.Common.Exceptions.ValidationException;

namespace Application.Recordings.Commands.UploadRecordingChunk;

public record UploadRecordingChunkCommand
{
    public Guid SessionId { get; init; }
    public int ChunkIndex { get; init; }
    public required Stream Content { get; init; }
    public required string ContentType { get; init; }
    public long Length { get; init; }
}

public class UploadRecordingChunkCommandHandler
{
    private readonly IValidator<UploadRecordingChunkCommand> _validator;
    private readonly IFileStorage _fileStorage;

    public UploadRecordingChunkCommandHandler(IValidator<UploadRecordingChunkCommand> validator,
IFileStorage fileStorage)
    {
        _validator = validator;
        _fileStorage = fileStorage;
    }

    public async Task<string> Handle(UploadRecordingChunkCommand request, CancellationToken
cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var chunk = new RecordingChunk(request.SessionId, request.ChunkIndex);

        await _fileStorage.UploadAsync(chunk.StorageKey, request.Content, request.ContentType,
cancellationToken);

        return chunk.StorageKey;
    }
}