using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MillionThings.Core;
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
        using var client = apiFactory.CreateClient();
 
        var response = await client.GetAsync("/api/v1/todos");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("[]", response.Content.ReadAsStringAsync().Result);
    }
    
    [Fact]
    private async Task ShouldReturnSingleTodoListWhenOneIsCreated()
    {
        using var client = apiFactory.CreateClient();
        
        var todoListName = Guid.NewGuid().ToString();
 
        await client.PostAsync("/api/v1/todos", JsonContent.Create(todoListName));
 
        var listResponse = await client.GetAsync("/api/v1/todos");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        List<TodoData> listResult = await listResponse.Content.ReadFromJsonAsync<List<TodoData>>();
        Assert.Single(listResult);
        Assert.Equal(todoListName, listResult[0].Name);
    }

    [Fact]
    private async Task ShouldCreateTodoList()
    {
        using var client = apiFactory.CreateClient();
        var todoListName = Guid.NewGuid().ToString();
 
        var response = await client.PostAsync("/api/v1/todos", JsonContent.Create(todoListName));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        TodoData result = await response.Content.ReadFromJsonAsync<TodoData>();
        Assert.Equal(todoListName, result.Name);
    }
    
    [Fact]
    private async Task ShouldReturnNotFoundWhenRequestingNonExistingTodoList()
    {
        using var client = apiFactory.CreateClient();
        var nonExistingTodoListId = Guid.NewGuid().ToString();
 
        var response = await client.GetAsync($"/api/v1/todos/{nonExistingTodoListId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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