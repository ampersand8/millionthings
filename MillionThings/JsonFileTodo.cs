using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MillionThings
{
    public class JsonFileTodo : Todo
    {
        public List<TodoItem> todos;
        private string path;

        public JsonFileTodo(string path)
        {
            this.path = path;
            CreateFileIfNotExists(path);
            todos = LoadJsonFile(path);
        }


        public List<TodoItem> List() { return todos; }

        public void Add(string description)
        {
            if (!todos.Any(todo => todo.Description == description))
            {
                todos.Add(new() { Description = description });
                PersistToFile();
            }
        }

        public void Done(string id)
        {
            todos.RemoveAll(todo => todo.Id == id);
            PersistToFile();
        }
        private static List<TodoItem> LoadJsonFile(string path)
        {
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                return JsonSerializer.Deserialize<List<TodoItem>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
        }

        public void Update(TodoItem item)
        {
            var index = todos.FindIndex(todo => todo.Id == item.Id);
            if (index > -1)
            {
                todos[index] = item;
            } else
            {
                Add(item.Description);
            }
            PersistToFile();
        }

        private static void CreateFileIfNotExists(string path)
        {
            if (File.Exists(path))
            {
                return;
            }
            File.WriteAllText(path, "[]");
        }

        private void PersistToFile()
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.Write(JsonSerializer.Serialize(todos));
            }
        }
    }
}
