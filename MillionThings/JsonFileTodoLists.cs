using System.Text.Json;

namespace MillionThings.Core;

public class JsonFileTodoLists : TodoLists
{
    private List<JsonFileTodo> todos;
    private readonly string path;

    public JsonFileTodoLists(string path)
    {
        this.path = path;
        CreateFileIfNotExists(path);
        todos = LoadJsonFile(path);
    }


    public Dictionary<string, Todo> Todos()
    {
        throw new NotImplementedException();
    }

    public Tuple<string, Todo> Add(Todo todo)
    {
        throw new NotImplementedException();
    }

    public string Add(string name)
    {
        throw new NotImplementedException();
    }

    public Tuple<string, Todo> Add(string name, Todo todo)
    {
        throw new NotImplementedException();
    }

    public string Rename(string previousName, string newName)
    {
        throw new NotImplementedException();
    }

    public Todo Get(string name)
    {
        throw new NotImplementedException();
    }

    public Todo Delete(string name)
    {
        throw new NotImplementedException();
    }

    private void PersistToFile()
    {
        using var sw = new StreamWriter(path);
        sw.Write(JsonSerializer.Serialize(todos));
    }

    private static List<JsonFileTodo> LoadJsonFile(string path)
    {
        using var r = new StreamReader(path);
        string json = r.ReadToEnd();
        if (string.IsNullOrEmpty(json)) return new();
        List<JsonFileTodo.TodoData> todosData = JsonSerializer.Deserialize<List<JsonFileTodo.TodoData>>(json,
                   new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ??
               new();
        return todosData.Select(todoData => new JsonFileTodo(path, todoData)).ToList();
    }

    private static void CreateFileIfNotExists(string path)
    {
        if (File.Exists(path))
        {
            return;
        }

        File.WriteAllText(path, "[]");
    }
}