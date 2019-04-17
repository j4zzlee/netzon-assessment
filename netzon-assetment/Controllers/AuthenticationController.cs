using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using netzon_assetment.Models;
using netzon_assetment.Services;
using netzon_assetment.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netzon_assetment.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private IUserService _userService;

        public AuthenticationController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Authenticate a user and get the access token.
        /// </summary>
        /// <returns>An updated user</returns>
        /// <response code="200">Success</response>
        /// <response code="400">Username or password is incorrect</response>
        /// <response code="422">Validation failed</response>  
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        public IActionResult Authenticate([FromBody]UserLoginViewModel userParam)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }
            var user = _userService.Authenticate(userParam.Email, userParam.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });
            user.Password = null;
            return Ok(user);
        }

        /// <summary>
        /// Make a user be an admin
        /// </summary>
        /// <returns>An updated user</returns>
        /// <response code="200">Success</response>
        /// <response code="400">The resource is not found</response>
        /// <response code="422">Validation failed</response>  
        [Authorize(Roles = "ADMIN")]
        [HttpPost("admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        public IActionResult UpgradeUser([FromBody]UserUpgradeViewModel userParam)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }
            if (userParam.UserID == 0)
            {
                return BadRequest(new { message = "UserID is required" });
            }
            var user = _userService.GetById(userParam.UserID.Value);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            _userService.AddRole(user, "ADMIN");
            var response = _userService.GetById(userParam.UserID.Value);
            response.Password = null;
            response.Roles = null; // avoid self references loop
            return Ok(response);
        }
    }
}
