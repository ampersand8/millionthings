using MillionThings.Core;
using MillionThings.Database;
using Testcontainers.MongoDb;

namespace MillionThings.IntegrationTests.Database;

public class MongodbTodoTest : IAsyncLifetime
{
    private readonly MongoDbContainer mongoContainer = new MongoDbBuilder().Build();

    [Fact]
    public void ShouldSaveAnAddedTask()
    {
        var testString = Guid.NewGuid().ToString();
        Todo sut = new MongodbTodo(mongoContainer.GetConnectionString(), "todos");
        
        sut.Add(testString);
        
        Assert.Single(sut.List());
        Assert.Equal(testString, sut.List()[0].Description);
    }

    [Fact]
    public void ShouldMarkAFinishedTaskAsDone()
    {
        var testString = Guid.NewGuid().ToString();
        Todo sut = new MongodbTodo(mongoContainer.GetConnectionString(), "todos");

        sut.Add(testString);
        var taskToMarkAsFinished = sut.List().Find(t => t.Description == testString);
        sut.Done(taskToMarkAsFinished.Id);

        var result = sut.List().Find(t => t.Description == testString);
        Assert.Equal(TodoStatus.Done, result.Status);
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