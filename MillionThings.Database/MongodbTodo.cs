using MillionThings.Core;
using MongoDB.Driver;

namespace MillionThings.Database;

public class MongodbTodo : Todo
{
    private readonly IMongoCollection<TodoTask> todos;

    public MongodbTodo(string connectionString, string database)
    {
        todos = new MongoClient(connectionString).GetDatabase(database).GetCollection<TodoTask>("todos");
    }

    public List<TodoTask> List()
    {
        return todos.Find(_ => true).ToList();
    }

    public TodoTask Add(string description)
    {
        var newTask = new TodoTask(description);
        todos.InsertOne(newTask);
        return newTask;
    }

    public TodoTask? Done(string id)
    {
        return todos.FindOneAndUpdate(
            Builders<TodoTask>.Filter.Eq(todo => todo.Id, id),
            Builders<TodoTask>.Update.Set(todo => todo.Status, TodoStatus.Done));
    }

    public TodoTask Update(TodoTask task)
    {
        return todos.FindOneAndReplace(Builders<TodoTask>.Filter.Eq(todo => todo.Id, task.Id), task);
    }

    public TodoTask? Delete(string id)
    {
        return todos.FindOneAndDelete(Builders<TodoTask>.Filter.Eq(todo => todo.Id, id));
    }
}