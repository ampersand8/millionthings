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
    private async Task ShouldReturnEmptyListWhenListingFirstTime()
    {
        using var client = apiFactory.CreateClient();
        
        var todoListName = Guid.NewGuid().ToString();
        
 
        var createResponse = await client.PostAsync("/api/v1/todos", JsonContent.Create(todoListName));
        var createResult = await createResponse.Content.ReadFromJsonAsync<TodoData>();

        var getResponse = await client.GetAsync($"/api/v1/todos/{createResult.Id}/tasks");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        List<TodoTask> getResult = await getResponse.Content.ReadFromJsonAsync<List<TodoTask>>();

        Assert.Empty(getResult);
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