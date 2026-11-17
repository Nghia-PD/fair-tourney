# ADR-001: Client-Assigned Sequential Chunk Index

## Status

Accepted. The overwrite-on-retry behaviour and the extension in the key are superseded by [ADR-002](ADR-002-Write-Once-Upload-Chunks.md).

## Date

2026-10-28

## Context

Participant recordings are uploaded in pieces rather than as a single file. The browser records with `MediaRecorder` and a timeslice, and each `ondataavailable` blob is posted to `POST /recordings/{sessionId}/chunks` as multipart form data with a `chunkIndex` field.

The handler stores each chunk in S3 under a key derived from the session and the index:

```
recordings/{sessionId}/chunk_{chunkIndex:D4}.{extension}
```

The original filename sent by the client is ignored. The key must therefore carry everything needed to identify a chunk, keep the chunks of a session together and put them back in recording order.

Several forces shape how the chunk is identified:

- Uploads happen over unreliable networks, so a chunk may be retried after a timeout even if the first attempt actually succeeded.
- Chunks may arrive out of order, or in parallel.
- S3 lists keys lexicographically, not numerically.
- Whatever later stitches or processes a recording needs to know whether every chunk is present.
- Client device clocks cannot be trusted: they drift, can be changed by the user and can jump backwards when NTP corrects them.

## Decision

The client assigns each chunk a zero-based, sequential integer index (`0, 1, 2, …`) per recording session and sends it as `chunkIndex`. The server uses that index, zero-padded to four digits, as the chunk's identity in the storage key. A retry of a chunk resends the same index.

The index is not a timestamp. If wall-clock time is needed for a chunk (for example `recordedAt` or `durationMs`), it is sent as a separate field and stored as metadata, not encoded in the key.

## Rationale

**Sequential index over a timestamp.** A Unix timestamp was considered as the chunk identifier, since it also orders chunks and carries timing information. It was rejected because:

- Gaps become undetectable. With `0, 1, 3` it is obvious chunk `2` is missing. With timestamps, a missing chunk looks the same as normal timeslice jitter, because `MediaRecorder` does not fire at exact intervals.
- Ordering depends on the device clock, which can move backwards mid-recording and produce keys that sort out of order.
- A timestamp in milliseconds overflows `int` (`ChunkIndex` would need to become `long`), and the `D4` padding stops being meaningful.
- Retries still require the client to remember the original value per chunk, so a timestamp is no simpler for the client than a counter.

**Client-assigned over server-assigned.** Having the server hand out the next index was considered. It was rejected because:

- The server would need per-session state and concurrency control to allocate indexes safely.
- A retry after a timeout would be indistinguishable from a new chunk and could be stored twice under different indexes. With a client-assigned index, a retry overwrites the same key, so uploads are idempotent.
- Only the client knows the true recording order. Chunks that arrive out of order or in parallel still land in the right place.

**Zero-padding the index.** S3 lists keys lexicographically, so without padding `chunk_10` sorts before `chunk_2`. Padding to four digits keeps listing order equal to recording order.

## Consequences

**Easier:**
- Uploads are idempotent: retrying a chunk with the same index replaces it instead of duplicating it.
- Missing chunks can be detected by checking for gaps in the index sequence.
- Chunks can be uploaded in parallel or out of order without affecting the final order.
- Listing `recordings/{sessionId}/` in S3 returns chunks in recording order.
- The server stays stateless with respect to chunk numbering.

**Harder:**
- The client is responsible for correctness. It must start at `0` for each new session, never reuse an index except to retry the same chunk, and track which indexes have uploaded successfully. A client bug that reuses an index silently overwrites recorded data.
- The server cannot know when a recording is complete from the chunks alone. A separate completion signal (for example a "complete" call carrying the total chunk count) will be needed before stitching or processing.
- `D4` is a minimum width, not a maximum. Beyond index `9999` keys widen to five digits and lexicographic order breaks (`chunk_10000` sorts before `chunk_9999`). At a 5-second timeslice that is roughly 13.9 hours per session, which is well beyond expected match lengths; revisit the padding if longer sessions or shorter timeslices are introduced.
- Wall-clock timing for a chunk is not available from the key and must be sent and stored separately when needed.
