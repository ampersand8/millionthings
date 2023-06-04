using System.Reflection.Metadata;
using MillionThings.Core;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MillionThings.Database;

public class MongodbTodos : Todos
{
    private readonly IMongoCollection<TodoData> todos;


    public MongodbTodos(string connectionString, string database)
    {
        todos = new MongoClient(connectionString).GetDatabase(database).GetCollection<TodoData>("todos");
    }


    public string NameTodo(string todoId)
    {
        return GetTodo(todoId).Name;
    }

    public TodoData RenameTodo(string todoId, string newName)
    {
        return todos.FindOneAndUpdate(
            Builders<TodoData>.Filter.Eq(todo => todo.Id, todoId),
            Builders<TodoData>.Update.Set(todo => todo.Name, newName));
    }

    public TodoData? DeleteTodo(string todoId)
    {
        return todos.FindOneAndDelete(Builders<TodoData>.Filter.Eq(todo => todo.Id, todoId));
    }

    public TodoData AddTodo(TodoData todo)
    {
        todos.InsertOne(todo);
        return todo;
    }

    public TodoData AddTodo(string name)
    {
        return AddTodo(new TodoData(Guid.NewGuid().ToString(), name, new()));
    }

    public TodoData GetTodo(string todoId)
    {
        return todos.Find(t => t.Id == todoId).First();
    }

    public List<TodoData> ListTodos()
    {
        return todos.Find(_ => true).ToList();
    }

    public List<TodoTask> ListTasks(string todoId)
    {
        return GetTodo(todoId).Tasks;
    }

    public TodoTask AddTask(string todoId, string description)
    {
        var newTask = new TodoTask(Guid.NewGuid().ToString(), description);
        todos.UpdateOne(
            Builders<TodoData>.Filter.Eq(todo => todo.Id, todoId),
            Builders<TodoData>.Update.Push(todo => todo.Tasks, newTask));
        return newTask;
    }

    public TodoTask? DeleteTask(string todoId, string taskId)
    {
        var task = GetTodo(todoId).Tasks.Find(task => task.Id == taskId);
        if (task is null) return null;
        todos.UpdateOne(
            Builders<TodoData>.Filter.Eq(todo => todo.Id, todoId),
            Builders<TodoData>.Update.PullFilter(todo => todo.Tasks, Builders<TodoTask>.Filter.Eq(t => t.Id, taskId)));
        return task;
    }

    public TodoTask? DoneTask(string todoId, string taskId)
    {
        var task = GetTodo(todoId).Tasks.Find(task => task.Id == taskId);
        if (task is null) return null;
        return UpdateTask(todoId, task.Finish());
    }

    public TodoTask UpdateTask(string todoId, TodoTask task)
    {
        var filter = Builders<TodoData>.Filter.Eq(t => t.Id, todoId) &
                     Builders<TodoData>.Filter.ElemMatch(t => t.Tasks, t => t.Id == task.Id);
        var update = Builders<TodoData>.Update.Set(t => t.Tasks.FirstMatchingElement(), task);

        todos.UpdateOne(filter, update);
        return task;
    }
}