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

    public Todo Add(JsonFileTodo todo)
    {
        todos.Add(todo);
        PersistToFile();
        return todo;
    }

    public Todo Get(string name)
    {
        JsonFileTodo? todo = todos.FirstOrDefault(t => t.Name() == name);
        if (todo is not null) return todo;
        
        todo = new JsonFileTodo(null, new JsonFileTodo.TodoData(Guid.NewGuid().ToString(), name, new()));
        Add(todo);
        return todo;
    }

    public Todo Delete(string name)
    {
        throw new NotImplementedException();
    }

    public void PersistToFile()
    {
        using var sw = new StreamWriter(path);
        sw.Write(JsonSerializer.Serialize(todos.Select(t => t.data)));
    }

    private static List<JsonFileTodo> LoadJsonFile(string path)
    {
        using var r = new StreamReader(path);
        string json = r.ReadToEnd();
        if (string.IsNullOrEmpty(json)) return new();
        List<JsonFileTodo.TodoData> todosData = JsonSerializer.Deserialize<List<JsonFileTodo.TodoData>>(json,
                   new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ??
               new();
        foreach (JsonFileTodo.TodoData data in todosData)
        {
            JsonFileTodo todo = new JsonFileTodo(null, data);
            //todo.ValueChanged += this.HandleValueChanged;
        }
        return todosData.Select(todoData => new JsonFileTodo(null, todoData)).ToList();
    }

    private static void CreateFileIfNotExists(string path)
    {
        if (File.Exists(path))
        {
            return;
        }

        File.WriteAllText(path, "[]");
    }

    public void HandleValueChanged(Todo todo)
    {
        PersistToFile();
    }
}