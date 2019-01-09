using AspNet.Security.OAuth.Oldsaratov;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OKN.Core;
using Swashbuckle.AspNetCore.Swagger;
using System;
using EventFlow;
using EventFlow.DependencyInjection.Extensions;
using EventFlow.Extensions;
using MongoDB.Driver;
using OKN.Core.Handlers.Commands;
using OKN.Core.Handlers.Queries;
using OKN.Core.Models.Commands;

namespace OKN.WebApp
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddAuthentication(OldsaratovAuthenticationDefaults.AuthenticationScheme).AddOldsaratov(options =>
            {
                options.TokenExpirationTimeout = TimeSpan.FromSeconds(3600);
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAny",
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
            });

            services.AddMvc();
            
            ConfigureMongo(services);

            services.AddTransient<DbContext>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "OKN API", Version = "v1" });
            });

            CoreContainer.Init(services);
        }

        private void ConfigureMongo(IServiceCollection services)
        {
            var url = new MongoUrl(Configuration.GetConnectionString("MongoDB"));

            var client = new MongoClient(url);
            var database = client.GetDatabase(url.DatabaseName);

            services.AddSingleton(database);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowAny");
            app.UseAuthentication();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Archief API V1");
            });
        }
    }
}