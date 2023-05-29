using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillionThings.Core;

public interface Todo
{
    string Name();
    Todo Rename(string newName);
    List<TodoTask> List();
    TodoTask Add(string description);
    /// <summary>
    /// Marks the given task as done
    /// </summary>
    /// <returns>Task that was marked as done. Null if no matching task was found.</returns>
    TodoTask? Done(string id);
    TodoTask Update(TodoTask task);
    /// <summary>
    /// Removes a task
    /// </summary>
    /// <returns>Task that was deleted. Null if no matching task was found.</returns>
    TodoTask? Delete(string id);
}