using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Mongo2Go;
using MongoDB.Driver;
using OKN.Core;

namespace OKN.WebApp.Tests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<Startup>
    {
        public MongoDbRunner runner;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            runner = MongoDbRunner.Start();

            builder.ConfigureServices(services =>
            {
                var client = new MongoClient(runner.ConnectionString);

                var database = client.GetDatabase("okn");

                services.AddSingleton(database);
                CoreContainer.Init(services);
            });
        }
    }
}
