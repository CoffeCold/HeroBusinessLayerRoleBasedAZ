using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Reflection.Metadata.Ecma335;
using HeroBusinessLayerRoleBased.Models;
using HeroBusinessLayerRoleBased.Services;
using Microsoft.AspNetCore.Authorization;

namespace HeroBusinessLayerRoleBased.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class HeroesController : ControllerBase
    {
        private readonly ILogger<HeroesController> _logger;
        private IHeroesService _heroesService;

        public HeroesController(ILogger<HeroesController> logger, IHeroesService heroesService)
        {
            _logger = logger;
            _heroesService = heroesService;

        }

        //[Authorize(Roles = "HeroesReader")]
        [HttpGet]
        public IActionResult Get(int? id, string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                _logger.LogInformation("GetById heroes called for name {0}", name);

                Heroes hero = _heroesService.GetOneByName(name);
                if (hero != null) { return Ok(hero); } else { return NotFound(); }
            }
            if (id != null)
            {
                _logger.LogInformation("GetById heroes called for id {0}", id);
                Heroes hero = _heroesService.GetOneById(id);
                if (hero != null) { return Ok(hero); } else { return NotFound(); }
            }
            _logger.LogInformation("Get heroes called");
            IEnumerable<Heroes> allheroes = _heroesService.GetAll();
            if (allheroes != null)
            {
                return Ok(allheroes);
            }
            return NotFound();
        }



        //[Authorize(Roles = "HeroesReader")]
        [HttpGet("{id}")]
        public IActionResult GetById(int? id)
        {
            _logger.LogInformation("GetById heroes called for id {0}", id);
            Heroes hero = _heroesService.GetOneById(id);
            if (hero != null)
            {
                return Ok(hero);
            }
            return NotFound();

        }


        //[Authorize(Roles = "HeroesWriter")]
        [HttpPost]
        public async Task<ActionResult<Heroes>> CreateHero(Heroes hero)
        {
            _logger.LogInformation("Post heroes called for id {0}", hero.Id);
            await _heroesService.AddOne(hero);
            if (hero != null)
            {
                return CreatedAtAction(nameof(GetById), new { id = hero.Id }, hero);
            }
            else
            {
                // internal server error
                return new StatusCodeResult(500);
            }
        }

        //[Authorize(Roles = "HeroesWriter")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Heroes>> DeleteHero(int? id)
        {
            _logger.LogInformation("Delete heroes called for id {0}", id);
            Heroes hero = await _heroesService.DeleteOneByID(id);
            if (hero != null)
            {
                return hero;
            }
            else
            {
                return NotFound();
            }
        }

        //[Authorize(Roles = "HeroesWriter")]
        [HttpPut]
        public async Task<ActionResult<Heroes>> UpdateHero(Heroes hero)
        {
            _logger.LogInformation("Put heroes called for id {0}", hero.Id);
            await _heroesService.UpdateOne(hero);
            if (hero != null)
            {
                return hero;
            }
            else
            {
                return NotFound();
            }
        }



    }
}
