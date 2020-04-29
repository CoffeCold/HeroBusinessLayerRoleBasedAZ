using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HeroBusinessLayer.Services;
using HeroBusinessLayer.Models;
using System.Reflection.Metadata.Ecma335;

namespace HeroBusinessLayer.Controllers
{
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

        [HttpGet]
        public IActionResult Get(int? id, string name)
        {
            if (!String.IsNullOrEmpty(name))
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

        [HttpDelete("{id}")]
        public async Task<ActionResult<Heroes>> DeleteHero(int? id)
        {
            _logger.LogInformation("Delete heroes called for id {0}", id);
            Heroes hero =  await _heroesService.DeleteOneByID(id);
            if (hero != null)
            {
                return hero;
            }
            else
            {               
                return NotFound();
            }
        }

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
