using Microsoft.AspNetCore.Mvc;
using MillionThings;
using MillionThings.Core;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MillionThings.WebAPI.Controllers;

[Route("api/todos")]
[ApiController]
public class MillionThingsController : ControllerBase
{
    private readonly Dictionary<string, Todo> todoLists = new Dictionary<string, Todo>();
    public MillionThingsController()
    {
    }

    [HttpGet("{list}")]
    public IEnumerable<TodoTask> Get(string list)
    {
        return GetTodo(list).List();
    }

    [HttpGet("{list}/{id}")]
    public TodoTask? Get(string list, string id)
    {

        return GetTodo(list).List().Find(t => t.Id.Equals(id));
    }

    [HttpPost("{list}")]
    public TodoTask Post(string list, [FromBody] string description)
    {
        return GetTodo(list).Add(description);
    }

    [HttpPut("{list}/{id}")]
    public TodoTask Put(string list, string id, [FromBody] string value)
    {
        return GetTodo(list).Update(new TodoTask { Id = id, Description = value });
    }

    [HttpPost("{list}/{id}/done")]
    public TodoTask? Done(string list, string id)
    {
        return GetTodo(list).Done(id);
    }

    private Todo GetTodo(string todoList)
    {
        todoLists.TryGetValue(todoList, out Todo? todo);
        if (todo is not null) return todo;
        todo = new JsonFileTodo($"{todoList}.json");
        todoLists.Add(todoList, todo);
        return todo;
    }
}
