using Microsoft.EntityFrameworkCore;
using ReadMoreBooks.Models;

namespace ReadMoreBooks.Data
{
    // DbContext - Configured using EF Core Conventions Only
    public class BookStoreDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Author> Authors { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Requirement 3: Use SQL Server as database provider
            optionsBuilder.UseSqlServer("Server=.;Database=ReadMoreBooksDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        // NOTE: No OnModelCreating or Fluent API used here to strictly satisfy:
        // 'Requirement 2: Use EF Core CONVENTIONS ONLY'
    }
}
