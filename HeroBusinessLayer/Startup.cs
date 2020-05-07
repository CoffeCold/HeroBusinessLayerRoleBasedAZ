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
using System.Text;
using Microsoft.EntityFrameworkCore;
using HeroBusinessLayerRoleBased.Helpers;
using HeroBusinessLayerRoleBased.Models;
using HeroBusinessLayerRoleBased.Services;
using Authentication.Services;
using Authentication.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace HeroBusinessLayerRoleBased
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

            services.AddControllers();

            // Appsettings in appsettings.json used for DI services
            var authSettingsSection = Configuration.GetSection("AuthenticationSettings");
            services.Configure<AuthenticationSettings>(authSettingsSection);
            var authSettings = authSettingsSection.Get<AuthenticationSettings>();

            // Connection string used for DI context
            var connectionStringsSection = Configuration.GetSection("ConnectionStrings");
            services.Configure<ConnectionStrings>(connectionStringsSection);
            var connectionstrings = connectionStringsSection.Get<ConnectionStrings>();


            var key = Encoding.ASCII.GetBytes(authSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });


            services.AddScoped<IHeroesService, HeroesService>();
            services.AddScoped<IUserService, UserService>();

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

 
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
