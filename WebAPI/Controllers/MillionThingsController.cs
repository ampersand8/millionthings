using Microsoft.AspNetCore.Mvc;
using MillionThings;
using MillionThings.Core;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers;

[Route("api/todos")]
[ApiController]
public class MillionThingsController : ControllerBase
{
    private Dictionary<string, Todo> todoLists = new Dictionary<string, Todo>();
    public MillionThingsController()
    {
    }

    [HttpGet("{list}")]
    public IEnumerable<TodoItem> Get(string list)
    {
        return getTodo(list).List();
    }

    [HttpGet("{list}/{id}")]
    public TodoItem Get(string list, string id)
    {

        return getTodo(list).List().Find(t => t.Id.Equals(id));
    }

    [HttpPost("{list}")]
    public void Post(string list, [FromBody] string description)
    {
        getTodo(list).Add(description);
    }

    [HttpPut("{list}/{id}")]
    public void Put(string list, string id, [FromBody] string value)
    {
        getTodo(list).Update(new TodoItem { Id = id, Description = value });
    }

    [HttpPost("{list}/{id}/done")]
    public void Done(string list, string id)
    {
        getTodo(list).Done(id);
    }

    private Todo getTodo(string todoList)
    {
        todoLists.TryGetValue(todoList, out Todo todo);
        if (todo == null)
        {
            todo = new JsonFileTodo($"{todoList}.json");
            todoLists.Add(todoList, todo);
        }
        return todo;
    }
}
