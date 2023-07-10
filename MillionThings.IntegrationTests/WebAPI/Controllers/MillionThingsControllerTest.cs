using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MillionThings.WebAPI.Controllers;
using MillionThings.WebAPI.Models;
using Testcontainers.MongoDb;

namespace MillionThings.IntegrationTests.WebAPI.Controllers;

public class MillionThingsControllerTest : IClassFixture<ApiWebApplicationFactory>
{
    private readonly MongoDbContainer mongoContainer = new MongoDbBuilder().Build();
    private readonly ApiWebApplicationFactory apiFactory;
    private readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    
    public MillionThingsControllerTest(ApiWebApplicationFactory factory)
    {
        apiFactory = factory;
    }
    
    [Fact]
    private async Task ShouldReturnEmptyListWhenListingFirstTime()
    {
        //await using var application = new WebApplicationFactory<MillionThingsController>();
        using var client = apiFactory.CreateClient();
 
        var response = await client.GetAsync("/api/v1/todos");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("[]", response.Content.ReadAsStringAsync().Result);
        // var sut = new MillionThingsController(SetupMongoDbSettings());
        
        //var result = sut.GetTodoLists();
        
        //Assert.Empty(result);
    }
/*
    [Fact]
    private void ShouldCreateTodoList()
    {
        var sut = new MillionThingsController(loggerFactory, SetupMongoDbSettings());
        
        var createResult = sut.CreateTodoList("Test Create List");
        var listResult = sut.GetTodoLists();
        
        Assert.Equal("Test Create List", createResult.Name);
        Assert.Single(listResult);
    }
    
    
    [Fact]
    private void ShouldGetTodoListWhenExists()
    {
        var todoListName = Guid.NewGuid().ToString();
        var sut = new MillionThingsController(loggerFactory, SetupMongoDbSettings());
        
        var createResult = sut.CreateTodoList(todoListName);
        var getTodoListResult = sut.GetTodoList(createResult.Id);
        
        Assert.Equal(todoListName, createResult.Name);
        Assert.Equal(todoListName, getTodoListResult.Name);
    }
    
    [Fact]
    private void ShouldReturnEmptyObjectWhen()
    {
        var todoListName = Guid.NewGuid().ToString();
        var sut = new MillionThingsController(loggerFactory, SetupMongoDbSettings());
        
        var createResult = sut.CreateTodoList(todoListName);
        var getTodoListResult = sut.GetTodoList(createResult.Id);
        
        Assert.Equal(todoListName, createResult.Name);
        Assert.Equal(todoListName, getTodoListResult.Name);
    }

    [Fact]
    private void ShouldRemoveTodoList()
    {
        var testTodoListName = Guid.NewGuid().ToString();
        var sut = new MillionThingsController(loggerFactory, SetupMongoDbSettings());
        
        var createResult = sut.CreateTodoList(testTodoListName);
        var deleteResult = sut.DeleteTodoList(createResult.Id);
        var listResult = sut.GetTodoLists();
        
        Assert.Equal(testTodoListName, createResult.Name);
        Assert.Equal(testTodoListName, deleteResult.Name);
        Assert.Empty(listResult);
    }

    [Fact]
    private void ShouldCreateTask()
    {
        var listName = Guid.NewGuid().ToString();
        var taskDescription = Guid.NewGuid().ToString();
        var sut = new MillionThingsController(loggerFactory, SetupMongoDbSettings());
        
        var createTodoListResult = sut.CreateTodoList(listName);
        var createTaskResult = sut.CreateTask(createTodoListResult.Id, taskDescription);
        var listTasksResult = sut.ListTasks(createTodoListResult.Id);
        
        Assert.Equal(taskDescription, createTaskResult.Description);
        Assert.Single(listTasksResult);
    }*/
    
    public Task InitializeAsync()
    {
        return mongoContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return mongoContainer.DisposeAsync().AsTask();
    }
    
    private IOptions<MillionThingsDatabaseSettings> SetupMongoDbSettings()
    {
        return new OptionsWrapper<MillionThingsDatabaseSettings>(new MillionThingsDatabaseSettings
        {
            ConnectionString = mongoContainer.GetConnectionString(),
            DatabaseName = Guid.NewGuid().ToString()
        });
    }
}