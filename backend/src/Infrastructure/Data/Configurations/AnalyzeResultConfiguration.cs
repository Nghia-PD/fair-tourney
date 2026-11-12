// Source: https://github.com/jasontaylordev/CleanArchitecture/blob/main/src/Infrastructure/Data/Configurations/TodoItemConfiguration.cs — ~50% copied
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class AnalyzeResultConfiguration : IEntityTypeConfiguration<AnalyzeResult>
{
    public void Configure(EntityTypeBuilder<AnalyzeResult> builder)
    {
        builder.Property(r => r.Verdict)
            .IsRequired();

        // Npgsql maps string[] to a native PostgreSQL text[] column.
        builder.Property(r => r.Reasons)
            .IsRequired();
    }
}
