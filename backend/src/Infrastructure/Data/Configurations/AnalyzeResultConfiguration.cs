// Source: https://github.com/jasontaylordev/CleanArchitecture/blob/main/src/Infrastructure/Data/Configurations/TodoItemConfiguration.cs — ~40% copied
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class AnalyzeResultConfiguration : IEntityTypeConfiguration<AnalyzeResult>
{
    public void Configure(EntityTypeBuilder<AnalyzeResult> builder)
    {
        // One result per screenshot chunk: a redelivered SQS message must not add a second row.
        builder.HasIndex(r => new { r.SessionId, r.ChunkIndex })
            .IsUnique();

        builder.Property(r => r.Verdict)
            .IsRequired();

        // 0.00 to 1.00
        builder.Property(r => r.Confidence)
            .HasPrecision(3, 2);

        // Npgsql maps string[] to a native PostgreSQL text[] column.
        builder.Property(r => r.Reasons)
            .IsRequired();
    }
}
