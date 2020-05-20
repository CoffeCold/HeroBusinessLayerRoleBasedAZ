using Microsoft.AspNetCore.Mvc;
using Authentication.Services;
using Authentication.Entities;
using Authentication.Models;
using Microsoft.AspNetCore.Authorization;
using Authentication.Controllers;
using Microsoft.Extensions.Logging;

namespace HeroBusinessLayerRoleBased.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : UsersController
    {
        private IUserService _userService;
        private readonly ILogger<AuthenticationController> _logger;
        public AuthenticationController( IUserService userService, ILogger<AuthenticationController> logger)
            :base(userService, logger)
        {
            _userService = userService;
            _logger = logger; 
        }


        [Authorize(Roles = "HeroesReader")]
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAll()
        {
            _logger.LogInformation("GetAll users called");
            var users = _userService.GetAll();
            return Ok(users);
        }

        [Authorize(Roles = "HeroesReader")]
        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {

            // only allow admins to access other user records
            var currentUserId = int.Parse(User.Identity.Name);
            if (id != currentUserId && !User.IsInRole(RoleTypes.HeroesWriter))
                return Forbid();

            var user = _userService.GetById(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }
    }
}