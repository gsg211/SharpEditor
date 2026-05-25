// =============================================================================
// File:        Document.cs
// Author:      Mart Sebastian
// Description: Represents the Document entity stored in the database.
//              Contains the document's content, ownership information,
//              a version counter for optimistic concurrency control,
//              and timestamps for auditing purposes.
// =============================================================================

namespace Core.persistence.entities;

public class Document
{
    // Primary key
    public int Id { get; set; }

    // Display title of the document
    public string Title { get; set; }

    // Full text content of the document
    public string Content { get; set; }

    // Incremented on every update; used for optimistic concurrency control
    public int Version { get; set; }

    // Foreign key referencing the user who created the document
    public int OwnerId { get; set; }

    // Timestamp of when the document was first created (UTC)
    public DateTime CreatedAt { get; set; }

    // Timestamp of the most recent update to the document (UTC)
    public DateTime UpdatedAt { get; set; }
}