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
    public void ShouldSaveAnAddedTodo()
    {
        var testString = Guid.NewGuid().ToString();
        Todos sut = new MongodbTodos(mongoContainer.GetConnectionString(), Guid.NewGuid().ToString());

        sut.AddTodo(testString);

        Assert.Single(sut.ListTodos());
        Assert.Equal(testString, sut.ListTodos()[0].Name);
    }

    [Fact]
    public void ShouldSaveAnAddedTask()
    {
        var testString = Guid.NewGuid().ToString();
        var (sut, todoId) = PrepareTodo();
        
        var task = sut.AddTask(todoId, testString);
        
        Assert.Single(sut.ListTasks(todoId));
        Assert.Equal(testString, sut.ListTasks(todoId)[0].Description);
        Assert.Equal(TodoStatus.Open, sut.ListTasks(todoId)[0].Status);
        Assert.Equal(task, sut.ListTasks(todoId)[0]);
    }

    [Fact]
    public void ShouldMarkAFinishedTaskAsDone()
    {
        var testString = Guid.NewGuid().ToString();
        var (sut, todoId) = PrepareTodo();

        sut.AddTask(todoId, testString);
        var taskToMarkAsFinished = sut.ListTasks(todoId).Find(t => t.Description == testString);
        sut.DoneTask(todoId, taskToMarkAsFinished.Id);

        var result = sut.ListTasks(todoId).Find(t => t.Description == testString);
        Assert.Equal(TodoStatus.Done, result.Status);
    }

    [Fact]
    public void ShouldNotListRemovedTasks()
    {
        var testString = Guid.NewGuid().ToString();
        var (sut, todoId) = PrepareTodo();

        sut.AddTask(todoId, testString);
        var taskToBeDeleted = sut.ListTasks(todoId).Find(t => t.Description == testString);
        sut.DeleteTask(todoId, taskToBeDeleted.Id);

        Assert.Empty(sut.ListTasks(todoId));
    }

    private (Todos, string) PrepareTodo()
    {
        Todos sut = new MongodbTodos(mongoContainer.GetConnectionString(), Guid.NewGuid().ToString());
        return (sut, sut.AddTodo(Guid.NewGuid().ToString()).Id);
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