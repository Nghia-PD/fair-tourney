namespace Application.Screenshots.Commands.UploadScreenshotChunk;

public class UploadScreenshotChunkCommandValidator : AbstractValidator<UploadScreenshotChunkCommand>
{
    public UploadScreenshotChunkCommandValidator()
    {
        RuleFor(v => v.SessionId)
            .NotEmpty();

        RuleFor(v => v.ChunkIndex)
            .GreaterThanOrEqualTo(0);

        RuleFor(v => v.Length)
            .GreaterThan(0)
            .WithMessage("File must not be empty.");

        RuleFor(v => v.ContentType)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(ct => ScreenshotContentTypes.GetExtension(ct) is not null)
            .WithMessage("File must be a PNG, JPEG or WebP image.");
    }
}
