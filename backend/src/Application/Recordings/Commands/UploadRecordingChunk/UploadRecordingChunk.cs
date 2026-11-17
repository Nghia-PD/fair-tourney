using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
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

    public UploadRecordingChunkCommandHandler(IValidator<UploadRecordingChunkCommand> validator, IFileStorage fileStorage)
    {
        _validator = validator;
        _fileStorage = fileStorage;
    }

    /// <returns><c>true</c> if the chunk was stored; <c>false</c> if an identical chunk was already stored (a retry).</returns>
    public async Task<bool> Handle(UploadRecordingChunkCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var chunk = new RecordingChunk(request.SessionId, request.ChunkIndex);
        // No extension in the key: one index maps to exactly one object whatever the video format; the type lives in Content-Type.
        var key = $"recordings/{chunk.SessionId}/chunk_{chunk.ChunkIndex:D4}";

        var result = await _fileStorage.UploadIfNotExistsAsync(key, request.Content, request.ContentType, cancellationToken);

        return result switch
        {
            WriteOnceUploadResult.Created => true,
            WriteOnceUploadResult.AlreadyExistsWithSameContent => false,
            _ => throw new ConflictException($"Chunk {chunk.ChunkIndex} of session {chunk.SessionId} has already been uploaded with different content.")
        };
    }
}
