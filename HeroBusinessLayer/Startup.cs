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
        public IConfiguration Configuration { get; }
        public AuthenticationSettings checkAuthsettings; // just for logging purposes
        public Startup( IConfiguration configuration)
        {
            Configuration = configuration;
       }

     

        readonly string AllThinkableOrigins = "_myAllThinkableOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        // This method does not allow an injected logger in .net Core 3.x, so logging is not possible 
        public void ConfigureServices(IServiceCollection services)
        {

            //      services.AddCors(options =>
            //      {
            //          options.AddPolicy(name: AllThinkableOrigins,
            //                            builder =>
            //                            {
            //                                builder.SetIsOriginAllowedToAllowWildcardSubdomains()
            //.WithOrigins("*")
            //.AllowAnyMethod()
            //.AllowAnyHeader()
            //.Build();
            //                            });
            //      });

            services.AddControllers();

            // Appsettings in Azure Application settings Configuration used for DI services
            
            IConfigurationSection authSettingsSection = Configuration.GetSection("AuthenticationSettings"); // GetSection does not work somehow

            // ugly workaround, just using non-hierarchical values, can't get a section even AuthenticationSettings:Secret does not work
            if (authSettingsSection == null)
            {
                string value = "{" +
    "\"Secret\": \""+ Configuration.GetValue<String>("Secret") + "\"," +
    "\"AllowedRoles\": \"" + Configuration.GetValue<String>("AllowedRoles") + "\"," +
    "}";
                authSettingsSection.Value = value;
            }

            // continue..
            services.Configure<AuthenticationSettings>(authSettingsSection);
            var authSettings = authSettingsSection.Get<AuthenticationSettings>();
            checkAuthsettings = authSettings; // just for logging

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

            // use configured connectionstring directly
            services.AddDbContext<AngularHeroesContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("AngularHeroesDbConstr"))
 ); ;


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // this method allows a injected logger and is called, we use logging to check the configuration 
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            logger.LogInformation("Configure called  ");


            logger.LogInformation("AuthenticationSettings Secrets string {0}", Configuration.GetValue<String>("Secret"));

            logger.LogInformation("AuthenticationSettings AllowedRoles string {0}", Configuration.GetValue<String>("AllowedRoles"));

            // this does not work : 
            logger.LogInformation("AuthenticationSettings string {0}", Configuration.GetSection("AuthenticationSettings").Value);

            logger.LogInformation("Connection string {0}", Configuration.GetConnectionString("AngularHeroesDbConstr"));

            if (checkAuthsettings!=null)
            {
                logger.LogInformation("realAuthsettings Secret {0}", checkAuthsettings.Secret);
                logger.LogInformation("realAuthsettings AllowedRoles 0 {0}", checkAuthsettings.AllowedRoles[0]);
                logger.LogInformation("realAuthsettings AllowedRoles  1 {0}", checkAuthsettings.AllowedRoles[1]);

            }

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
