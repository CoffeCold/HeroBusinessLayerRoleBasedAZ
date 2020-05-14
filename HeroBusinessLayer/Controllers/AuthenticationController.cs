using Microsoft.AspNetCore.Mvc;
using Authentication.Services;
using Authentication.Entities;
using Authentication.Models;
using Microsoft.AspNetCore.Authorization;
using Authentication.Controllers;

namespace HeroBusinessLayerRoleBased.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : UsersController
    {
        private IUserService _userService;

        public AuthenticationController(IUserService userService)
            :base(userService)
        {
            _userService = userService;

        }


        [Authorize(Roles = "HeroesReader")]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }

        [Authorize(Roles = "HeroesReader")]
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