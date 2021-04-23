using System;
using System.IO;
using System.Text;
using Xunit.Abstractions;

namespace Rogero.Common.Tests
{
    public class TestOutputHelperToTextWriterAdapter : TextWriter
    {
        ITestOutputHelper _output;
        public TestOutputHelperToTextWriterAdapter(ITestOutputHelper output)
        {
            _output = output;
        }
        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
        public override void WriteLine(string message)
        {
            _output.WriteLine(message);
        }
        public override void WriteLine(string format, params object[] args)
        {
            _output.WriteLine(format, args);
        }

        public override void Write(char value)
        {
        }

        public override void Write(string message)
        {
            _output.WriteLine(message);
        }
    }

    public class UnitTestBaseWithConsoleRedirection
    {
        protected ITestOutputHelper _outputHelper;

        public UnitTestBaseWithConsoleRedirection(ITestOutputHelper outputHelperHelper)
        {
            Console.SetOut(new TestOutputHelperToTextWriterAdapter(outputHelperHelper));
            _outputHelper = outputHelperHelper;
        }
    }
}