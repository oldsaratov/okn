using AspNet.Security.OAuth.Oldsaratov;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OKN.Core;
using System;
using System.IO;
using MongoDB.Driver;
using System.Reflection;
using Microsoft.OpenApi.Models;

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
                .AddUserSecrets<Startup>()
                .AddEnvironmentVariables();

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

            var v = Assembly.GetEntryAssembly().GetName().Version;
            var b = Configuration.GetValue<string>("BuildNumber");

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = $"OKN API", 
                    Version = v.ToString(), 
                    Description = $"Build Number: {b}"
                });
                
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            
                c.IncludeXmlComments(xmlPath);
            });

            CoreContainer.Init(services);
        }

        private void ConfigureMongo(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("MongoDB");
            var url = new MongoUrl(connectionString);

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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "OKN API V1");
            });
        }
    }
}