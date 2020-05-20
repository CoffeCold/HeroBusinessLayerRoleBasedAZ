using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Authentication.Services;
using Authentication.Entities;
using Authentication.Models;
using Microsoft.Extensions.Logging;

namespace Authentication.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
 
        private IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;

        }
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthenticateModel model)
        {
            _logger.LogInformation("Authenticate users called");

            var user = _userService.Authenticate(model.Username, model.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });
            _logger.LogInformation("Authenticate user returned");

            return Ok(user);
        }

    }



}
