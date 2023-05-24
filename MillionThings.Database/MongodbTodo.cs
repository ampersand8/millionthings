using MillionThings.Core;
using MongoDB.Driver;

namespace MillionThings.Database;

public class MongodbTodo : Todo
{
    private readonly IMongoCollection<TodoItem> todos;

    public MongodbTodo(string connectionString, string database)
    {
        todos = new MongoClient(connectionString).GetDatabase(database).GetCollection<TodoItem>("todos");
    }

    public List<TodoItem> List()
    {
        return todos.Find(_ => true).ToList();
    }

    public void Add(string description)
    {
        todos.InsertOne(new TodoItem(description));
    }

    public void Done(string id)
    {
        todos.UpdateOne(
            Builders<TodoItem>.Filter.Eq(todo => todo.Id, id),
            Builders<TodoItem>.Update.Set(todo => todo.Status, TodoStatus.Done));
    }

    public void Update(TodoItem item)
    {
        todos.ReplaceOne(Builders<TodoItem>.Filter.Eq(todo => todo.Id, item.Id), item);
    }

    public void Delete(string id)
    {
        todos.DeleteOne(Builders<TodoItem>.Filter.Eq(todo => todo.Id, id));
    }
}