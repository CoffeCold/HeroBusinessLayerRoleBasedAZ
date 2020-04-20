using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HeroBusinessLayer.Services;
using HeroBusinessLayer.Models;

namespace HeroBusinessLayer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HeroesController : ControllerBase
    {
        private readonly ILogger<HeroesController> _logger;
        private IHeroesService _heroesService;

        public HeroesController(ILogger<HeroesController> logger, IHeroesService heroesService )
        {
            _logger = logger;
            _heroesService = heroesService;
         
        }
        
        [HttpGet]
        public IEnumerable<Heroes> Get()
        {
            _logger.LogInformation("Get heroes called"); 
            return _heroesService.GetAll();
        }
    }
}
