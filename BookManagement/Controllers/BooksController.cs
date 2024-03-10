using BookManagement.DbContexts;
using BookManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace BookManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookLibraryContext _context;

        public BooksController(BookLibraryContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Will return <see cref="Book"/> based on provided ID
        /// </summary>
        /// <param name="id">ID of the book to be found</param>
        /// <returns>An action result with the newly creaed <see cref="Book"/>, or BadRequest if ID is invalid, 
        ///  or NotFound if Book does not exist, or status code 500 with message body if something goes wrong</returns>
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
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                    return NotFound();

                return Ok(book);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while performing this action: UNHANDLED ERROR");
            }
        }
        /// <summary>
        /// Will return <see cref="Book"/> tha are either borrowed or not borrowed
        /// </summary>
        /// <param name="borrowed">returns Books based on whether they are borrowed or not</param>
        /// <returns>Returns Action result with <see cref="List{Book}"/>, or BadRequest if borrowed parameter is not correctly specified, or status code 500 with message body if something goes wrong</returns>
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
                List<BorrowedBook> BorrowedBooks = await _context.BorrowedBooks.ToListAsync();
                List<Book> Books = await _context.Books.ToListAsync();
                List<Book> result = Books.Where(x =>
                (borrowed.Value && BorrowedBooks.Any(b => b.BookID == x.ID)) ||
                (!borrowed.Value && !BorrowedBooks.Any(b => b.BookID == x.ID))
                ).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while performing this action: UNHANDLED ERROR");
            }
        }
        /// <summary>
        /// Will create a new <see cref="Book"/>
        /// </summary>
        /// <param name="book">New <see cref="Book"/> details</param>
        /// <returns>An action result with the newly creaed <see cref="Book"/>, or BadRequest if the book details are invoalid, 
        ///  or status code 500 with message body if something goes wrong</returns>
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
                await _context.Books.AddAsync(book);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetBook), new { id = book.ID }, book);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while performing this action: UNHANDLED ERROR");
            }
            
        }
        /// <summary>
        /// Will delete book based on provided ID
        /// </summary>
        /// <param name="id">Id of the book that shall be deleted</param>
        /// <returns>An action result with the newly creaed <see cref="Book"/>, NotFound if such bok does not exist, BadRequest if ID is not valid
        ///  or status code 500 with message body if something goes wrong</returns>
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
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                    return NotFound();

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                return Ok("Successfuly deleted book with id=" + id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while performing this action: UNHANDLED ERROR");
            }
            
        }
        /// <summary>
        /// Will update a book based on provided ID
        /// </summary>
        /// <param name="id">ID of the book that shall be updated</param>
        /// <param name="book">book model with properties that shall be set to the updated book</param>
        /// <returns>An action result with the Updated <see cref="Book"/>, NotFound if such book does not exist, BadRequest if ID or the book details are not valid
        ///  or status code 500 with message body if something goes wrong</returns>
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
                var find = await _context.Books.FindAsync(id);
                if (find == null)
                    return NotFound();

                find.Title = book.Title;
                find.Author = book.Author;

                _context.Entry(find).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(find);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while performing this action: UNHANDLED ERROR");
            }

        }
        
    }
}
