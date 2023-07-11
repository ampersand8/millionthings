using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MillionThings.WebAPI.Controllers;
using MillionThings.WebAPI.Models;
using Testcontainers.MongoDb;

namespace MillionThings.IntegrationTests.WebAPI;

public class ApiWebApplicationFactory : WebApplicationFactory<MillionThingsController>, IAsyncLifetime
{
    private readonly MongoDbContainer mongoContainer = new MongoDbBuilder().Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(config =>
        {
            IConfiguration configuration = new ConfigurationBuilder().Build();
            configuration.Bind(new OptionsWrapper<MillionThingsDatabaseSettings>(new MillionThingsDatabaseSettings
            {
                ConnectionString = mongoContainer.GetConnectionString(),
                DatabaseName = Guid.NewGuid().ToString(),
                MillionThingsCollectionName = "MillionThings"
            }));
            
            config.AddConfiguration(configuration);
        });
        builder.ConfigureServices(services =>
        {
            services.Configure<MillionThingsDatabaseSettings>(options =>
            {
                options.ConnectionString = mongoContainer.GetConnectionString();
                options.DatabaseName = Guid.NewGuid().ToString();
                options.MillionThingsCollectionName = "MillionThings";
            });
        });
    }

    public Task InitializeAsync()
    {
        return mongoContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return mongoContainer.DisposeAsync().AsTask();
    }
}