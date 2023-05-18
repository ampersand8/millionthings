namespace MillionThings
{
    public class Tui
    {
        private Todo todo;
        private List<TodoItem> todos = new List<TodoItem>();
        private Dictionary<string, Command> commands = new Dictionary<string, Command>();
        private TextReader input;
        private TextWriter output;

        public Tui(TextReader input, TextWriter output) : this(input, output, AskForFilePath(input, output))
        {
        }



        public Tui(TextReader input, TextWriter output, string todoPath)
        {
            this.input = input;
            this.output = output;

            todo = new JsonFileTodo(todoPath);

            commands.Add("add", new Command("Add new todo", AddQuestion, "add", "a"));
            commands.Add("done", new Command("Mark a todo as done", DoneQuestion, "done", "d"));
            commands.Add("edit", new Command("Edit a todo", UpdateTodo, "edit", "e"));
            commands.Add("quit", new Command("Quit from todo app", () => { }, "quit", "exit", "q"));
        }

        public void Run()
        {
            string parsedInput = "";
            do
            {
                PrintTodos();
                PrintCommandQuery();

                string stringInput = input.ReadLine();
                parsedInput = ParseStringCommand(stringInput);
                if (commands.ContainsKey(parsedInput))
                {
                    Command currentCommand = commands[parsedInput];
                    currentCommand.Run();
                }
                else
                {
                    PrintUnknownCommand(stringInput);
                    continue;
                }
            } while (parsedInput != "quit");
        }

        private static string AskForFilePath(TextReader input, TextWriter output)
        {
            output.Write("Please enter path to todo json file: ");
            return input.ReadLine();
        }

        private string ParseStringCommand(string command)
        {
            if (command == null) return "unknown";
            if (commands.ContainsKey(command))
            {
                return command;
            }
            foreach (var cmd in commands)
            {
                if (cmd.Value.IsCommand(command))
                {
                    return cmd.Key;
                }
            }
            return "unknown";
        }

        private void UpdateTodo()
        {
            if (todos.Count == 0)
            {
                output.Write("No todos to edit");
                return;
            }
            output.Write("id: ");
            string inputId = input.ReadLine();
            int? id = Int32.TryParse(inputId, out int parsedId) ? parsedId : null;
            output.Write("description: ");
            string description = input.ReadLine();
            if (id is not null && id <= todos.Count)
            {
                todo.Update(new TodoItem(todos[id.Value - 1].Id, description));
            }
            output.WriteLine("Updated");
        }

        private void AddQuestion()
        {
            output.Write("description: ");
            string description = input.ReadLine();
            todo.Add(description);
            output.WriteLine("Added");
        }

        private void DoneQuestion()
        {
            if (todos.Count == 0)
            {
                output.Write("No todos to finish");
                return;
            }
            output.Write("id: ");
            string inputId = input.ReadLine();
            int? id = Int32.TryParse(inputId, out int parsedId) ? parsedId : null;
            if (id is not null && id <= todos.Count)
            {
                todo.Done(todos[id.Value - 1].Id);
            }
            output.WriteLine("Done");
        }

        private void PrintCommandQuery()
        {
            output.WriteLine("\nPlease enter command:");
            foreach (var command in commands)
            {
                output.WriteLine($"{command.Value.GetCommands()[0],8}: {command.Value.GetDescription()}");
            }
            output.Write("#> ");
        }

        private void PrintTodos()
        {
            output.WriteLine("\nTodos:");
            todos = todo.List().FindAll(todo => todo.Status == TodoStatus.Open);
            for (int index = 0; index < todos.Count; index++)
            {
                output.WriteLine($"{index + 1,5}): {todos[index].Description}");
            }
        }

        private void PrintUnknownCommand(string command)
        {
            output.WriteLine($"Unknown command: {command}\n");
        }
    }
}
