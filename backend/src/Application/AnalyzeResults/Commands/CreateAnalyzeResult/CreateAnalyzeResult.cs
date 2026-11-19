using Application.Common.Interfaces;
using Domain.ValueObjects;
using ValidationException = Application.Common.Exceptions.ValidationException;

namespace Application.AnalyzeResults.Commands.CreateAnalyzeResult;

public record CreateAnalyzeResultCommand
{
    public Guid SessionId { get; init; }
    public int ChunkIndex { get; init; }
    public required string Verdict { get; init; }
    public decimal Confidence { get; init; }
    public required IReadOnlyList<string> Reasons { get; init; }
}

public class CreateAnalyzeResultCommandHandler
{
    private readonly IValidator<CreateAnalyzeResultCommand> _validator;
    private readonly IApplicationDbContext _context;

    public CreateAnalyzeResultCommandHandler(IValidator<CreateAnalyzeResultCommand> validator, IApplicationDbContext context)
    {
        _validator = validator;
        _context = context;
    }

    /// <returns>The id of the stored result; the existing one if this chunk was already analysed (a retry).</returns>
    public async Task<Guid> Handle(CreateAnalyzeResultCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var chunk = new ScreenshotChunk(request.SessionId, request.ChunkIndex);

        var existingId = await _context.AnalyzeResults
            .Where(r => r.SessionId == chunk.SessionId && r.ChunkIndex == chunk.ChunkIndex)
            .Select(r => (Guid?)r.Id)
            .FirstOrDefaultAsync(cancellationToken);
        if (existingId is not null)
            return existingId.Value;

        var entity = new AnalyzeResult
        {
            SessionId = chunk.SessionId,
            ChunkIndex = chunk.ChunkIndex,
            Verdict = request.Verdict,
            Confidence = request.Confidence,
            Reasons = [.. request.Reasons]
        };

        _context.AnalyzeResults.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
