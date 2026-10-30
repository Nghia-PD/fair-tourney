namespace Application.Common.Models;

public enum WriteOnceUploadResult
{
    Created,
    AlreadyExistsWithSameContent,
    AlreadyExistsWithDifferentContent
}
