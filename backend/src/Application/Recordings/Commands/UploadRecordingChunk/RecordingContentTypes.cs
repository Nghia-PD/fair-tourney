namespace Application.Recordings.Commands.UploadRecordingChunk;

internal static class RecordingContentTypes
{
    private static readonly Dictionary<string, string> Extensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["video/webm"] = "webm",
        ["video/mp4"] = "mp4"
    };

    // Browsers send MediaRecorder types with parameters, e.g. "video/webm;codecs=vp8,opus".
    public static string? GetExtension(string? contentType)
    {
        var mediaType = contentType?.Split(';')[0].Trim();

        return mediaType is not null && Extensions.TryGetValue(mediaType, out var extension)
            ? extension
            : null;
    }
}
