using Application.Common.Models;

namespace Application.Common.Interfaces;

public interface IFileStorage
{
    /// <summary>
    /// Writes the object only if no object exists at <paramref name="key"/>. If one exists, reports whether its content matches.
    /// </summary>
    Task<WriteOnceUploadResult> UploadIfNotExistsAsync(string key, Stream content, string contentType, CancellationToken cancellationToken);
}
