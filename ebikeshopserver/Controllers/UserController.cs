using System;
using System.Security.Authentication;
using ebikeshopserver.Authentication;
using ebikeshopserver.Exceptions;
using ebikeshopserver.Models.User;
using ebikeshopserver.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace ebikeshopserver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private UsersService _usersService;

        public UserController(IMongoClient mongoClient)

        {
            _usersService = new UsersService(mongoClient);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers() //Make sure this is only accssiable to admin
        {
            List<User> result = await _usersService.GetUsersAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSpecificUser(string id) //Make sure this is only accessible to admins or users with an id the same as the requested user
        {
            try
            {
                User u = await _usersService.GetUserAsync(id);
                return Ok(u);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegisterNewUser([FromBody] User newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                object DTOuser = await _usersService.CreateUserAsync(newUser);
                return CreatedAtAction(nameof(GetSpecificUser), new { Id = newUser.Id }, newUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] User updatedUser) //Make sure this is only accessible to admins or users with an id the same as the requested user
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
        public async Task<IActionResult> DeleteUser(string id) //Make sure this is only accessible to admins or users with an id the same as the requested user
        {
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

