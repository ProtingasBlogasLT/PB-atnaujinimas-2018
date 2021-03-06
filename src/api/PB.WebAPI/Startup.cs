﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Neo4j.Driver.V1;
using PB.WebAPI.Models;
using PB.WebAPI.Repositories;
using PB.WebAPI.Services;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PB.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSwaggerGen(SetupSwagger);

            var appSettingsSection = Configuration.GetSection(nameof(AppSettings));
            services.Configure<AppSettings>(appSettingsSection);

            services.AddScoped<IUsersRepo, UsersRepo>();
            services.AddScoped<IArticlesRepo, ArticlesRepo>();

            services.AddScoped<SecurityTokenHandler, JwtSecurityTokenHandler>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IUserService, UserService>();

            services.AddSingleton(CreateDriver);

            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.TokenSecret);

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

            // configure DI for application services
            services.AddScoped<IUserService, UserService>();
        }

        private void SetupSwagger(SwaggerGenOptions options)
        {
            var swaggerInfo = new Info
            {
                Version = "v1",
                Title = "PB WebAPI",
                Description = "Web API for Protingas Blogas",
                TermsOfService = "None",
            };
            options.SwaggerDoc("v1", swaggerInfo);
        }

        private IDriver CreateDriver(IServiceProvider serviceProvider)
        {
            var settings = serviceProvider.GetService<IOptions<AppSettings>>().Value;
            var token = AuthTokens.Basic(settings.DbUsername, settings.DbPassword);
            var config = new Config();
            config.EncryptionLevel = EncryptionLevel.None;
            var driver = GraphDatabase.Driver(settings.DbURL, token, config);
            return driver;
        }

        public class SecurityHeadersPolicy
        {
            public IDictionary<string, string> SetHeaders { get; }
                = new Dictionary<string, string>();

            public ISet<string> RemoveHeaders { get; }
                = new HashSet<string>();
        }


        public class SecurityHeadersMiddleware
        {
            private readonly RequestDelegate _next;
            private readonly SecurityHeadersPolicy _policy;

            public SecurityHeadersMiddleware(RequestDelegate next, SecurityHeadersPolicy policy)
            {
                _next = next;
                _policy = policy;
            }

            public async Task Invoke(HttpContext context)
            {
                IHeaderDictionary headers = context.Response.Headers;

                foreach (var headerValuePair in _policy.SetHeaders)
                {
                    headers[headerValuePair.Key] = headerValuePair.Value;
                }

                foreach (var header in _policy.RemoveHeaders)
                {
                    headers.Remove(header);
                }

                await _next(context);
            }
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            app.UseMvc();
            //app.UseOptions();
            /*app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
                context.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "*" });
                context.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "*" });
                await next();
            });*/

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

        }
    }
}
