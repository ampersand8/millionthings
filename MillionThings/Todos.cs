using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillionThings.Core;

public interface Todos
{
    string NameTodo(string todoId);
    TodoData RenameTodo(string todoId, string newName);
    TodoData? DeleteTodo(string todoId);
    TodoData AddTodo(TodoData todo);
    TodoData AddTodo(string name);

    TodoData GetTodo(string todoId);

    List<TodoData> ListTodos();
    
    List<TodoTask> ListTasks(string todoId);
    TodoTask AddTask(string todoId, string description);
    /// <summary>
    /// Removes a task
    /// </summary>
    /// <returns>Task that was deleted. Null if no matching task was found.</returns>
    TodoTask? DeleteTask(string todoId, string taskId);
    
    /// <summary>
    /// Marks the given task as done
    /// </summary>
    /// <returns>Task that was marked as done. Null if no matching task was found.</returns>
    TodoTask? DoneTask(string todoId, string taskId);
    TodoTask UpdateTask(string todoId, TodoTask task);

}