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
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            Console.SetIn(new StringReader(path));

            // Setup expected output
            string expectedOutput = "Please enter path to todo json file: ";
            var writer = new StringWriter();
            Console.SetOut(writer);

            new Tui();

            string actualOutput = writer.ToString();
            Assert.Equal(expectedOutput, actualOutput);
        }
    }
}
