using Lms.Domain.Aggregates;
using Lms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lms.Infrastructure.Persistence
{
    public class LibraryDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Patron> Patrons { get; set; }
        public DbSet<Library> Libraries { get; set; }

        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>().HasKey(b => b.Id);
            modelBuilder.Entity<Book>().Property(b => b.Title).IsRequired();
            modelBuilder.Entity<Book>().Property(b => b.ISBN).IsRequired();
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Library)
                .WithMany(l => l.Books)
                .HasForeignKey(b => b.LibraryId);

            modelBuilder.Entity<Patron>().HasKey(p => p.Id);
            modelBuilder.Entity<Patron>().Property(p => p.Name).IsRequired();
            modelBuilder.Entity<Patron>()
                .HasMany(p => p.CheckedOutBooks)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Library>().HasKey(l => l.Id);
            modelBuilder.Entity<Library>().Property(l => l.Name).IsRequired();
        }
    }
}