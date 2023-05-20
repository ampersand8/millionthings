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

    public IndexModel(ILogger<IndexModel> logger)
    {
        this.logger = logger;
    }

    public void OnGet()
    {
        Request.Query.TryGetValue("action", out var action);
        Request.Query.TryGetValue("id", out var id);
        if (action != "finish" || StringValues.IsNullOrEmpty(id)) return;
        logger.LogInformation("Setting todo {} to done", id!);
        todo.Done(id!);
        if (Request.Query.Count > 0) Response.Redirect("/");
    }

    public IActionResult OnPost(string description)
    {
        if (!ModelState.IsValid) return RedirectToPage("Index");
        logger.LogInformation("I just got a post request! {}", description);
        todo.Add(description);
        return RedirectToPage("Index");
    }
}