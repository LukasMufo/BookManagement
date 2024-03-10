using BookManagement.Models;
using Microsoft.EntityFrameworkCore;
namespace BookManagement.DbContexts
{
    public class BookLibraryContext : DbContext
    {
        public BookLibraryContext(DbContextOptions<BookLibraryContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<BorrowedBook> BorrowedBooks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Konfigurácia pre tabuľku kníh
            modelBuilder.Entity<Book>(b =>
            {
                b.Property(f => f.ID).ValueGeneratedOnAdd();
                b.HasKey(f => f.ID);
            });

            modelBuilder.Entity<User>(u =>
            {
                u.Property(f => f.ID).ValueGeneratedOnAdd();
                u.HasKey(f => f.ID);
            });

            modelBuilder.Entity<BorrowedBook>(b =>
            {
                b.HasKey(f => f.BookID);
            });

            modelBuilder.Entity<BorrowedBook>()
                .HasOne<Book>()
                .WithMany() // No navigation property in Book pointing back to BorrowedBook
                .HasForeignKey(b => b.BookID);

            modelBuilder.Entity<BorrowedBook>()
                .HasOne<User>()
                .WithMany() // No navigation property in Book pointing back to BorrowedBook
                .HasForeignKey(b => b.UserID);

            base.OnModelCreating(modelBuilder);
        }
    }
}
