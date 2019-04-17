using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using netzon_assetment.Services;
using netzon_assetment.ViewModels;
using System.Linq;
using System.Security.Claims;

namespace netzon_assetment.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Searchs users via fistname and lastname
        /// </summary>
        /// <returns>List of users</returns>
        /// <response code="200">Success</response>
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        [ProducesResponseType(200)]
        public IActionResult GetAll([FromQuery] string q, [FromQuery] int? limit, [FromQuery] int? offset)
        {
            var users = _userService.Search(q, limit, offset).Select(u => { u.Password = null; return u; });
            return Ok(users);
        }

        /// <summary>
        /// Gets user by ID.
        /// </summary>
        /// <returns>An updated user</returns>
        /// <response code="200">Success</response>
        /// <response code="403">Forbidden if a user is not currently logged in user nor admin</response>  
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        public IActionResult GetById(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.FindAll(ClaimTypes.Role).Any(r => r.Value == "ADMIN");
            if (userId != id.ToString() && !isAdmin)
            {
                return Forbid();
            }
            var user = _userService.GetById(id);
            user.Password = null;
            return Ok(user);
        }

        /// <summary>
        /// Registers a user
        /// </summary>
        /// <returns>An updated user</returns>
        /// <response code="200">Success</response>
        /// <response code="400">The use is exists</response>
        /// <response code="422">Validation failed</response>  
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult Register([FromBody] UserRegistrationViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }
            var exists = _userService.FindByEmail(request.Email);
            if (exists != null)
            {
                return BadRequest("The user is already exists.");
            }

            var user = _userService.Register(request.Firstname, request.Lastname, request.Email, request.Password);
            user.Password = null;
            return Ok(user);
        }

        /// <summary>
        /// Updates a user firstname and lastname
        /// </summary>
        /// <returns>An updated user</returns>
        /// <response code="200">Success</response>
        /// <response code="400">The resource is not found</response>
        /// <response code="403">Forbidden if a user is not currently logged in user nor admin</response>  
        /// <response code="422">Validation failed</response>  
        [Authorize]
        [HttpPatch()]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(422)]
        public IActionResult Update([FromBody] UserUpdateViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.FindAll(ClaimTypes.Role).Any(r => r.Value == "ADMIN");
            if (userId != request.UserID?.ToString() && !isAdmin)
            {
                return Forbid();
            }
            var userToUpdate = _userService.GetById(request.UserID.Value);
            if (userToUpdate == null)
            {
                return NotFound("The user is not exists.");
            }

            userToUpdate = _userService.UpdateProfile(userToUpdate, request.FirstName, request.LastName);
            userToUpdate.Password = null;
            return Ok(userToUpdate);
        }

        /// <summary>
        /// Deletes a user by ID
        /// </summary>
        /// <returns></returns>
        /// <response code="204">The resource is deleted</response>
        /// <response code="400">The resource is not found</response>
        /// <response code="403">Forbidden if a user is not currently logged in user nor admin</response>  
        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public IActionResult Delete(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.FindAll(ClaimTypes.Role).Any(r => r.Value == "ADMIN");
            if (userId != id.ToString() && !isAdmin)
            {
                return Forbid();
            }
            var userToBeDeleted = _userService.GetById(id);
            if (userToBeDeleted == null)
            {
                return NotFound("The user is not exists.");
            }
            _userService.Delete(userToBeDeleted);
            //user = _userService.UpdateProfile(user, request.FirstName, request.LastName);
            return NoContent();
        }
    }
}
