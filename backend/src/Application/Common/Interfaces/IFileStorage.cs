namespace Application.Common.Interfaces;

public interface IFileStorage
{
    Task UploadAsync(string key, Stream content, string contentType, CancellationToken cancellationToken);
}