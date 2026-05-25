// =============================================================================
// File:        UserDocument.cs
// Author:      Mart Sebastian
// Description: Defines the SharePermission enum and the UserDocument join
//              entity that represents the many-to-many relationship between
//              users and documents. Each record stores the permission level
//              granted to a specific user for a specific document.
//              The composite primary key is (UserId, DocumentId).
// =============================================================================

using Microsoft.EntityFrameworkCore;

namespace Core.persistence.entities;

// Defines the access levels a user can have on a document
public enum SharePermission
{
    Owner,      // Created the document; full control including sharing and deletion
    ReadOnly,   // Can view the document but cannot modify it
    ReadWrite,  // Can view and edit the document
}

// Composite primary key defined via EF Core attribute
[PrimaryKey(nameof(UserId), nameof(DocumentId))]
public class UserDocument
{
    // Navigation property to the associated User
    public User User { get; set; }

    // Foreign key referencing the User
    public int UserId { get; set; }

    // Navigation property to the associated Document
    public Document Document { get; set; }

    // Foreign key referencing the Document
    public int DocumentId { get; set; }

    // The permission level granted to the user for this document
    public SharePermission PermissionLevel { get; set; }
}