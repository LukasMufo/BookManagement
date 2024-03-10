using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookManagement.DbContexts;
using BookManagement.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Swashbuckle.AspNetCore.Annotations;

namespace BookManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly BookLibraryContext _context;

        public UsersController(BookLibraryContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Will get <see cref="User"/> base on provided ID
        /// </summary>
        /// <param name="id">ID of the user that should be returned</param>
        /// <returns>An action result with the requested <see cref="User"/>, or BadRequest if ID is not valid, or NotFound if <see cref="User"/> does not exists,
        ///  or status code 500 with message body if something goes wrong</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Retrieves a user by ID", Description = "Returns the user based on provided ID.")]
        public async Task<ActionResult<User>> GetUser([FromQuery] int id)
        {
            if (id <= 0)
            {
                ModelState.AddModelError("Id", "Invalid user ID.");
                return BadRequest(ModelState);
            }
            try
            {
                var User = await _context.Users.FindAsync(id);
                if (User == null)
                    return NotFound();

                return Ok(User);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while performing this action: UNHANDLED ERROR");
            }

        }
        /// <summary>
        /// Will get List of all <see cref="User"/> entries
        /// </summary>
        /// <returns>An action result with the requested List of <see cref="User"/>, or status code 500 with message body if something goes wrong</returns>
        [HttpGet("All")]
        [SwaggerOperation(Summary = "Retrieves all users")]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            try
            {
                List<User> result = await _context.Users.ToListAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while performing this action: UNHANDLED ERROR");
            }
        }
        /// <summary>
        /// Will create a new <see cref="User"/>
        /// </summary>
        /// <param name="User">New <see cref="User"/> details</param>
        /// <returns>An CreatedAtAction result with the newly created <see cref="User"/>, or BadRequest if user details are not valid, 
        ///  or status code 500 with message body if something goes wrong</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Creates new user", Description = "User details for the newly created user")]
        public async Task<ActionResult<User>> CreateUser(User User)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _context.Users.AddAsync(User);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetUser), new { id = User.ID }, User);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while performing this action: UNHANDLED ERROR");
            }
        }
        /// <summary>
        /// Will Delete <see cref="User"/> by the provided ID
        /// </summary>
        /// <param name="id">Id of the <see cref="User"/> that shall be deleted</param>
        /// <returns>An Ok action result if delete succedes, or BadRequest if ID is not valid, or NotFound if <see cref="User"/> does not exists, or status code 500 with message body if something goes wrong</returns>
        [HttpDelete]
        [SwaggerOperation(Summary = "Deletes the user", Description = "Deletes the user based on ID")]
        public async Task<ActionResult<User>> DeleteUser([FromQuery] int id)
        {
            if (id <= 0)
            {
                ModelState.AddModelError("Id", "Invalid user ID.");
                return BadRequest(ModelState);
            }
            try
            {
                var User = _context.Users.Find(id);
                if (User == null)
                    return NotFound();

                _context.Users.Remove(User);
                await _context.SaveChangesAsync();
                return Ok("Successfuly deleted User with id=" + id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while performing this action: UNHANDLED ERROR");
            }
        }
        /// <summary>
        /// Will Update <see cref="User"/> of the provided ID, by the provided details
        /// </summary>
        /// <param name="id">ID of the <see cref="User"/> that shall be deleted</param>
        /// <param name="User"><see cref="User"/> details</param>
        /// <returns>An Ok action result with the updated <see cref="User"/>, or BadRequest if ID or the user details are not valid, or Not Found if such user does not exist, 
        ///  or status code 500 with message body if something goes wrong</returns>
        [HttpPut]
        public async Task<ActionResult<User>> UpdateUser([FromQuery] int id, User User)
        {
            if (id <= 0)
            {
                ModelState.AddModelError("Id", "Invalid user ID.");
                return BadRequest(ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var find = await _context.Users.FindAsync(id);
                if (find == null)
                    return NotFound();

                find.Name = User.Name;
                find.Email = User.Email;

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
