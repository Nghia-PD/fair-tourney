using System.Reflection;
using Application.AnalyzeResults.Commands.CreateAnalyzeResult;
using Application.Recordings.Commands.UploadRecordingChunk;
using Application.Screenshots.Commands.UploadScreenshotChunk;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<UploadRecordingChunkCommandHandler>();
        services.AddScoped<UploadScreenshotChunkCommandHandler>();
        services.AddScoped<CreateAnalyzeResultCommandHandler>();

        return services;
    }
}
