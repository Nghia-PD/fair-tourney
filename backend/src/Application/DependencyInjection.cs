using System.Reflection;
using Application.Recordings.Commands.UploadRecordingChunk;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<UploadRecordingChunkCommandHandler>();

        return services;
    }
}
