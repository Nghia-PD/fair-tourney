namespace Domain.Entities;
public class AnalyzeResult : BaseAuditableEntity
{
    public Guid session_id; // FK
    public string Versict {get; set;} = string.Empty;
    public decimal Confidence {get; set;}
    public string[] Reasons {get; set;} = [];
}