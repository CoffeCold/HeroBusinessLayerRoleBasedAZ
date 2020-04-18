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
        private readonly AppSettings _appSettings;

        public HeroesService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public HeroesService()
        {
        }

        public IEnumerable<Heroes> GetAll()
        {
            List<Heroes> Heroes = new List<Heroes>();
            using (AngularHeroesContext context = new AngularHeroesContext())
            {
                Heroes = context.Heroes.ToList();
            }
            return Heroes;
        }
    }
}
