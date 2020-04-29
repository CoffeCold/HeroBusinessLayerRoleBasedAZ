using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HeroBusinessLayer.Models;
using HeroBusinessLayer.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace HeroBusinessLayer.Services
{
    public interface IHeroesService
    {
        IEnumerable<Heroes> GetAll();
        Heroes GetOneById(int? id);
        Heroes GetOneByName(string name);
        Task<Heroes> AddOne(Heroes hero);
        Task<Heroes> DeleteOneByID(int? id);
        Task<Heroes> UpdateOne(Heroes hero);
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
            Heroes = _context.Heroes.ToList();
            return Heroes;
        }
        public Heroes GetOneById(int? id)
        {
            Heroes hero;
            hero = _context.Heroes.FirstOrDefault(a => a.Id == id);
            return hero;
        }
        public Heroes GetOneByName(string name)
        {
            Heroes hero;
            hero = _context.Heroes.FirstOrDefault(a => a.Name == name);
            return hero;
        }

        public async Task<Heroes> AddOne(Heroes hero)
        {
            Heroes lasthero =  _context.Heroes.OrderByDescending(u => u.Id).FirstOrDefault();
            if (lasthero != null)
            {
                hero.Id = lasthero.Id + 1; 
                _context.Heroes.Add(hero);
                if (await _context.SaveChangesAsync() > 0)
                {
                    return  hero;
                }
            }
            return null; 
        }

        public async Task<Heroes> UpdateOne(Heroes hero)
        {
            Heroes hero_toupdate = GetOneById(hero.Id);
            if (hero_toupdate != null)
            {
                hero_toupdate.Name = hero.Name;
            }
            if (await _context.SaveChangesAsync() > 0)
            {
                return hero_toupdate;
            }
            return null;
        }

        public async Task<Heroes> DeleteOneByID(int? id)
        {
            Heroes hero = GetOneById(id);
            if (hero != null)
            {
                _context.Remove(hero);
                if (await _context.SaveChangesAsync() > 0)
                {
                    return hero;
                }
            }
            return null;
        }








    }
}
