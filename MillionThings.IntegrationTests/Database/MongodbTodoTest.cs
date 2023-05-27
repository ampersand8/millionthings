using MillionThings.Core;
using MillionThings.Database;
using Testcontainers.MongoDb;

namespace MillionThings.IntegrationTests.Database;

public class MongodbTodoTest : IAsyncLifetime
{
    private readonly MongoDbContainer mongoContainer = new MongoDbBuilder().Build();

    [Fact]
    public void ShouldBeEmptyAtStart()
    {
        Todo sut = new MongodbTodo(mongoContainer.GetConnectionString(), Guid.NewGuid().ToString());

        Assert.Empty(sut.List());
    }

    [Fact]
    public void ShouldSaveAnAddedTask()
    {
        var testString = Guid.NewGuid().ToString();
        Todo sut = new MongodbTodo(mongoContainer.GetConnectionString(), Guid.NewGuid().ToString());
        
        sut.Add(testString);
        
        Assert.Single(sut.List());
        Assert.Equal(testString, sut.List()[0].Description);
        Assert.Equal(TodoStatus.Open, sut.List()[0].Status);
    }

    [Fact]
    public void ShouldMarkAFinishedTaskAsDone()
    {
        var testString = Guid.NewGuid().ToString();
        Todo sut = new MongodbTodo(mongoContainer.GetConnectionString(), Guid.NewGuid().ToString());

        sut.Add(testString);
        var taskToMarkAsFinished = sut.List().Find(t => t.Description == testString);
        sut.Done(taskToMarkAsFinished.Id);

        var result = sut.List().Find(t => t.Description == testString);
        Assert.Equal(TodoStatus.Done, result.Status);
    }

    [Fact]
    public void ShouldNotListRemovedTasks()
    {
        var testString = Guid.NewGuid().ToString();
        Todo sut = new MongodbTodo(mongoContainer.GetConnectionString(), Guid.NewGuid().ToString());

        sut.Add(testString);
        var taskToBeDeleted = sut.List().Find(t => t.Description == testString);
        sut.Delete(taskToBeDeleted.Id);

        Assert.Empty(sut.List());
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