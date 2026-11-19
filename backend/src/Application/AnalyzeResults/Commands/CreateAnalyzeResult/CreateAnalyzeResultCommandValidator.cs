namespace Application.AnalyzeResults.Commands.CreateAnalyzeResult;

public class CreateAnalyzeResultCommandValidator : AbstractValidator<CreateAnalyzeResultCommand>
{
    public CreateAnalyzeResultCommandValidator()
    {
        RuleFor(v => v.SessionId)
            .NotEmpty();

        RuleFor(v => v.ChunkIndex)
            .GreaterThanOrEqualTo(0);

        RuleFor(v => v.Verdict)
            .Must(AnalyzeVerdicts.All.Contains)
            .WithMessage("Verdict must be 'clean', 'suspicious' or 'unclear'.");

        RuleFor(v => v.Confidence)
            .InclusiveBetween(0m, 1m);

        RuleFor(v => v.Reasons)
            .NotNull();
    }
}
