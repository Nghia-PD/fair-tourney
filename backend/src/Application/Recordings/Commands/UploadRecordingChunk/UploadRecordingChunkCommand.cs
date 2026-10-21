namespace Application.Recordings.Commands.UploadRecordingChunk;

public class UploadRecordingChunkCommandValidator : AbstractValidator<UploadRecordingChunkCommand>
{
    public UploadRecordingChunkCommandValidator()
    {
        RuleFor(v => v.SessionId)
            .NotEmpty();

        RuleFor(v => v.ChunkIndex)
            .GreaterThanOrEqualTo(0);

        RuleFor(v => v.Length)
            .GreaterThan(0)
            .WithMessage("File must not be empty.");

        RuleFor(v => v.ContentType)
            .NotEmpty()
            .Must(ct => ct.StartsWith("video/"))
            .WithMessage("File must be a video.");
    }
}