using BookManagement.DbContexts;
using BookManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace BookManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowedBooksController : ControllerBase
    {
        private readonly BookLibraryContext _context;

        public BorrowedBooksController(BookLibraryContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Will get <see cref="BorrowedBook"/> entry based on provided Book ID
        /// </summary>
        /// <param name="id">Id of the book that the borrowed book entry refers to</param>
        /// <returns>An action result with the retrieved <see cref="BorrowedBook"/>, or BadRequest if the ID is invalid, 
        ///  or status code 500 with message body if something goes wrong</returns>
        [HttpGet("book")]
        [SwaggerOperation(Summary = "Retrieves a borrowed book entry by its BookID", Description = "Will get BorrowedBook entry based on provided Book ID")]
        public async Task<ActionResult<BorrowedBook>> GetByBook([FromQuery] int id)
        {
            if (id <= 0)
            {
                ModelState.AddModelError("Id", "Invalid book ID.");
                return BadRequest(ModelState);
            }
            try
            {
                List<BorrowedBook> result = await _context.BorrowedBooks.Where(x => x.BookID == id).ToListAsync();
                return Ok(result.First());
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while performing this action: UNHANDLED ERROR");
            }
        }
        /// <summary>
        /// Will get <see cref="BorrowedBook"/> entry based on provided User ID
        /// </summary>
        /// <param name="id">Id of the user that the borrowed book entry refers to</param>
        /// <returns>An action result with the retrieved <see cref="BorrowedBook"/>, or BadRequest if the ID is invalid, 
        ///  or status code 500 with message body if something goes wrong</returns>
        [HttpGet("user")]
        [SwaggerOperation(Summary = "Retrieves a borrowed book entry by its UserID", Description = "Will get BorrowedBook entry based on provided User ID")]
        public async Task<ActionResult<List<BorrowedBook>>> GetByUser([FromQuery] int id)
        {
            if (id <= 0)
            {
                ModelState.AddModelError("Id", "Invalid user ID.");
                return BadRequest(ModelState);
            }
            try
            {
                List<BorrowedBook> result = await _context.BorrowedBooks.Where(x => x.UserID == id).ToListAsync();
                return Ok(result.First());
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while performing this action: UNHANDLED ERROR");
            }
        }
        /// <summary>
        /// Will get List of all <see cref="BorrowedBook"/> entries
        /// </summary>
        /// <returns>An action result with the retrieved <see cref="List{BorrowedBook}"/>, or status code 500 with message body if something goes wrong</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Retrieves all borrowed book entries")]
        public async Task<ActionResult<List<BorrowedBook>>> GetAll()
        {
            try
            {
                List<BorrowedBook> result = await _context.BorrowedBooks.ToListAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while performing this action: UNHANDLED ERROR");
            }
        }
        /// <summary>
        /// Will add <see cref="BorrowedBook"/> entry to mark a book borrowed
        /// </summary>
        /// <param name="borrow">Borrow book entry details</param>
        /// <returns>An CreatedAtAction result with the newly creaed <see cref="BorrowedBook"/>, or BadRequest if the BorrowedBook details are invalid, 
        ///  or status code 500 with message body if something goes wrong</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "enters new entry to BorrowedBooks set", Description = "BorrowedBook netry details")]
        public async Task<ActionResult<BorrowedBook>> BorrowBook(BorrowedBook borrow)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                _context.BorrowedBooks.Add(borrow);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetByBook), new { id = borrow.BookID }, borrow);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while performing this action: UNHANDLED ERROR");
            }
        }
        /// <summary>
        /// Will deleted <see cref="BorrowedBook"/> by provided Book ID
        /// </summary>
        /// <param name="bookId">Id of the boook for which the borrowed book entry should be deleted</param>
        /// <returns>An Ok Action result with success message, or NotFound if the BorrowedBook entry does not exist, or BadRequest if the bookId is invalid, 
        ///  or status code 500 with message body if something goes wrong</returns>
        [HttpDelete]
        [SwaggerOperation(Summary = "Removes Borrowed book entry from the set", Description = "Id of the Book for which the entry was created")]
        public async Task<ActionResult<BorrowedBook>> Delete([FromQuery]int bookId)
        {
            if (bookId <= 0)
            {
                ModelState.AddModelError("Id", "Invalid book ID.");
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _context.BorrowedBooks.FindAsync(bookId);
                if (result == null)
                    return NotFound();

                _context.BorrowedBooks.Remove(result);
                await _context.SaveChangesAsync();
                return Ok("Successfuly deleted borrowed book entry with bookid=" + bookId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while performing this action: UNHANDLED ERROR");
            }
        }
        /// <summary>
        /// Will update the <see cref="BorrowedBook"/> entry based on provided ID and details
        /// </summary>
        /// <param name="bookId">Id of the <see cref="BorrowedBook"/> (Book ID) that shall be updated</param>
        /// <param name="borrow">updated <see cref="BorrowedBook"/> details</param>
        /// <returns>An Ok Action result with the updated <see cref="BorrowedBook"/>, or NotFound if the BorrowedBook entry does not exist,  or BadRequest if the bookId or borrow details are invalid, 
        ///  or status code 500 with message body if something goes wrong</returns>
        [HttpPut]
        public async Task<ActionResult<BorrowedBook>> UpdateBorrow(
            [FromQuery][SwaggerParameter("id", Description = "The Book ID that serves as a borrowed book entry's ID", Required = true)] int bookId,
            [SwaggerParameter("borrow", Description = "The updated Borrowed Book details", Required = true)] BorrowedBook borrow)
        {
            if (bookId <= 0)
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
                var find = await _context.BorrowedBooks.FindAsync(bookId);
                if (find == null)
                    return NotFound();

                find.BookID = borrow.BookID;
                find.UserID = borrow.UserID;
                find.BorrowedFrom = borrow.BorrowedFrom;
                find.BorrowedUntil = borrow.BorrowedUntil;

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
