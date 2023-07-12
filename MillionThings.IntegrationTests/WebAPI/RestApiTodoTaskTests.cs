using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using MillionThings.Core;

namespace MillionThings.IntegrationTests.WebAPI;

public class RestApiTodoTaskTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory apiFactory;
    private readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    private readonly ILogger<RestApiTodoTaskTests> logger;
    private TodoData? todoList;

    public RestApiTodoTaskTests(ApiWebApplicationFactory factory)
    {
        apiFactory = factory;
        logger = loggerFactory.CreateLogger<RestApiTodoTaskTests>();
    }

    [Fact]
    private async Task ShouldReturnEmptyTaskListWhenListingAfterCreationOfTodoList()
    {
        using var client = apiFactory.CreateClient();
        await CleanupTaskList();

        var getResponse = await client.GetAsync($"/api/v1/todos/{todoList.Id}/tasks");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        List<TodoTask> getResult = await getResponse.Content.ReadFromJsonAsync<List<TodoTask>>();

        Assert.Empty(getResult);
    }
    
    [Fact]
    private async Task ShouldCreateTask()
    {
        var taskDescription = Guid.NewGuid().ToString();
        using var client = apiFactory.CreateClient();
        await CleanupTaskList();

        var createResponse = await client.PostAsync($"/api/v1/todos/{todoList.Id}/tasks", JsonContent.Create(taskDescription));
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        TodoTask createResult = await createResponse.Content.ReadFromJsonAsync<TodoTask>();

        Assert.Equal(taskDescription, createResult.Description);
    }

    [Fact]
    private async Task ShouldDeleteTask()
    {
        var taskDescription = Guid.NewGuid().ToString();
        using var client = apiFactory.CreateClient();
        await CleanupTaskList();

        var createResponse = await client.PostAsync($"/api/v1/todos/{todoList.Id}/tasks", JsonContent.Create(taskDescription));
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        TodoTask createResult = await createResponse.Content.ReadFromJsonAsync<TodoTask>();
        
        var deleteResponse = await client.DeleteAsync($"/api/v1/todos/{todoList.Id}/tasks/{createResult.Id}");
        TodoTask deleteResult = await deleteResponse.Content.ReadFromJsonAsync<TodoTask>();
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        Assert.Equal(taskDescription, deleteResult.Description);
    }
    
    [Fact]
    private async Task ShouldReturnNotFoundWhenDeletingNonExistingTask()
    {
        var nonExistingTask = Guid.NewGuid().ToString();
        using var client = apiFactory.CreateClient();
        await CleanupTaskList();

        var deleteResponse = await client.DeleteAsync($"/api/v1/todos/{todoList.Id}/tasks/{nonExistingTask}");
        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
    }
    
    [Fact]
    private async Task ShouldMarkTaskAsDone()
    {
        var taskDescription = Guid.NewGuid().ToString();
        using var client = apiFactory.CreateClient();
        await GetTodoList();
        var createResponse = await client.PostAsync($"/api/v1/todos/{todoList.Id}/tasks", JsonContent.Create(taskDescription));
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        TodoTask createResult = await createResponse.Content.ReadFromJsonAsync<TodoTask>();
        
        var markAsDoneResponse = await client.PostAsync($"/api/v1/todos/{todoList.Id}/tasks/{createResult.Id}/done", null);
        Assert.Equal(HttpStatusCode.OK, markAsDoneResponse.StatusCode);
        
        var getResponse = await client.GetAsync($"/api/v1/todos/{todoList.Id}/tasks/{createResult.Id}");
        var getResult = await getResponse.Content.ReadFromJsonAsync<TodoTask>();
        
        Assert.Equal(TodoStatus.Done, getResult.Status);
    }

    private async Task<TodoData> GetTodoList()
    {
        if (todoList is not null)
        {
            return todoList;
        }

        using var client = apiFactory.CreateClient();
        var createResponse = await client.PostAsync("/api/v1/todos", JsonContent.Create(Guid.NewGuid().ToString()));
        todoList = await createResponse.Content.ReadFromJsonAsync<TodoData>();
        return todoList;
    }

    private async Task CleanupTaskList()
    {
        todoList = null;
        todoList = await GetTodoList();
    }
}