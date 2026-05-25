// =============================================================================
// File:        User.cs
// Author:      Mart Sebastian
// Description: Represents the User entity stored in the database.
//              Contains the user's credentials (hashed password), contact
//              information, and a creation timestamp for auditing purposes.
// =============================================================================

namespace Core.persistence.entities;

public class User
{
    // Primary key
    public int Id { get; set; }

    // Unique display name chosen by the user during registration
    public string Username { get; set; }

    // Hashed password stored in the format "base64(salt):base64(hash)"
    public string PasswordHash { get; set; }

    // Unique email address used for authentication
    public string Email { get; set; }

    // Timestamp of when the account was created (UTC)
    public DateTime CreatedAt { get; set; }
}