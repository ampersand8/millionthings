using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillionThings
{
    internal class Command
    {
        public delegate void CommandAction();
        private readonly string[] commands;
        private readonly string description;
        private readonly CommandAction action;

        public Command(string description, CommandAction action, params string[] commands)
        {
            this.commands = commands;
            this.description = description;
            this.action = action;
        }

        public bool IsCommand(string command)
        {
            return commands.Any(c => c == command);
        }

        public string GetDescription()
        {
            return description;
        }

        public string[] GetCommands()
        {
            return commands;
        }

        public void Run()
        {
            action();
        }
    }
}
