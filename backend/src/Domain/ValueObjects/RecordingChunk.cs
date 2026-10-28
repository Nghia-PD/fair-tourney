namespace Domain.ValueObjects;

public class RecordingChunk : ValueObject
{
    public Guid SessionId { get; }
    public int ChunkIndex { get; }

    public RecordingChunk(Guid sessionId, int chunkIndex)
    {
        if (sessionId == Guid.Empty)
        {
            throw new InvalidRecordingChunkException("session id is required.");
        }
        if (chunkIndex < 0)
        {
            throw new InvalidRecordingChunkException("chunk index must be zero or greater.");
        }

        SessionId = sessionId;
        ChunkIndex = chunkIndex;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return SessionId;
        yield return ChunkIndex;
    }
}
