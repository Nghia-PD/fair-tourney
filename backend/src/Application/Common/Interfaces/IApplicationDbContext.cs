// Source: https://github.com/jasontaylordev/CleanArchitecture/blob/main/src/Application/Common/Interfaces/IApplicationDbContext.cs — ~80% copied
namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<AnalyzeResult> AnalyzeResults { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
