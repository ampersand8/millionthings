using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillionThings.Core;

public interface Todo
{
    List<TodoItem> List();
    void Add(string description);
    void Done(string id);
    void Update(TodoItem item);
    void Delete(string id);
}