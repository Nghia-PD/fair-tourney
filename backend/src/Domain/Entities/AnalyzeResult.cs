namespace Domain.Entities;

public class AnalyzeResult : BaseAuditableEntity
{
    // Recording/screenshot session the result belongs to. No FK constraint yet: there is no sessions table.
    public Guid SessionId { get; set; }

    public string Verdict { get; set; } = string.Empty;

    public decimal Confidence { get; set; }

    public string[] Reasons { get; set; } = [];
}
