using System.Security.Authentication;
using System.Security.Claims;
using ebikeshopserver.Authentication;
using ebikeshopserver.Authorization;
using ebikeshopserver.Exceptions;
using ebikeshopserver.Models.User;
using ebikeshopserver.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace ebikeshopserver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private UsersService _usersService;

        public UserController(IMongoClient mongoClient)
        {
            _usersService = new UsersService(mongoClient);
        }

        [HttpGet]
        [Authorize("MustBeAdmin")]
        public async Task<IActionResult> GetAllUsers() //only available to admin
        {
            foreach (var claim in HttpContext.User.Claims) Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
            List<User> result = await _usersService.GetUsersAsync();
            return Ok(result);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = HttpContext.User.FindFirstValue("id");
            Console.WriteLine(userId);
            if (userId == null)
            {
                return Unauthorized("User ID not found in the token.");
            }

            try
            {
                User u = await _usersService.GetUserAsync(userId);
                return Ok(u);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterNewUser([FromBody] User newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                object DTOuser = await _usersService.CreateUserAsync(newUser);
                return CreatedAtAction(nameof(GetCurrentUser), new { Id = newUser.Id }, DTOuser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(string id, [FromBody] User updatedUser) //Make sure this is only accessible to admins or users with an id the same as the requested user
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (isAuthorized, actionResult) = AuthorizationHelper.AuthorizeUserOrAdmin(HttpContext,id);
            if (!isAuthorized)
            {
                return actionResult!;
            }

            try
            {
                User newUser = await _usersService.EditUserAsync(id, updatedUser);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(string id) //Make sure this is only accessible to admins or users with an id the same as the requested user
        {
            var (isAuthorized, actionResult) = AuthorizationHelper.AuthorizeUserOrAdmin(HttpContext,id);
            if (!isAuthorized)
            {
                return actionResult!;
            }

            try
            {
                await _usersService.DeleteUserAsync(id);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            return NoContent();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            try
            {
                User? u = await _usersService.LoginAsync(loginModel);
                string token = JwtHelper.GenerateAuthToken(u);
                return Ok(token);
            }
            catch (AuthenticationException ex)
            {
                Console.WriteLine(ex.Message);
                return Unauthorized("Email or Password wrong");

            }
        }
    }
}

