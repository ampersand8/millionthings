namespace MillionThings.Core;

public interface TodoLists
{
    Dictionary<string, Todo> Todos();
    Tuple<string, Todo> Add(Todo todo);
    Todo Get(string name);
    Todo Delete(string name);
}