using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;
using MillionThings.Core;

namespace MillionThings.SimpleWebApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> logger;
    private readonly Todo todo = new JsonFileTodo("todos.json");

    public List<TodoItem> Todos => todo.List().FindAll(t => t.Status == TodoStatus.Open);
    public List<TodoItem> DoneTodos => todo.List().FindAll(t => t.Status == TodoStatus.Done);

    public IndexModel(ILogger<IndexModel> logger)
    {
        this.logger = logger;
    }

    public void OnGet()
    {
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
                logger.LogInformation("Unknown action {}", action);
                break;
        }

        if (Request.Query.Count > 0) Response.Redirect("/");
    }

    public IActionResult OnPost(string description)
    {
        if (!ModelState.IsValid) return RedirectToPage("Index");
        logger.LogInformation("I just got a post request! {}", description);
        todo.Add(description);
        return RedirectToPage("Index");
    }

    private void Finish(string? id)
    {
        if (id == null) return;
        logger.LogInformation("Setting todo {} to done", id);
        todo.Done(id);
    }

    private void Reopen(string? id)
    {
        if (id == null) return;
        TodoItem? task = todo.List().Find(t => t.Id == id);
        if (task == null) return;
        logger.LogInformation("Reopening todo {}", id);
        todo.Update(new TodoItem { Id = task.Id, Description = task.Description, Status = TodoStatus.Open });
    }

    private void Delete(string? id)
    {
        if (id == null) return;
        logger.LogInformation("Deleting todo {}", id);
        todo.Delete(id);
    }
}