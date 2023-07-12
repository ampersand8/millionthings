using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using MillionThings.Core;

namespace MillionThings.IntegrationTests.WebAPI;

public class RestApiTodoListTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory apiFactory;
    private readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    private readonly ILogger<RestApiTodoListTests> logger;
    
    public RestApiTodoListTests(ApiWebApplicationFactory factory)
    {
        apiFactory = factory;
        logger = loggerFactory.CreateLogger<RestApiTodoListTests>();
    }
    
    [Fact]
    private async Task ShouldReturnEmptyListWhenListingFirstTime()
    {
        await CleanupTodoLists();
        using var client = apiFactory.CreateClient();
 
        var response = await client.GetAsync("/api/v1/todos");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("[]", response.Content.ReadAsStringAsync().Result);
    }
    
    [Fact]
    private async Task ShouldReturnTodoListInGetWhenOneIsCreated()
    {
        using var client = apiFactory.CreateClient();
        
        var todoListName = Guid.NewGuid().ToString();
        
 
        var createResponse = await client.PostAsync("/api/v1/todos", JsonContent.Create(todoListName));
        var createResult = await createResponse.Content.ReadFromJsonAsync<TodoData>();

        var getResponse = await client.GetAsync($"/api/v1/todos/{createResult.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        TodoData getResult = await getResponse.Content.ReadFromJsonAsync<TodoData>();

        Assert.Equal(todoListName, getResult.Name);
    }
    
    [Fact]
    private async Task ShouldReturnTodoListInListingWhenOneIsCreated()
    {
        using var client = apiFactory.CreateClient();
        
        var todoListName = Guid.NewGuid().ToString();
        
        var firstListResponse = await client.GetAsync("/api/v1/todos");
        var firstListCount = (await firstListResponse.Content.ReadFromJsonAsync<List<TodoData>>()).Count;
 
        var createResponse = await client.PostAsync("/api/v1/todos", JsonContent.Create(todoListName));
        var createResult = await createResponse.Content.ReadFromJsonAsync<TodoData>();
        
        
        var listResponse = await client.GetAsync("/api/v1/todos");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        List<TodoData> listResult = await listResponse.Content.ReadFromJsonAsync<List<TodoData>>();

        Assert.Contains(listResult, data => data.Id.Equals(createResult.Id));
        Assert.Equal(firstListCount + 1, listResult.Count);
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

    [Fact]
    private async Task ShouldRemoveTodoListFromListingWhenDeleting()
    {
        using var client = apiFactory.CreateClient();
        var todoListName = Guid.NewGuid().ToString();
        
        var firstListResponse = await client.GetAsync("/api/v1/todos");
        Assert.Equal(HttpStatusCode.OK, firstListResponse.StatusCode);
        var firstListCount = (await firstListResponse.Content.ReadFromJsonAsync<List<TodoData>>()).Count;
 
        var createResponse = await client.PostAsync("/api/v1/todos", JsonContent.Create(todoListName));
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        TodoData createResult = await createResponse.Content.ReadFromJsonAsync<TodoData>();

        var deleteResponse = await client.DeleteAsync($"/api/v1/todos/{createResult.Id}");
        
        
        var listResponse = await client.GetAsync("/api/v1/todos");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        List<TodoData> listResult = await listResponse.Content.ReadFromJsonAsync<List<TodoData>>();
        Assert.Equal(firstListCount, listResult.Count);
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
    }
    
    [Fact]
    private async Task ShouldReturnNotFoundWhenDeletingNonExistingTodoList()
    {
        using var client = apiFactory.CreateClient();
        var nonExistingTodoListId = Guid.NewGuid().ToString();
 
        var response = await client.DeleteAsync($"/api/v1/todos/{nonExistingTodoListId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task CleanupTodoLists()
    {
        using var client = apiFactory.CreateClient();
        var listResponse = await client.GetAsync("/api/v1/todos");
        var todoLists = await listResponse.Content.ReadFromJsonAsync<List<TodoData>>();
        
        foreach (var todoList in todoLists)
        {
            logger.LogInformation($"Deleting todo list {todoList.Id}");
            await client.DeleteAsync($"/api/v1/todos/{todoList.Id}");

        }
    }
}