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
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace OKN.WebApp
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
            services.AddApplicationInsightsTelemetry();
            services.AddMemoryCache();
            services.AddAuthentication(OldsaratovAuthenticationDefaults.AuthenticationScheme).AddOldsaratov(options =>
            {
                options.TokenExpirationTimeout = TimeSpan.FromSeconds(3600);
            });
            
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            services.AddMvc().AddNewtonsoftJson();
            
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

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "OKN API V1");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}