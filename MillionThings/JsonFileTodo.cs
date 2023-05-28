using System.Text.Json;

namespace MillionThings.Core;

public class JsonFileTodo : Todo
{
    private readonly List<TodoTask> todos;
    private readonly string path;

    public JsonFileTodo(string path)
    {
        this.path = path;
        CreateFileIfNotExists(path);
        todos = LoadJsonFile(path);
    }


    public List<TodoTask> List()
    {
        return todos;
    }

    public TodoTask Add(string description)
    {
        if (string.IsNullOrEmpty(description)) throw new ArgumentException("Description can not be empty or null");
        var newTask = new TodoTask() { Description = description };
        todos.Add(newTask);
        PersistToFile();
        return newTask;
    }

    public TodoTask? Done(string id)
    {
        TodoTask? todo = todos.Find(todo => todo.Id == id);
        if (todo == null) return null;
        var updatedTask = Update(todo.Finish());
        PersistToFile();
        return updatedTask;
    }
    
    public TodoTask Update(TodoTask task)
    {
        var index = todos.FindIndex(todo => todo.Id == task.Id);
        TodoTask updatedTodo;
        if (index > -1)
        {
            todos[index] = task;
            updatedTodo = task;
        }
        else
        {
            updatedTodo = Add(task.Description);
        }

        PersistToFile();
        return updatedTodo;
    }

    public TodoTask? Delete(string id)
    {
        var toRemove = todos.Find(todo => todo.Id == id);
        todos.RemoveAll(todo => todo.Id == id);
        PersistToFile();
        return toRemove;
    }

    private static List<TodoTask> LoadJsonFile(string path)
    {
        using var r = new StreamReader(path);
        string json = r.ReadToEnd();
        if (string.IsNullOrEmpty(json)) return new();
        return JsonSerializer.Deserialize<List<TodoTask>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
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