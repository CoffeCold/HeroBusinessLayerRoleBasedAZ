using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Authentication.Controllers;
using Authentication.Services;
using Authentication.Entities;
using Authentication.Models;
using Microsoft.AspNetCore.Authorization;

namespace HeroBusinessLayerRoleBased.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : UsersController
    {
        public AuthenticationController(IUserService userService)
           :base(userService)
        {
           
        }
    }
}