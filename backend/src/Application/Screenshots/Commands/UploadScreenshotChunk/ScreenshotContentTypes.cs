namespace Application.Screenshots.Commands.UploadScreenshotChunk;

internal static class ScreenshotContentTypes
{
    private static readonly Dictionary<string, string> Extensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["image/png"] = "png",
        ["image/jpeg"] = "jpg",
        ["image/webp"] = "webp"
    };

    public static string? GetExtension(string? contentType)
    {
        var mediaType = contentType?.Split(';')[0].Trim();

        return mediaType is not null && Extensions.TryGetValue(mediaType, out var extension) ? extension : null;
    }
}