using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Application.Common.Interfaces;
using Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<S3StorageOptions>(configuration.GetSection(S3StorageOptions.SectionName));

        services.AddSingleton<IAmazonS3>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<S3StorageOptions>>().Value;

            var config = new AmazonS3Config
            {
                ForcePathStyle = options.ForcePathStyle,
                // Emulators such as Floci may not support the SDK's default checksum headers.
                RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED,
                ResponseChecksumValidation = ResponseChecksumValidation.WHEN_REQUIRED
            };

            if (!string.IsNullOrEmpty(options.ServiceUrl))
            {
                config.ServiceURL = options.ServiceUrl;
                config.AuthenticationRegion = options.Region;
            }
            else
            {
                config.RegionEndpoint = RegionEndpoint.GetBySystemName(options.Region);
            }

            // Credentials come from the SDK default chain (AWS_ACCESS_KEY_ID / AWS_SECRET_ACCESS_KEY, profile or IAM role).
            return new AmazonS3Client(config);
        });

        services.AddScoped<IFileStorage, S3FileStorage>();

        return services;
    }
}