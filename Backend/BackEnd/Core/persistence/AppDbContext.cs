using Core.persistence.entities;
using Microsoft.EntityFrameworkCore;

namespace Core.persistence;

public class AppDbContext:DbContext
{
    public DbSet<Document> Documents { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserDocument> SharedDocuments { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    // {
    // }
    
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=docsDatabase;Username=postgres;Password=password123");
    // }
}