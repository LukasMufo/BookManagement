using BookManagement.DbContexts;
using BookManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.Services
{
    public interface IBorrowedService
    {
        Task<BorrowedBook> GetByBookAsync(int id);
        Task<List<BorrowedBook>> GetByUserAsync(int id);
        Task<List<BorrowedBook>> GetAllAsync();
        Task<BorrowedBook> BorrowBookAsync(BorrowedBook borrow);
        Task<BorrowedBook> DeleteAsync(int bookId);
        Task<BorrowedBook> UpdateAsync(int bookId, BorrowedBook borrow);
    }
    public class BorrowedService : IBorrowedService
    {
        private readonly BookLibraryContext _context;

        public BorrowedService(BookLibraryContext context)
        {
            _context = context;
        }

        public async Task<BorrowedBook> GetByBookAsync(int id)
        {
            var borrowedBook = await _context.BorrowedBooks.FirstOrDefaultAsync(x => x.BookID == id);
            return borrowedBook;
        }
        public async Task<List<BorrowedBook>> GetByUserAsync(int id)
        {
            var borrowedBooks = await _context.BorrowedBooks.Where(x => x.UserID == id).ToListAsync();
            return borrowedBooks;
        }
        public async Task<List<BorrowedBook>> GetAllAsync()
        {
                var borrowedBooks = await _context.BorrowedBooks.ToListAsync();
                return borrowedBooks;
        }
        public async Task<BorrowedBook> BorrowBookAsync(BorrowedBook borrow)
        {
            _context.BorrowedBooks.Add(borrow);
            await _context.SaveChangesAsync();
            return borrow;
        }
        public async Task<BorrowedBook> DeleteAsync(int bookId)
        {
            try
            {
                var borrowedBook = await _context.BorrowedBooks.FindAsync(bookId);
                if (borrowedBook == null)
                    throw new KeyNotFoundException($"User with ID {bookId} not found.");

                _context.BorrowedBooks.Remove(borrowedBook);
                await _context.SaveChangesAsync();
                return borrowedBook;
            }
            catch (Exception ex)
            {
                // Log or handle the exception accordingly
                throw;
            }
        }
        public async Task<BorrowedBook> UpdateAsync(int bookId, BorrowedBook borrow)
        {
            var find = await _context.BorrowedBooks.FindAsync(bookId);
            if (find == null)
                throw new KeyNotFoundException($"User with ID {bookId} not found.");

            find.BookID = borrow.BookID;
            find.UserID = borrow.UserID;
            find.BorrowedFrom = borrow.BorrowedFrom;
            find.BorrowedUntil = borrow.BorrowedUntil;

            _context.Entry(find).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return find;
        }
    }
}
