using BookManagement.DbContexts;
using BookManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.Services
{
    public interface IBookService
    {
        Task<Book> GetBookByIdAsync(int id);
        Task<List<Book>> GetBooksAsync(bool borrowed);
        Task<Book> CreateBookAsync(Book book);
        Task<Book> DeleteBookAsync(int id);
        Task<Book> UpdateBookAsync(int id, Book book);
    }
    public class BookService : IBookService
    {
        private readonly BookLibraryContext _context;

        public BookService(BookLibraryContext context)
        {
            _context = context;
        }

        public async Task<Book> GetBookByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task<List<Book>> GetBooksAsync(bool borrowed)
        {
            List<BorrowedBook> borrowedBooks = await _context.BorrowedBooks.ToListAsync();
            List<Book> books = await _context.Books.ToListAsync();

            List<Book> result = books.Where(x =>
                (borrowed && borrowedBooks.Any(b => b.BookID == x.ID)) ||
                (!borrowed && !borrowedBooks.Any(b => b.BookID == x.ID))
            ).ToList();

            return result;
        }
        public async Task<Book> CreateBookAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }
        public async Task<Book> DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return null; // Return null if the book does not exist

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return book;

        }
        public async Task<Book> UpdateBookAsync(int id, Book book)
        {
            if (id <= 0)
                return null; // Invalid book ID

            var existingBook = await _context.Books.FindAsync(id);
            if (existingBook == null)
                return null; // Book not found

            existingBook.Title = book.Title;
            existingBook.Author = book.Author;

            _context.Entry(existingBook).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return existingBook;
        }
    }
}
