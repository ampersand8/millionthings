using MillionThings.Core;

namespace MillionThings.Tests.Core;

public class JsonFileTodoTest
{
    [Fact]
    public void ShouldInstantiateWithExistingJsonFile()
    {
        Todo sut = new JsonFileTodo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources/onetodo.json"));
        Assert.NotNull(sut);
    }

    [Fact]
    public void ShouldInstantiateAndCreateNewFile()
    {
        string filename = Guid.NewGuid().ToString();
        Todo sut = new JsonFileTodo(filename);
        Assert.NotNull(sut);
    }

    [Fact]
    public void ShouldListEmptyTodoList()
    {
        Todo sut = CreateRandomTodo();
        Assert.Empty(sut.List());
    }

    [Fact]
    public void ShouldListTodoFromGivenJsonFile()
    {
        Todo sut = new JsonFileTodo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources/onetodo.json"));
        List<TodoTask> expected = new() { new() { Id = "1", Description = "This is the first todo" } };
        Assert.Equal(expected, sut.List());
    }

    [Fact]
    public void ShouldListTodosFromGivenJsonFile()
    {
        Todo sut = new JsonFileTodo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources/twotodos.json"));
        List<TodoTask> expected = new()
        {
            new() { Id = "1", Description = "This is the first todo" },
            new() { Id = "2", Description = "This is the second todo" }
        };
        Assert.Equal(expected, sut.List());
    }

    [Fact]
    public void ShouldAddTodo()
    {
        Todo sut = CreateRandomTodo();
        sut.Add("Write some code that doesn't suck.");

        Assert.Contains(sut.List(), item => item.Description == "Write some code that doesn't suck.");
    }

    [Fact]
    public void ShouldNotListTodosThatAreDone()
    {
        Todo sut = CreateRandomTodo();
        sut.Add("This is my one and only todo");
        Assert.Contains(sut.List(), item => item.Description == "This is my one and only todo");
        sut.Done(sut.List()[0].Id);
        Assert.Contains(sut.List(),
            task => task is { Description: "This is my one and only todo", Status: TodoStatus.Done });
    }

    [Theory]
    [InlineData("SOME_WRONG_ID")]
    [InlineData("")]
    public void ShouldNotFailWhenSettingANonExistentTodoToDone(string inputId)
    {
        Todo sut = CreateRandomTodo();
        sut.Add("This is my one and only todo");
        Assert.Contains(sut.List(), item => item.Description == "This is my one and only todo");
        sut.Done(inputId);
        Assert.Contains(sut.List(), item => item.Description == "This is my one and only todo");
    }

    [Theory]
    [InlineData("SOME_WRONG_ID")]
    [InlineData("")]
    [InlineData(null)]
    public void ShouldNotFailWhenSettingANonExistentTodoToDoneWhenNoTodosExist(string inputId)
    {
        Todo sut = CreateRandomTodo();
        var result = sut.Done(inputId);
        Assert.Empty(sut.List());
        Assert.Null(result);
    }

    [Fact]
    public void ShouldPersistTodosInJsonFile()
    {
        string filename = Guid.NewGuid().ToString();
        Todo? sut = new JsonFileTodo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename));
        Assert.Empty(sut.List());
        sut.Add("Testing One");
        sut.Add("Testing Two");
        Assert.Contains(sut.List(), item => item.Description == "Testing One");
        Assert.Contains(sut.List(), item => item.Description == "Testing Two");
        sut.Done(sut.List().First().Id);
        Assert.Contains(sut.List(), item => item.Description == "Testing One" && item.Status == TodoStatus.Done);

        Todo reloadedSut = new JsonFileTodo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename));
        Assert.Contains(reloadedSut.List(),
            item => item.Description == "Testing One" && item.Status == TodoStatus.Done);
        Assert.Contains(reloadedSut.List(), item => item.Description == "Testing Two");
    }

    [Fact]
    public void ShouldAddTaskWithSameDescription()
    {
        Todo sut = CreateRandomTodo();

        sut.Add("Testing One");
        Assert.Single(sut.List());

        var result = sut.Add("Testing One");
        Assert.Equal("Testing One", result.Description);
        Assert.Equal(TodoStatus.Open, result.Status);
        Assert.Equal(2, sut.List().FindAll(todo => todo.Description == "Testing One").Count);
    }

    [Fact]
    public void ShouldUpdateTodoDescription()
    {
        Todo sut = CreateRandomTodo();

        sut.Add("Testing One");
        TodoTask savedTodo = sut.List().First();

        sut.Update(new() { Id = savedTodo.Id, Description = "Updated todo description" });
        Assert.Single(sut.List());
        Assert.DoesNotContain(sut.List(), item => item.Description == "Testing One");
        Assert.Contains(sut.List(), item => item.Description == "Updated todo description");
    }

    [Fact]
    public void ShouldAddTodoWhenUpdateTodoHasADifferentId()
    {
        Todo sut = CreateRandomTodo();

        sut.Add("Testing One");

        sut.Update(new() { Id = "ID_DOES_NOT_EXIST", Description = "Updated todo description" });
        Assert.Equal(2, sut.List().Count);
        Assert.Contains(sut.List(), item => item.Description == "Testing One");
        Assert.Contains(sut.List(), item => item.Description == "Updated todo description");
    }

    [Fact]
    public void ShouldNotAddTodoWhenUpdateTodoHasADifferentIdButSameDescriptionAlreadyExists()
    {
        Todo sut = CreateRandomTodo();

        sut.Add("Testing One");

        sut.Update(new() { Id = "ID_DOES_NOT_EXIST", Description = "Testing One" });
        Assert.Equal(2, sut.List().Count);
        Assert.Contains(sut.List(), item => item.Description == "Testing One");
    }

    [Fact]
    public void ShouldRemoveTodoWhenDeleteIsCalled()
    {
        Todo sut = CreateRandomTodo();

        sut.Add("Testing One");
        TodoTask savedTodo = sut.List().First();

        sut.Delete(savedTodo.Id);
        Assert.Empty(sut.List());
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ShouldIgnoreDeletingATaskWithNullOrEmptyId(string inputId)
    {
        Todo sut = CreateRandomTodo();
        
        sut.Add("Testing One");
        
        sut.Delete(inputId);
        Assert.Single(sut.List());
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ShouldFailAddingATaskWithNullOrEmptyDescription(string inputDescription)
    {
        Todo sut = CreateRandomTodo();
        
        var exception = Assert.Throws<ArgumentException>(() => sut.Add(inputDescription));
        
        Assert.Equal("Description can not be empty or null", exception.Message);
    }

    private static Todo CreateRandomTodo()
    {
        string filename = Guid.NewGuid().ToString();
        return new JsonFileTodo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename));
    }
}