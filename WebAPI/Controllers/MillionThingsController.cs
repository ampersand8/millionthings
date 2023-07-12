using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MillionThings.Core;
using MillionThings.Database.MongoDB;
using MillionThings.WebAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MillionThings.WebAPI.Controllers;

[Route("api/v1/todos")]
[ApiController]
public class MillionThingsController : ControllerBase
{
    private readonly Todos todos;
    private readonly ILogger<MillionThingsController> _logger;

    public MillionThingsController(ILoggerFactory loggerFactory, IOptions<MillionThingsDatabaseSettings> settings)
    {
        _logger = loggerFactory.CreateLogger<MillionThingsController>();
        todos = new MongodbTodos(loggerFactory.CreateLogger<MongodbTodos>(), settings.Value.ConnectionString,
            settings.Value.DatabaseName);
    }

    [HttpGet]
    public IEnumerable<TodoData> GetTodoLists()
    {
        _logger.LogInformation("GetTodoLists");
        return todos.ListTodos();
    }

    [HttpGet("{listId}")]
    public ActionResult<TodoData> GetTodoList(string listId)
    {
        TodoData? result = todos.GetTodo(listId);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    public TodoData CreateTodoList([FromBody] string name)
    {
        return todos.AddTodo(name);
    }

    [HttpDelete("{listId}")]
    public ActionResult<TodoData> DeleteTodoList(string listId)
    {
        var deleted =  todos.DeleteTodo(listId);
        if (deleted is null)
        {
            return NotFound();
        }

        return Ok(deleted);
    }

    [HttpGet("{listId}/tasks")]
    public IEnumerable<TodoTask> ListTasks(string listId)
    {
        return todos.ListTasks(listId);
    }

    [HttpGet("{listId}/tasks/{taskId}")]
    public TodoTask? GetTask(string listId, string taskId)
    {
        return todos.GetTask(listId, taskId);
    }

    [HttpPost("{listId}/tasks")]
    public TodoTask CreateTask(string listId, [FromBody] string description)
    {
        return todos.AddTask(listId, description);
    }
    
    [HttpDelete("{listId}/tasks/{taskId}")]
    public ActionResult<TodoTask> DeleteTask(string listId, string taskId)
    {
        var deleted =  todos.DeleteTask(listId, taskId);
        if (deleted is null)
        {
            return NotFound();
        }

        return Ok(deleted);
    }

    [HttpPut("{listId}/tasks/{taskId}")]
    public TodoTask UpdateTask(string listId, string taskId, [FromBody] string value)
    {
        return todos.UpdateTask(listId, new TodoTask { Id = taskId, Description = value });
    }

    [HttpPost("{listId}/tasks/{taskId}/done")]
    public TodoTask? FinishTask(string listId, string taskId)
    {
        return todos.DoneTask(listId, taskId);
    }
}