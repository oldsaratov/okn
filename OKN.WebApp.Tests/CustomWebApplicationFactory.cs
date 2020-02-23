using Microsoft.AspNetCore.Authentication;
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
        public MongoDbRunner Runner;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            Runner = MongoDbRunner.Start();

            builder.ConfigureServices(services =>
            {
                var client = new MongoClient(Runner.ConnectionString);

                var database = client.GetDatabase("okn");

                services.AddSingleton(database);
                CoreContainer.Init(services);
            });
        }
    }

    public class CustomWebApplicationFactoryWithAuth<TStartup> : WebApplicationFactory<Startup>
    {
        public MongoDbRunner Runner;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            Runner = MongoDbRunner.Start();

            builder.ConfigureServices(services =>
            {
                var client = new MongoClient(Runner.ConnectionString);

                var database = client.GetDatabase("okn");

                services.AddSingleton(database);
                CoreContainer.Init(services);

                services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, FakeAuthHandler>("Test", options => { });
            });
        }
    }
}
