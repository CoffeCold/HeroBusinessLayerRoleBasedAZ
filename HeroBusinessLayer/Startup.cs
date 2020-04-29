using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using HeroBusinessLayer.Services;
using HeroBusinessLayer.Models;
using HeroBusinessLayer.Helpers;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace HeroBusinessLayer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        readonly string AllThinkableOrigins = "_myAllThinkableOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: AllThinkableOrigins,
                                  builder =>
                                  {
                                      builder.SetIsOriginAllowedToAllowWildcardSubdomains()
      .WithOrigins("*")
      .AllowAnyMethod()
      .AllowAnyHeader()
      .Build();
                                  });
            });

            // Appsettings in appsettings.json used for DI services
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();

            // Connection string used for DI context
            var connectionStringsSection = Configuration.GetSection("ConnectionStrings");
            services.Configure<ConnectionStrings>(connectionStringsSection);
            var connectionstrings = connectionStringsSection.Get<ConnectionStrings>();


            services.AddScoped<IHeroesService, HeroesService>();

            services.AddControllers();
            services.AddDbContext<AngularHeroesContext>(options =>
                options.UseSqlServer(connectionstrings.AngularHeroesDbConstr)
            );


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(AllThinkableOrigins);

            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
