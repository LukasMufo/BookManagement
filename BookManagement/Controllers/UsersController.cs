using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookManagement.DbContexts;
using BookManagement.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Swashbuckle.AspNetCore.Annotations;
using BookManagement.Services;

namespace BookManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly BookLibraryContext _context;
        private readonly IUserService _userService;
        private string StatusErrorMessage { get { return "An error occurred while performing this action: UNHANDLED ERROR"; } }

        public UsersController(BookLibraryContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }
        /// <summary>
        /// Will get <see cref="User"/> base on provided ID
        /// </summary>
        /// <param name="id">ID of the user that should be returned</param>
        /// <returns>An action result with the requested <see cref="User"/>, or BadRequest if ID is not valid, or NotFound if <see cref="User"/> does not exists,
        ///  or status code 500 with <see cref="StatusErrorMessage"/> if something goes wrong</returns>
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
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound();

                return Ok(user);
            }
            catch
            {
                return StatusCode(500, StatusErrorMessage);
            }

        }
        /// <summary>
        /// Will get List of all <see cref="User"/> entries
        /// </summary>
        /// <returns>An action result with the requested List of <see cref="User"/>, or status code 500 with <see cref="StatusErrorMessage"/> if something goes wrong</returns>
        [HttpGet("All")]
        [SwaggerOperation(Summary = "Retrieves all users")]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            try
            {
                var users = await _userService.GetUsersAsync();
                return Ok(users);
            }
            catch
            {
                return StatusCode(500, StatusErrorMessage);
            }
        }
        /// <summary>
        /// Will create a new <see cref="User"/>
        /// </summary>
        /// <param name="user">New <see cref="User"/> details</param>
        /// <returns>An CreatedAtAction result with the newly created <see cref="User"/>, or BadRequest if user details are not valid, 
        ///  or status code 500 with <see cref="StatusErrorMessage"/> if something goes wrong</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Creates new user", Description = "User details for the newly created user")]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var createdUser = await _userService.CreateUserAsync(user);
                return CreatedAtAction(nameof(GetUser), new { id = createdUser.ID }, createdUser);
            }
            catch
            {
                return StatusCode(500, StatusErrorMessage);
            }
        }
        /// <summary>
        /// Will Delete <see cref="User"/> by the provided ID
        /// </summary>
        /// <param name="id">Id of the <see cref="User"/> that shall be deleted</param>
        /// <returns>An Ok action result if delete succedes, or BadRequest if ID is not valid, or NotFound if <see cref="User"/> does not exists, or status code 500 with <see cref="StatusErrorMessage"/> if something goes wrong</returns>
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
                var deletedUser = await _userService.DeleteUserAsync(id);
                return Ok($"Successfully deleted User with id={deletedUser.ID}");
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
        /// Will Update <see cref="User"/> of the provided ID, by the provided details
        /// </summary>
        /// <param name="id">ID of the <see cref="User"/> that shall be deleted</param>
        /// <param name="User"><see cref="User"/> details</param>
        /// <returns>An Ok action result with the updated <see cref="User"/>, or BadRequest if ID or the user details are not valid, or Not Found if such user does not exist, 
        ///  or status code 500 with <see cref="StatusErrorMessage"/> if something goes wrong</returns>
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
                var updatedUser = await _userService.UpdateUserAsync(id, User);
                if (updatedUser == null)
                {
                    // Handle invalid model state
                    return BadRequest(ModelState);
                }

                return Ok(updatedUser);
            }
            catch
            {
                return StatusCode(500, StatusErrorMessage);
            }
            
        }
    }
}
