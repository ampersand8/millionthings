using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;
using MillionThings.Core;

namespace MillionThings.SimpleWebApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> logger;
    private readonly Todos todoLists;
    private TodoData todo => todoLists.GetTodo(todoId);
    private string todoId;

    public List<TodoTask> Tasks => todo.Tasks.FindAll(t => t.Status == TodoStatus.Open);
    public List<TodoTask> DoneTodos => todo.Tasks.FindAll(t => t.Status == TodoStatus.Done);

    public IndexModel(ILogger<IndexModel> logger, string todoFile = "todos.json")
    {
        this.logger = logger;
        todoLists = new JsonFileTodos(todoFile);
        
        if (todoLists.ListTodos().Count == 0)
        {
            var defaultTodo = new TodoData(Guid.NewGuid().ToString(), "default", new());
            todoId = defaultTodo.Id;
            todoLists.AddTodo(defaultTodo);
        } else
        {
            todoId = todoLists.ListTodos()[0].Id;
        }

    }

    public void OnGet()
    {
        if (Request.Query.TryGetValue("todos", out StringValues inTodoId))
        {
            todoId = inTodoId[0] ?? "";
        }
        Request.Query.TryGetValue("action", out var action);
        Request.Query.TryGetValue("id", out var id);
        if (StringValues.IsNullOrEmpty(action) || StringValues.IsNullOrEmpty(id)) return;
        switch (action)
        {
            case "finish":
                Finish(id);
                break;
            case "reopen":
                Reopen(id);
                break;
            case "delete":
                Delete(id);
                break;
            default:
                logger.LogInformation("Unknown action {}", action.ToString());
                break;
        }

        if (Request.Query.Count > 0) Response.Redirect("/");
    }

    public IActionResult OnPost(string description)
    {
        if (!ModelState.IsValid) return RedirectToPage("Index");
        logger.LogInformation("I just got a post request! {}", description);
        todoLists.AddTask(todoId, description);
        return RedirectToPage("Index");
    }

    public IActionResult OnPostEdit([FromForm] Dictionary<string,string> model)
    {
        logger.LogInformation("Updating task: {}", model);
        todoLists.UpdateTask(todoId, new TodoTask(model["id"], model["description"], TodoStatus.Open));
        return RedirectToPage("Index");
    }

    private void Finish(string? taskId)
    {
        if (taskId == null) return;
        logger.LogInformation("Setting todo {} to done", taskId);
        todoLists.DoneTask(todoId, taskId);
    }

    private void Reopen(string? taskId)
    {
        if (taskId == null) return;
        TodoTask? task = todoLists.ListTasks(todoId).Find(t => t.Id == taskId);
        if (task == null) return;
        logger.LogInformation("Reopening todo {}", taskId);
        todoLists.UpdateTask(todoId, new TodoTask { Id = task.Id, Description = task.Description, Status = TodoStatus.Open });
    }

    private void Delete(string? taskId)
    {
        if (taskId == null) return;
        logger.LogInformation("Deleting todo {}", taskId);
        todoLists.DeleteTask(todoId, taskId);
    }
}