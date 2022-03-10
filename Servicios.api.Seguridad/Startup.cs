using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Servicios.api.Seguridad.Core.Application;
using Servicios.api.Seguridad.Core.DTO;
using Servicios.api.Seguridad.Core.Entities;
using Servicios.api.Seguridad.Core.JwtLogic;
using Servicios.api.Seguridad.Core.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios.api.Seguridad
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SeguridadContexto>(opt =>
           {
               opt.UseSqlServer(Configuration.GetConnectionString("ConnectionDB"));
           });

            var builder = services.AddIdentityCore<Usuario>();
            var identityBuilder = new IdentityBuilder(builder.UserType, builder.Services);
            identityBuilder.AddEntityFrameworkStores<SeguridadContexto>();
            identityBuilder.AddSignInManager<SignInManager<Usuario>>();
            services.TryAddSingleton<ISystemClock, SystemClock>();

            services.AddMediatR(typeof(Registrar.UsuarioRegistrarCommand).Assembly);
            services.AddAutoMapper(typeof(Registrar.UsuarioRegistrarHandler));

            services.AddScoped<IJwtGenerator, JwtGenerator>();
            services.AddScoped<IUsuarioSesion, UsuarioSesion>();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(""));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateAudience = false,
                    ValidateIssuer = false
                };
            });

            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsRule", rule =>
                {
                    rule.AllowAnyHeader().AllowAnyMethod().WithOrigins("*");
                });
            });

            services.AddControllers().AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<Registrar>());

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Servicios.api.Seguridad", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Servicios.api.Seguridad v1"));
            }

            app.UseRouting();
            app.UseCors("CorsRule");
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
