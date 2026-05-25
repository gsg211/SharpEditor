// =============================================================================
// File:        AppDbContext.cs
// Author:      Mart Sebastian
// Description: Defines the main database context class using Entity Framework
//              Core. Manages the mapping of entity classes (Document, User,
//              UserDocument) to database tables and handles database connection
//              and configuration lifecycle.
// =============================================================================

using Core.persistence.entities;
using Microsoft.EntityFrameworkCore;

namespace Core.persistence;

public class AppDbContext:DbContext
{
    // Collection mapping for Document entities to the corresponding database table
    public DbSet<Document> Documents { get; set; }
    
    // Collection mapping for User entities to the corresponding database table
    public DbSet<User> Users { get; set; }
    
    // Collection mapping for UserDocument relation entities representing shared document links
    public DbSet<UserDocument> SharedDocuments { get; set; }
    
    // Initializes a new context instance with the specified connection configuration options
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
}