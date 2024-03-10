using BookManagement.DbContexts;
using BookManagement.Models;
using BookManagement.Services;
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
        private readonly IBorrowedService _borrowedBookService;
        private string StatusErrorMessage { get { return "An error occurred while performing this action: UNHANDLED ERROR"; } }

        public BorrowedBooksController(BookLibraryContext context, IBorrowedService borrowedBookService)
        {
            _context = context;
            _borrowedBookService = borrowedBookService;
        }
        /// <summary>
        /// Will get <see cref="BorrowedBook"/> entry based on provided Book ID
        /// </summary>
        /// <param name="id">Id of the book that the borrowed book entry refers to</param>
        /// <returns>An action result with the retrieved <see cref="BorrowedBook"/>, or BadRequest if the ID is invalid, 
        ///  or status code 500 with <see cref="StatusErrorMessage"/> if something goes wrong</returns>
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
                var borrowedBook = await _borrowedBookService.GetByBookAsync(id);
                if (borrowedBook == null)
                {
                    return NotFound();
                }
                return Ok(borrowedBook);
            }
            catch
            {
                return StatusCode(500, StatusErrorMessage);
            }
        }
        /// <summary>
        /// Will get <see cref="BorrowedBook"/> entry based on provided User ID
        /// </summary>
        /// <param name="id">Id of the user that the borrowed book entry refers to</param>
        /// <returns>An action result with the retrieved <see cref="BorrowedBook"/>, or BadRequest if the ID is invalid, 
        ///  or status code 500 with <see cref="StatusErrorMessage"/> if something goes wrong</returns>
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
                var borrowedBooks = await _borrowedBookService.GetByUserAsync(id);
                if (borrowedBooks == null || borrowedBooks.Count == 0)
                {
                    return NotFound();
                }

                return Ok(borrowedBooks);
            }
            catch
            {
                return StatusCode(500, StatusErrorMessage);
            }
        }
        /// <summary>
        /// Will get List of all <see cref="BorrowedBook"/> entries
        /// </summary>
        /// <returns>An action result with the retrieved <see cref="List{BorrowedBook}"/>, or status code 500 with <see cref="StatusErrorMessage"/> if something goes wrong</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Retrieves all borrowed book entries")]
        public async Task<ActionResult<List<BorrowedBook>>> GetAll()
        {
            try
            {
                var borrowedBooks = await _borrowedBookService.GetAllAsync();
                if (borrowedBooks == null || borrowedBooks.Count == 0)
                {
                    return NotFound();
                }

                return Ok(borrowedBooks);
            }
            catch
            {
                return StatusCode(500, StatusErrorMessage);
            }
        }
        /// <summary>
        /// Will add <see cref="BorrowedBook"/> entry to mark a book borrowed
        /// </summary>
        /// <param name="borrow">Borrow book entry details</param>
        /// <returns>An CreatedAtAction result with the newly creaed <see cref="BorrowedBook"/>, or BadRequest if the BorrowedBook details are invalid, 
        ///  or status code 500 with <see cref="StatusErrorMessage"/> if something goes wrong</returns>
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
                var borrowedBook = await _borrowedBookService.BorrowBookAsync(borrow);
                return CreatedAtAction(nameof(GetByBook), new { id = borrow.BookID }, borrowedBook);
            }
            catch
            {
                return StatusCode(500, StatusErrorMessage);
            }
        }
        /// <summary>
        /// Will deleted <see cref="BorrowedBook"/> by provided Book ID
        /// </summary>
        /// <param name="bookId">Id of the boook for which the borrowed book entry should be deleted</param>
        /// <returns>An Ok Action result with success message, or NotFound if the BorrowedBook entry does not exist, or BadRequest if the bookId is invalid, 
        ///  or status code 500 with <see cref="StatusErrorMessage"/> if something goes wrong</returns>
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
                var borrowedBook = await _borrowedBookService.DeleteAsync(bookId);
                return Ok("Successfully deleted borrowed book entry with bookid=" + bookId);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch
            {
                return StatusCode(500, StatusErrorMessage);
            }
        }
        /// <summary>
        /// Will update the <see cref="BorrowedBook"/> entry based on provided ID and details
        /// </summary>
        /// <param name="bookId">Id of the <see cref="BorrowedBook"/> (Book ID) that shall be updated</param>
        /// <param name="borrow">updated <see cref="BorrowedBook"/> details</param>
        /// <returns>An Ok Action result with the updated <see cref="BorrowedBook"/>, or NotFound if the BorrowedBook entry does not exist,  or BadRequest if the bookId or borrow details are invalid, 
        ///  or status code 500 with <see cref="StatusErrorMessage"/> if something goes wrong</returns>
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
                var updatedBorrowedBook = await _borrowedBookService.UpdateAsync(bookId, borrow);
                return Ok(updatedBorrowedBook);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch
            {
                return StatusCode(500, StatusErrorMessage);
            }
                
        }
    }
}
