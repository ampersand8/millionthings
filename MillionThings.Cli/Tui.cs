using MillionThings.Core;

namespace MillionThings.Cli;

public class Tui
{
    private readonly Todo todo;
    private List<TodoTask> todos = new List<TodoTask>();
    private readonly Dictionary<string, Command> commands = new Dictionary<string, Command>();
    private readonly TextReader input;
    private readonly TextWriter output;

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

            string? stringInput = input.ReadLine();
            parsedInput = ParseStringCommand(stringInput);
            if (commands.TryGetValue(parsedInput, out Command? currentCommand))
            {
                currentCommand.Run();
            }
            else
            {
                PrintUnknownCommand(stringInput);
            }
        } while (parsedInput != "quit");
    }

    private static string AskForFilePath(TextReader input, TextWriter output)
    {
        do
        {
            output.Write("Please enter path to todo json file: ");
            string? path = input.ReadLine();
            if (path is null)
            {
                output.Write("Empty path is invalid");
                continue;
            }

            try
            {
                if (File.Exists(path))
                {
                    return path;
                }

                File.Create(path).Close();
                return path;
            }
            catch (IOException)
            {
                output.Write("File {0} does not exist and can not be created", path);
            }
        } while (true);
    }

    private string ParseStringCommand(string? command)
    {
        if (command == null) return "unknown";
        if (commands.ContainsKey(command))
        {
            return command;
        }

        return commands.FirstOrDefault(c => c.Value.IsCommand(command)).Key ?? "unknown";
    }

    private void UpdateTodo()
    {
        if (todos.Count == 0)
        {
            output.Write("No todos to edit");
            return;
        }

        int id = AskId();
        string description = AskDescription();
        if (id <= todos.Count)
        {
            todo.Update(new TodoTask(todos[id - 1].Id, description));
        }

        output.WriteLine("Updated");
    }

    private int AskId()
    {
        var successfulParse = false;
        int id;
        do
        {
            output.Write("id: ");
            string? inputId = input.ReadLine();
            successfulParse = Int32.TryParse(inputId, out id);
            if (!successfulParse)
            {
                output.WriteLine("Invalid id '{0}', choose between 1 and {1}", inputId, todos.Count);
            }
            else if (id > todos.Count)
            {
                output.WriteLine("Id not available, choose between 1 and {0}", todos.Count);
                successfulParse = false;
            }
        } while (!successfulParse);

        return id;
    }

    private string AskDescription()
    {
        var successfulDescription = false;
        string description = "";
        do
        {
            output.Write("description: ");
            string? inDescription = input.ReadLine();
            if (inDescription is null || inDescription.Length == 0)
            {
                output.WriteLine("Description can not be empty. Given: {0}", inDescription);
            }
            else
            {
                successfulDescription = true;
                description = inDescription;
            }
        } while (!successfulDescription);

        return description;
    }

    private void AddQuestion()
    {
        string description = AskDescription();
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

        int id = AskId();
        todo.Done(todos[id - 1].Id);

        output.WriteLine("Done");
    }

    private void PrintCommandQuery()
    {
        output.WriteLine(Environment.NewLine + "Please enter command:");
        foreach (var command in commands.Values)
        {
            output.WriteLine($"{command.GetCommands()[0],8}: {command.GetDescription()}");
        }

        output.Write("#> ");
    }

    private void PrintTodos()
    {
        output.WriteLine(Environment.NewLine + "Todos:");
        todos = todo.List().FindAll(todo => todo.Status == TodoStatus.Open);
        for (int index = 0; index < todos.Count; index++)
        {
            output.WriteLine($"{index + 1,5}): {todos[index].Description}");
        }
    }

    private void PrintUnknownCommand(string? command)
    {
        output.WriteLine($"Unknown command: {command}" + Environment.NewLine);
    }
}