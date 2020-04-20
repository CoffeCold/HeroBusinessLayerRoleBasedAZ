using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HeroBusinessLayer.Models;
using HeroBusinessLayer.Helpers;
using Microsoft.Extensions.Options;


namespace HeroBusinessLayer.Services
{
    public interface IHeroesService
    {
        IEnumerable<Heroes> GetAll();
    }

    internal class HeroesService : IHeroesService
    {
 
        private AngularHeroesContext _context;
        private readonly AppSettings _appSettings;

        public HeroesService(IOptions<AppSettings> appSettings, AngularHeroesContext context)
        {
            _appSettings = appSettings.Value;
            _context = context;
        }

         public IEnumerable<Heroes> GetAll()
        {
            List<Heroes> Heroes = new List<Heroes>();
            using (_context)
            {
                Heroes = _context.Heroes.ToList();
            }
            return Heroes;
        }
    }
}
