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
        private List<TodoItem> todos;
        private Dictionary<string, Command> commands = new Dictionary<string, Command>();

        public Tui()
        {
            Console.Write("Please enter path to todo json file: ");
            string todoPath = Console.ReadLine();
            todo = new JsonFileTodo(todoPath);

            commands.Add("add", new Command("Add new todo", AddQuestion, false, "add", "a"));
            commands.Add("done", new Command("Mark a todo as done", DoneQuestion, false, "done", "d"));
            commands.Add("exit", new Command("Exit from todo app", () => { }, true, "exit", "e", "q"));
        }

        public void Run()
        {

            string stringInput;
            do
            {
                PrintTodos();
                PrintCommandQuery();
                stringInput = Console.ReadLine();
                string parsedInput = ParseStringCommand(stringInput);
                if (commands.ContainsKey(parsedInput))
                {
                    Command currentCommand = commands[parsedInput];
                    currentCommand.Run();
                    if (currentCommand.Exits()) Environment.Exit(0);
                } else
                {
                    PrintUnknownCommand(stringInput);
                    continue;
                }
            } while (stringInput != "exit" );
        }

        private string ParseStringCommand( string command )
        {
            if (commands.ContainsKey( command ))
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
            Console.Write("description: ");
            string description = Console.ReadLine();
            todo.Add(description);
            Console.WriteLine("Added");
        }

        private void DoneQuestion()
        {
            Console.Write("id: ");
            string id = Console.ReadLine();
            todo.Done(todos[Int32.Parse(id)].Id);
            Console.WriteLine("Done");
        }

        private void PrintCommandQuery()
        {
            Console.WriteLine("\nPlease enter command:");
            foreach (var command  in commands)
            {
                Console.WriteLine($"{command.Value.GetCommands()[0],8}: {command.Value.GetDescription()}");
            }
            Console.Write("#> ");
        }

        private void PrintTodos()
        {
            Console.WriteLine("\nTodos:");
            todos = todo.List();
            for (int index = 0; index < todos.Count; index++)
            {
                Console.WriteLine($"{index + 1, 5}): {todos[index].Description}");
            }
        }

        private void PrintUnknownCommand(string command)
        {
            Console.WriteLine($"Unknown command: {command}\n");
        }
    }
}
