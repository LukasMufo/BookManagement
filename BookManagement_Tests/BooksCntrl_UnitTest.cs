using BookManagement.Controllers;
using BookManagement.DbContexts;
using BookManagement.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace BookManagement_Tests
{
    public class BooksCntrl_UnitTest
    {
        private BookLibraryContext _context;
        private BooksController _controller;

        public BooksCntrl_UnitTest()
        {
            var options = new DbContextOptionsBuilder<BookLibraryContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new BookLibraryContext(options);

            _controller = new BooksController(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
        [Fact]
        public async Task GetBook_WithValidId_ReturnsBook()
        {
            var book = new Book { Title = "Test Book", Author = "Test Author" };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetBook(id: book.ID);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedBook = Assert.IsType<Book>(okResult.Value);
            Assert.Equal(book.ID, returnedBook.ID);
            Assert.Equal(book.Title, returnedBook.Title);
            Assert.Equal(book.Author, returnedBook.Author);
        }
        [Fact]
        public async Task GetBook_WithInValidId_ReturnsBadReq()
        {
            //create book anyway
            var book = new Book { Title = "Test Book", Author = "Test Author" };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetBook(id: 0);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetBooks_WithValidBorrow_ReturnsBook()
        {
            int BorrowedCount = 3;
            for (int i=0; i<BorrowedCount; i++)
            {
                var book = new Book { Title = "Test Book", Author = "Test Author" };
                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                var BorrowEntry = new BorrowedBook { BookID = book.ID, UserID = 1, BorrowedFrom = DateOnly.FromDateTime(DateTime.Now), BorrowedUntil = DateOnly.FromDateTime(DateTime.Now.AddDays(1)) };
                _context.Books.Add(book);
                await _context.SaveChangesAsync();
            }

            // Act
            /*var result = await _controller.GetBooks(borrowed: true);*/

            // Assert
            /*var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedBook = Assert.IsType<List<Book>>(okResult.Value);
            Assert.Equal(book.ID, returnedBook.ID);
            Assert.Equal(book.Title, returnedBook.Title);
            Assert.Equal(book.Author, returnedBook.Author);*/
        }

        [Fact]
        public async Task GetBooks_WithInValidBorrowed_ReturnsBadReq()
        {
            //create book anyway
            var book = new Book { Title = "Test Book", Author = "Test Author" };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetBook(id: 0);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
    }
}