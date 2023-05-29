using MillionThings.Core;

namespace MillionThings.Tests.Core;

public class JsonFileTodoListsTest
{
    [Fact]
    public void ShouldInstantiateAndCreateANewFile()
    {
        string filename = Guid.NewGuid().ToString();
        TodoLists sut = new JsonFileTodoLists(filename);
        Assert.NotNull(sut);
        Assert.True(File.Exists(filename));
    }

    [Fact]
    public void ShouldInstantiateWithExistingFile()
    {
        TodoLists sut = new JsonFileTodoLists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources/onetodolist.json"));
        Assert.NotNull(sut);
    }
    
    private static TodoLists CreateRandomTodoLists()
    {
        string filename = Guid.NewGuid().ToString();
        return new JsonFileTodoLists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename));
    }
}