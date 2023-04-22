using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            commands.Add("exit", new Command("Exit from todo app", () => { }, "quit", "exit", "e", "q"));
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
                } else
                {
                    PrintUnknownCommand(stringInput);
                    continue;
                }
            } while (parsedInput != "exit" );
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

        private void AddQuestion()
        {
            output.Write("description: ");
            string description = input.ReadLine();
            todo.Add(description);
            output.WriteLine("Added");
        }

        private void DoneQuestion()
        {
            output.Write("id: ");
            string id = input.ReadLine();
            todo.Done(todos[Int32.Parse(id)].Id);
            output.WriteLine("Done");
        }

        private void PrintCommandQuery()
        {
            output.WriteLine("\nPlease enter command:");
            foreach (var command  in commands)
            {
                output.WriteLine($"{command.Value.GetCommands()[0],8}: {command.Value.GetDescription()}");
            }
            output.Write("#> ");
        }

        private void PrintTodos()
        {
            output.WriteLine("\nTodos:");
            todos = todo.List();
            for (int index = 0; index < todos.Count; index++)
            {
                output.WriteLine($"{index + 1, 5}): {todos[index].Description}");
            }
        }

        private void PrintUnknownCommand(string command)
        {
            output.WriteLine($"Unknown command: {command}\n");
        }
    }
}
