# ADR-002: Write-Once Upload Chunks

## Status

Accepted. Supersedes the overwrite-on-retry behaviour in [ADR-001](ADR-001-Client-Assigned-Sequential-Chunk-Index.md).

## Date

2026-10-30

## Context

[ADR-001](ADR-001-Client-Assigned-Sequential-Chunk-Index.md) makes chunk uploads idempotent by letting a retry overwrite the same key. This applies to both recording chunks (`/recordings/{sessionId}/chunks`) and screenshot chunks (`/screenshots/{sessionId}/chunks`). Two problems follow:

- Recordings and screenshots are anti-cheat evidence. Overwriting lets a client replace a chunk that has already been stored.
- The key included the file extension (`chunk_0003.png`). Re-uploading index 3 with a different content type created a second object (`chunk_0003.jpg`) instead of overwriting, so one index could map to two chunks.

Simply returning `409 Conflict` for any existing index would break retries. If an upload succeeds but the response is lost, the client retries and gets 409, so it has to treat 409 as success. That makes the 409 meaningless.

## Decision

Upload chunks are write-once, keyed with no extension:

```
recordings/{sessionId}/chunk_{chunkIndex:D4}
screenshots/{sessionId}/chunk_{chunkIndex:D4}
```

The media type is stored as the object's `Content-Type`.

- No object at the key → store it, `201 Created`.
- Object exists with identical bytes (a retry) → store nothing, `200 OK`.
- Object exists with different bytes → `409 Conflict`.

The write uses an S3 conditional `PutObject` (`If-None-Match: *`), so concurrent uploads of the same index cannot both succeed. On a 412, the existing object's ETag is compared with the MD5 of the new content.

## Rationale

- **Write-once with content comparison over blind overwrite:** this keeps ADR-001's retry safety and stops stored evidence from being replaced.
- **Write-once with content comparison over plain 409:** a client can tell a lost-response retry (200) from a real index clash (409).
- **Conditional write over checking existence first:** a `HEAD` followed by a `PUT` has a race window. `If-None-Match` is atomic in S3.
- **Hashing the request stream in place:** ASP.NET buffers large `IFormFile` uploads to disk and the stream is seekable. The MD5 is computed, the stream is rewound and it is uploaded, so a 50 MB recording chunk is not copied into memory.

## Consequences

**Easier:**
- Stored chunks cannot be silently replaced.
- One index maps to exactly one object.
- Client bugs that reuse an index surface as 409 instead of losing data.

**Harder:**
- The client must keep a chunk's exact bytes until its upload is confirmed, so that a retry matches.
- The content is read twice, once to hash it and once to upload it.
- Comparing the ETag with the MD5 only holds for single-part, non-KMS-encrypted uploads. Switching to multipart uploads (for example for chunks above 5 GB) or SSE-KMS requires a different comparison, for example a stored checksum.
- The S3 implementation (including local emulators) must support conditional writes on `PutObject`.
- Chunks stored before this change keep their old extension keys and are not detected as existing.
