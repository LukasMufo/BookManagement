using BookManagement.Controllers;
using BookManagement.DbContexts;
using BookManagement.Models;
using BookManagement.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

namespace BookManagement_Tests
{
    public class BooksCntrl_UnitTest : IDisposable
    {
        private BookLibraryContext _context;
        private BooksController _controller;

        public BooksCntrl_UnitTest()
        {
            var options = new DbContextOptionsBuilder<BookLibraryContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new BookLibraryContext(options);
            var bookService = new BookService(_context);
            
            _controller = new BooksController(_context, bookService);
            PrepareData().Wait();
        }

        private async Task PrepareData()
        {
            if (_context.Books.Count() > 0)
                return;
            //Create BorrowedBooks
            for (int i = 0; i < 3; i++)
            {
                var book = new Book { Title = "Borrowed Test Book " + (i+1).ToString(), Author = "Borrowed Test Author " + (i + 1).ToString() };
                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                var BorrowEntry = new BorrowedBook { BookID = book.ID, UserID = i + 1, BorrowedFrom = DateOnly.FromDateTime(DateTime.Now), BorrowedUntil = DateOnly.FromDateTime(DateTime.Now.AddDays(1)) };
                _context.BorrowedBooks.Add(BorrowEntry);
                await _context.SaveChangesAsync();
            }

            //Create UnborrowedBooks
            for (int i = 0; i < 3; i++)
            {
                var book = new Book { Title = "UnBorrowed Test Book " + (i + 1).ToString(), Author = "UnBorrowed Test Author " + (i + 1).ToString() };
                _context.Books.Add(book);
                await _context.SaveChangesAsync();
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task CreateBook_Valid()
        {
            var NewBook = new Book { Title = "Valid Title", Author = "Valid Author" };
            var result = await _controller.Create(NewBook);

            // Assert
            var okResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedBook = Assert.IsType<Book>(okResult.Value);
            Assert.Equal(NewBook.Title, returnedBook.Title);
            Assert.Equal(NewBook.Author, returnedBook.Author);
        }

        [Fact]
        public async Task CreateBook_InValid()
        {
            var result = await _controller.Create(null);

            // Assert
            var returnedResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, returnedResult.StatusCode);
            //Assert.Equal(NewBook.Author, returnedBook.Author);
        }

        [Fact]
        public async Task GetBook_WithValidId()
        {
            var book = _context.Books.First();
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
        public async Task GetBook_WithInValidId()
        {
            // Act
            var result = await _controller.GetBook(id: 0);
            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetBooks_WithValidBorrow()
        {
            // Act
            var result = await _controller.GetBooks(borrowed: true);
            var result2 = await _controller.GetBooks(borrowed: false);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedList = Assert.IsType<List<Book>>(okResult.Value);
            foreach (var item in returnedList)
            {
                Assert.IsType<Book>(item);
            }
            var okResult2 = Assert.IsType<OkObjectResult>(result2.Result);
            var returnedList2 = Assert.IsType<List<Book>>(okResult2.Value);
            foreach (var item in returnedList2)
            {
                Assert.IsType<Book>(item);
            }
        }

        [Fact]
        public async Task GetBooks_WithInValidBorrowed()
        {
            // Act
            var result = await _controller.GetBooks(borrowed: null);
            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
    }
}