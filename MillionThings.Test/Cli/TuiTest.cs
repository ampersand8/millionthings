﻿using MillionThings.Cli;

namespace MillionThings.Test.Cli
{
    public class TuiTest
    {
        [Fact]
        public void ShouldShowWelcomeScreen()
        {
            // Setup requested input
            string filename = Guid.NewGuid().ToString();
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename) + "\n";
            Console.SetIn(new StringReader(path));

            // Setup expected output
            string[] expectedOutput = new string[] { "Please enter path to todo json file:" };
            var writer = new StringWriter();
            var input = new StringReader(path);

            new Tui(input, writer);

            string[] actualOutput = writer.ToString().Trim().Split(Environment.NewLine);
            Assert.Equal(expectedOutput, actualOutput);
        }

        [Fact]
        public void ShouldShowMenu()
        {

            // Setup requested input
            string filename = Guid.NewGuid().ToString();
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            var writer = new StringWriter();
            var input = new StringReader("quit\n");

            var sut = new Tui(input, writer, path);

            sut.Run();

            List<string> expectedOutput = new() { "Todos:" };
            expectedOutput.AddRange(enterCommandPrompt);
            expectedOutput.Add("#>");
            string[] actualOutput = writer.ToString().Trim().Split(Environment.NewLine);
            Assert.Equal(expectedOutput, actualOutput);

        }

        [Fact]
        public void ShouldShowAddMenu()
        {

            // Setup requested input
            string filename = Guid.NewGuid().ToString();
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            var writer = new StringWriter();
            var input = new StringReader("add\nthis is a test\nquit\n");

            var sut = new Tui(input, writer, path);

            sut.Run();

            List<string> expectedOutput = new() { "Todos:" };
            expectedOutput.AddRange(enterCommandPrompt);
            expectedOutput.AddRange(new[] {"#> description: Added", "\nTodos:", "    1): this is a test", });
            expectedOutput.AddRange(enterCommandPrompt);
            expectedOutput.Add("#>");
            string[] actualOutput = writer.ToString().Trim().Split(Environment.NewLine);
            Assert.Equal(expectedOutput, actualOutput);
        }

        [Fact]
        public void ShouldShowMarkingATodoAsDone()
        {

            // Setup requested input
            string filename = Guid.NewGuid().ToString();
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            var writer = new StringWriter();
            var input = new StringReader("add\nthis is a test\ndone\n1\nquit\n");

            var sut = new Tui(input, writer, path);

            sut.Run();


            List<string> expectedOutput = new() { "Todos:" };
            expectedOutput.AddRange(enterCommandPrompt);
            expectedOutput.AddRange(new[] { "#> description: Added", "\nTodos:", "    1): this is a test" });
            expectedOutput.AddRange(enterCommandPrompt);
            expectedOutput.AddRange(new[] { "#> id: Done", "\nTodos:", });
            expectedOutput.AddRange(enterCommandPrompt);
            expectedOutput.Add("#>");
            string[] actualOutput = writer.ToString().Trim().Split(Environment.NewLine);
            Assert.Equal(expectedOutput, actualOutput);
        }

        [Fact]
        public void ShouldShowEditingATodo()
        {

            // Setup requested input
            string filename = Guid.NewGuid().ToString();
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            var writer = new StringWriter();
            var input = new StringReader("add\nthis is a test\nedit\n1\nnot sure if it's a test\nquit\n");

            var sut = new Tui(input, writer, path);

            sut.Run();


            List<string> expectedOutput = new() { "Todos:" };
            expectedOutput.AddRange(enterCommandPrompt);
            expectedOutput.AddRange(new[] { "#> description: Added", "\nTodos:", "    1): this is a test" });
            expectedOutput.AddRange(enterCommandPrompt);
            expectedOutput.AddRange(new[] { "#> id: description: Updated", "\nTodos:", "    1): not sure if it's a test" });
            expectedOutput.AddRange(enterCommandPrompt);
            expectedOutput.Add("#>");
            string[] actualOutput = writer.ToString().Trim().Split(Environment.NewLine);
            Assert.Equal(expectedOutput, actualOutput);
        }

        [Fact]
        public void ShouldShowUnknownCommand()
        {

            // Setup requested input
            string filename = Guid.NewGuid().ToString();
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            var writer = new StringWriter();
            var input = new StringReader("asdf\nquit\n");

            var sut = new Tui(input, writer, path);

            sut.Run();

            List<string> expectedOutput = new() { "Todos:" };
            expectedOutput.AddRange(enterCommandPrompt);
            expectedOutput.AddRange(new[] { "#> Unknown command: asdf\n", "\nTodos:" });
            expectedOutput.AddRange(enterCommandPrompt);
            expectedOutput.Add("#>");
            
            string[] actualOutput = writer.ToString().Trim().Split(Environment.NewLine);
            Assert.Equal(expectedOutput, actualOutput);
        }

        private List<string> enterCommandPrompt = new List<string>() {
            "\nPlease enter command:",
            "     add: Add new todo",
            "    done: Mark a todo as done",
            "    edit: Edit a todo",
            "    quit: Quit from todo app", 
        };
    }
}