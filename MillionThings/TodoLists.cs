namespace MillionThings.Core;

public interface TodoLists
{
    Dictionary<string, Todo> Todos();
    Todo Add(JsonFileTodo todo);
    Todo Get(string name);
    Todo? Delete(string name);
}