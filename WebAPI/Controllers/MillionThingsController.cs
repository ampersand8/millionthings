using Microsoft.AspNetCore.Mvc;
using MillionThings;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers;

[Route("api/todos")]
[ApiController]
public class MillionThingsController : ControllerBase
{
    private Todo todo;
    public MillionThingsController()
    {
        todo = new JsonFileTodo("todos.json");

    }

    [HttpGet]
    public IEnumerable<TodoItem> Get()
    {
        return todo.List();
    }

    [HttpGet("{id}")]
    public TodoItem Get(string id)
    {
        return todo.List().Find(t => t.Id.Equals(id));
    }

    [HttpPost]
    public void Post([FromBody] string description)
    {
        todo.Add(description);
    }

    [HttpPut("{id}")]
    public void Put(string id, [FromBody] string value)
    {
        todo.Update(new TodoItem { Id = id, Description = value });
    }

    [HttpPost("{id}/done")]
    public void Delete(string id)
    {
        todo.Done(id);
    }
}
