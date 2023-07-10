using System.Text.Json;

namespace MillionThings.Core;

public class JsonFileTodos : Todos
{
    private readonly Dictionary<string, TodoData> data;
    private readonly string path;

    public JsonFileTodos(string jsonFilePath)
    {
        path = jsonFilePath;
        CreateFileIfNotExists(path);
        data = LoadJsonFile(path);
    }
    
    public string NameTodo(string todoId)
    {
        return data[todoId].Name;
    }

    public TodoData RenameTodo(string todoId, string newName)
    {
        data[todoId] = new TodoData(todoId, newName,data[todoId].Tasks);
        PersistToFile();
        return data[todoId];
    }

    public TodoData? DeleteTodo(string todoId)
    {
        var removed = data[todoId];
        data.Remove(todoId);
        PersistToFile();
        return removed;
    }

    public TodoData AddTodo(TodoData todo)
    {
        data.Add(todo.Id, todo);
        PersistToFile();
        return todo;
    }
    
    public TodoData AddTodo(string name)
    {
        return AddTodo(new TodoData(Guid.NewGuid().ToString(), name, new()));
    }

    public TodoData GetTodo(string todoId)
    {
        return data[todoId];
    }

    public List<TodoData> ListTodos()
    {
        return data.Values.ToList();
    }

    public List<TodoTask> ListTasks(string todoId)
    {
        return data[todoId].Tasks;
    }

    public TodoTask? GetTask(string todoId, string taskId)
    {
        return data[todoId].Tasks.Find(t => t.Id == taskId);
    }

    public TodoTask AddTask(string todoId, string description)
    {
        var added = new TodoTask(description);
        data[todoId].Tasks.Add(added);
        PersistToFile();
        return added;
    }

    public TodoTask? DeleteTask(string todoId, string taskId)
    {
        var deleted = data[todoId].Tasks.Find(t => t.Id == taskId);
        if (deleted is null) return null;
        data[todoId].Tasks.Remove(deleted);
        PersistToFile();
        return deleted;
    }

    public TodoTask? DoneTask(string todoId, string taskId)
    {
        var done = data[todoId].Tasks.Find(t => t.Id == taskId);
        if (done is null) return null;
        UpdateTask(todoId, done.Finish());
        return done;
    }

    public TodoTask UpdateTask(string todoId, TodoTask task)
    {
        var toUpdatePosition = data[todoId].Tasks.FindIndex(t => t.Id == task.Id);
        data[todoId].Tasks[toUpdatePosition] = task;
        PersistToFile();
        return task;
    }
    
    private static void CreateFileIfNotExists(string path)
    {
        if (File.Exists(path))
        {
            return;
        }

        File.WriteAllText(path, "");
    }
    
    private static Dictionary<string, TodoData> LoadJsonFile(string path)
    {
        using var r = new StreamReader(path);
        string json = r.ReadToEnd();
        if (string.IsNullOrEmpty(json)) return new();

        return JsonSerializer.Deserialize<List<TodoData>>(json,
                   new JsonSerializerOptions { PropertyNameCaseInsensitive = true })?
                   .ToDictionary(t => t.Id, t => t) ??
               new();
    }

    private void PersistToFile()
    {
        //if (path is null) return;
        using var sw = new StreamWriter(path);
        sw.Write(JsonSerializer.Serialize(data.Values));
    }
}