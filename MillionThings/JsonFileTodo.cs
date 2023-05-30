using System.Text.Json;
using System.Text.Json.Serialization;

namespace MillionThings.Core;

public class JsonFileTodo : Todo
{
    // Define a delegate type for the event handler
    public delegate void ValueChangedEventHandler(Todo newValue);

    // Define an event based on the delegate
    public event ValueChangedEventHandler? ValueChanged;
    public TodoData data { get; set; }
    private readonly string? path;

    private List<TodoTask> tasks => data.Tasks;

    public JsonFileTodo(string? path)
    {
        if (path is not null)
        {
            CreateFileIfNotExists(path);
            data = LoadJsonFile(path);
            this.path = path;
        }
        else
        {
            data = new TodoData(Guid.NewGuid().ToString(), "default", new());
        }
    }

    public JsonFileTodo(string? path, TodoData data)
    {
        this.path = path;
        this.data = data;
    }

    public string Name()
    {
        return data.Name;
    }

    public Todo Rename(string newName)
    {
        data = new TodoData(data.Id, newName, tasks);
        return this;
    }

    public List<TodoTask> List()
    {
        return tasks;
    }

    public TodoTask Add(string description)
    {
        if (string.IsNullOrEmpty(description)) throw new ArgumentException("Description can not be empty or null");
        var newTask = new TodoTask() { Description = description };
        tasks.Add(newTask);
        PersistToFile();
        return newTask;
    }

    public TodoTask? Done(string id)
    {
        TodoTask? todo = tasks.Find(todo => todo.Id == id);
        if (todo == null) return null;
        var updatedTask = Update(todo.Finish());
        PersistToFile();
        return updatedTask;
    }

    public TodoTask Update(TodoTask task)
    {
        var index = tasks.FindIndex(todo => todo.Id == task.Id);
        TodoTask updatedTodo;
        if (index > -1)
        {
            tasks[index] = task;
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
        var toRemove = tasks.Find(todo => todo.Id == id);
        tasks.RemoveAll(todo => todo.Id == id);
        PersistToFile();
        return toRemove;
    }

    private static TodoData LoadJsonFile(string path)
    {
        using var r = new StreamReader(path);
        string json = r.ReadToEnd();
        if (string.IsNullOrEmpty(json)) return new(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), new());

        return JsonSerializer.Deserialize<TodoData>(json,
                   new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ??
               new(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), new());
    }


    private static void CreateFileIfNotExists(string path)
    {
        if (File.Exists(path))
        {
            return;
        }

        File.WriteAllText(path, "");
    }

    private void PersistToFile()
    {
        OnValueChanged(this);
        if (path is null) return;
        using var sw = new StreamWriter(path);
        sw.Write(JsonSerializer.Serialize(data));
    }
    
    protected virtual void OnValueChanged(Todo newValue)
    {
        // Check if there are any subscribers, then invoke the event
        ValueChanged?.Invoke(newValue);
    }

    public record TodoData(string Id, string Name, List<TodoTask> Tasks);
}