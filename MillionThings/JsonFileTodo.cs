using System.Text.Json;

namespace MillionThings.Core;

public class JsonFileTodo : Todo
{
    private readonly List<TodoItem> todos;
    private readonly string path;

    public JsonFileTodo(string path)
    {
        this.path = path;
        CreateFileIfNotExists(path);
        todos = LoadJsonFile(path);
    }


    public List<TodoItem> List()
    {
        return todos;
    }

    public void Add(string description)
    {
        if (todos.Any(todo => todo.Description == description)) return;
        todos.Add(new() { Description = description });
        PersistToFile();
    }

    public void Done(string id)
    {
        TodoItem? todo = todos.Find(todo => todo.Id == id);
        if (todo == null) return;
        Update(todo.Finish());
        PersistToFile();
    }

    private static List<TodoItem> LoadJsonFile(string path)
    {
        using var r = new StreamReader(path);
        string json = r.ReadToEnd();
        if (string.IsNullOrEmpty(json)) return new();
        return JsonSerializer.Deserialize<List<TodoItem>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
    }

    public void Update(TodoItem item)
    {
        var index = todos.FindIndex(todo => todo.Id == item.Id);
        if (index > -1)
        {
            todos[index] = item;
        }
        else
        {
            Add(item.Description);
        }

        PersistToFile();
    }

    public void Delete(string id)
    {
        todos.RemoveAll(todo => todo.Id == id);
        PersistToFile();
    }

    private static void CreateFileIfNotExists(string path)
    {
        if (File.Exists(path))
        {
            return;
        }

        File.WriteAllText(path, "[]");
    }

    private void PersistToFile()
    {
        using var sw = new StreamWriter(path);
        sw.Write(JsonSerializer.Serialize(todos));
    }
}