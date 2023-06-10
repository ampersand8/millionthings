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
    public TodoData CurrentTodo => todoLists.GetTodo(todoId);
    public List<TodoData> AllTodos => todoLists.ListTodos();
    private string todoId;

    public List<TodoTask> Tasks => CurrentTodo.Tasks.FindAll(t => t.Status == TodoStatus.Open);
    public List<TodoTask> DoneTasks => CurrentTodo.Tasks.FindAll(t => t.Status == TodoStatus.Done);
    public string CurrentTodoId => CurrentTodo.Id;

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

    public ActionResult OnGetTodo(string todo, string action)
    {
        logger.LogDebug("Talking to OnTodoGet with parameters {} and {}", todo, action);
        if (action == "delete")
        {
            logger.LogDebug("Deleting todo {}", todo);
            todoLists.DeleteTodo(todo);
        }
        return RedirectToPage("Index");
    }

    public void OnGet()
    {
        UpdateTodoId();
        Request.Query.TryGetValue("action", out var action);
        Request.Query.TryGetValue("id", out var id);
        if (StringValues.IsNullOrEmpty(action) || StringValues.IsNullOrEmpty(id)) return;
        switch (action)
        {
            case "finish":
                Finish(id);
                Response.Redirect("/?todo=" + todoId);
                break;
            case "reopen":
                Reopen(id);
                Response.Redirect("/?todo=" + todoId);
                break;
            case "delete":
                Delete(id);
                Response.Redirect("/?todo=" + todoId);
                break;
            default:
                logger.LogDebug("Unknown action {}", action.ToString());
                break;
        }
    }

    public IActionResult OnPost(string description)
    {
        UpdateTodoId();
        if (!ModelState.IsValid) return RedirectToPage("Index?todo=" + todoId);
        logger.LogDebug("I just got a post request! {}", description);
        todoLists.AddTask(todoId, description);
        return RedirectToPage("Index", new { todo = todoId });
    }

    public IActionResult OnPostEdit([FromForm] Dictionary<string,string> model)
    {
        UpdateTodoId();
        logger.LogDebug("Updating task: {}", model);
        todoLists.UpdateTask(model["todoId"], new TodoTask(model["id"], model["description"], TodoStatus.Open));
        return RedirectToPage("Index", new { todo = model["todoId"] });

    }
    
    public IActionResult OnPostAddTodo([FromForm] Dictionary<string,string> model)
    {
        UpdateTodoId();
        logger.LogDebug("Adding todo: {}", model);
        todoLists.AddTodo(new TodoData(Guid.NewGuid().ToString(), model["name"], new()));
        return RedirectToPage("Index", new { todo = todoId });

    }

    private void Finish(string? taskId)
    {
        if (taskId == null) return;
        logger.LogDebug("Setting todo {} to done", taskId);
        todoLists.DoneTask(todoId, taskId);
    }

    private void Reopen(string? taskId)
    {
        if (taskId == null) return;
        TodoTask? task = todoLists.ListTasks(todoId).Find(t => t.Id == taskId);
        if (task == null) return;
        logger.LogDebug("Reopening todo {}", taskId);
        todoLists.UpdateTask(todoId, new TodoTask { Id = task.Id, Description = task.Description, Status = TodoStatus.Open });
    }

    private void Delete(string? taskId)
    {
        if (taskId == null) return;
        logger.LogDebug("Deleting todo {}", taskId);
        todoLists.DeleteTask(todoId, taskId);
    }

    private void UpdateTodoId()
    {
        if (Request.Query.TryGetValue("todo", out StringValues inTodoId))
        {
            todoId = inTodoId[0] ?? todoId;
        }
    }
}