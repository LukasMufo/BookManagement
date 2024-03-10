using BookManagement.DbContexts;
using BookManagement.Models;
using BookManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace BookManagement.Controllers
{
    /// <summary>
    /// Controller for Books Model
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookLibraryContext _context;
        private readonly IBookService _bookService;
        private string StatusErrorMessage { get { return "An error occurred while performing this action: UNHANDLED ERROR"; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="BooksController"/> class.
        /// </summary>
        /// <param name="context">The database context for books.</param>
        /// <param name="bookService">The service for book operations.</param>
        public BooksController(BookLibraryContext context, IBookService bookService)
        {
            _context = context;
            _bookService = bookService;
        }
        /// <summary>
        /// Will return <see cref="Book"/> based on provided ID
        /// </summary>
        /// <param name="id">ID of the book to be found</param>
        /// <returns>An action result with the newly creaed <see cref="Book"/>, or BadRequest if ID is invalid, 
        ///  or NotFound if Book does not exist, or status code 500 with <see cref="StatusErrorMessage"/> if something goes wrong</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Retrieves a book by its ID", Description = "Returns the book based on provided ID.")]
        public async Task<ActionResult<Book>> GetBook([FromQuery] int id)
        {
            if (id<=0)
            {
                ModelState.AddModelError("Id", "Invalid book ID.");
                return BadRequest(ModelState);
            }
            try
            {
                var book = await _bookService.GetBookByIdAsync(id);
                if (book == null)
                    return NotFound();

                return Ok(book);
            }
            catch 
            {
                return StatusCode(500, StatusErrorMessage);
            }
        }
        /// <summary>
        /// Will return <see cref="Book"/> tha are either borrowed or not borrowed
        /// </summary>
        /// <param name="borrowed">returns Books based on whether they are borrowed or not</param>
        /// <returns>Returns Action result with <see cref="List{Book}"/>, or BadRequest if borrowed parameter is not correctly specified, or status code 500 with <see cref="StatusErrorMessage"/> if something goes wrong</returns>
        [HttpGet("borrowed={borrowed}")]
        [SwaggerOperation(Summary = "Retrieves a books that are either in BorrowedBooks or are not", Description = "true will retrieve borrowed books, false will retrieve available books")]
        public async Task<ActionResult<List<Book>>> GetBooks([Required] bool? borrowed)
        {
            if (!borrowed.HasValue)
            {
                ModelState.AddModelError(nameof(borrowed), "The 'borrowed' parameter is required.");
                return BadRequest(ModelState);
            }
            if (borrowed.Value != true && borrowed.Value != false)
            {
                ModelState.AddModelError(nameof(borrowed), "The 'borrowed' parameter must be either 'true' or 'false'.");
                return BadRequest(ModelState);
            }
            try
            {
                List<Book> result = await _bookService.GetBooksAsync(borrowed.Value);
                return Ok(result);
            }
            catch
            {
                return StatusCode(500, StatusErrorMessage);
            }
        }
        /// <summary>
        /// Will create a new <see cref="Book"/>
        /// </summary>
        /// <param name="book">New <see cref="Book"/> details</param>
        /// <returns>An action result with the newly creaed <see cref="Book"/>, or BadRequest if the book details are invoalid, 
        ///  or status code 500 with <see cref="StatusErrorMessage"/> if something goes wrong</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Will create a new book", Description = "book model that shall be cerated")]
        public async Task<ActionResult<Book>> Create(Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                Book createdBook = await _bookService.CreateBookAsync(book);
                return CreatedAtAction(nameof(GetBook), new { id = createdBook.ID }, createdBook);
            }
            catch
            {
                return StatusCode(500, StatusErrorMessage);
            }
            
        }
        /// <summary>
        /// Will delete book based on provided ID
        /// </summary>
        /// <param name="id">Id of the book that shall be deleted</param>
        /// <returns>An action result with the newly creaed <see cref="Book"/>, NotFound if such bok does not exist, BadRequest if ID is not valid
        ///  or status code 500 with <see cref="StatusErrorMessage"/> if something goes wrong</returns>
        [HttpDelete]
        [SwaggerOperation(Summary = "Will delete book by its id", Description = "ID of the book that shall be useed")]
        public async Task<ActionResult<Book>> DeleteBook([FromQuery] int id)
        {
            if (id <= 0)
            {
                ModelState.AddModelError("Id", "Invalid book ID.");
                return BadRequest(ModelState);
            }
            try
            {
                var deletedBook = await _bookService.DeleteBookAsync(id);
                if (deletedBook == null)
                    return NotFound(); // Return NotFound if the book does not exist

                return Ok("Successfully deleted book with id=" + id);
            }
            catch
            {
                return StatusCode(500, StatusErrorMessage);
            }
            
        }
        /// <summary>
        /// Will update a book based on provided ID
        /// </summary>
        /// <param name="id">ID of the book that shall be updated</param>
        /// <param name="book">book model with properties that shall be set to the updated book</param>
        /// <returns>An action result with the Updated <see cref="Book"/>, NotFound if such book does not exist, BadRequest if ID or the book details are not valid
        ///  or status code 500 with <see cref="StatusErrorMessage"/> if something goes wrong</returns>
        [HttpPut]
        [SwaggerOperation(Summary = "Updates an existing book", Description = "Updates an existing book.")]
        public async Task<ActionResult<Book>> UpdateBook(
            [FromQuery][SwaggerParameter("id", Description = "The ID of the book to update", Required = true)] int id,
            [SwaggerParameter("book", Description = "The updated book data", Required = true)] Book book)
        {
            if (id <= 0)
            {
                ModelState.AddModelError("Id", "Invalid book ID.");
                return BadRequest(ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var deletedBook = await _bookService.DeleteBookAsync(id);
                if (deletedBook == null)
                    return NotFound(); // Return NotFound if the book does not exist

                return Ok("Successfully deleted book with id=" + id);
            }
            catch
            {
                return StatusCode(500, StatusErrorMessage);
            }

        }
        
    }
}
