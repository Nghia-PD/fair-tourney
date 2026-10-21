using Domain.Exceptions;

namespace Domain.ValueObjects;

public sealed record RecordingChunk
{
    public Guid SessionId {get; }
    public int ChunkIndex {get; }

    public RecordingChunk (Guid sessionId, int chunkIndex)
    {
        if(sessionId == Guid.Empty)
        {
            throw new InvalidRecordingChunkException("session id is required.");
        }
        if(chunkIndex < 0)
        {
            throw new InvalidRecordingChunkException("chunk index must be zero or greater.");
        }

        SessionId = sessionId;
        ChunkIndex = chunkIndex;

    }

    public string StorageKey => $"recordings/{SessionId}/chunk_{ChunkIndex:D4}.webm";
}