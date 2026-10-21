namespace Domain.Exceptions;

public class InvalidRecordingChunkException: Exception
{
    public InvalidRecordingChunkException(string reason) : base($"Recording chunk is invalid: {reason}")
    {
        
    }
}