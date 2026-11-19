namespace Application.AnalyzeResults.Commands.CreateAnalyzeResult;

public static class AnalyzeVerdicts
{
    public const string Clean = "clean";
    public const string Suspicious = "suspicious";
    public const string Unclear = "unclear";

    public static readonly IReadOnlySet<string> All = new HashSet<string> { Clean, Suspicious, Unclear };
}
