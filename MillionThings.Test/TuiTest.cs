using Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillionThings.Test
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

            string[] expectedOutput = new string[] { 
                "Todos:",
                "\nPlease enter command:",
                "     add: Add new todo",
                "    done: Mark a todo as done",
                "    quit: Exit from todo app",
                "#>"};
            string[] actualOutput = writer.ToString().Trim().Split(Environment.NewLine);
            Assert.Equal(expectedOutput, actualOutput);

        }
    }
}
