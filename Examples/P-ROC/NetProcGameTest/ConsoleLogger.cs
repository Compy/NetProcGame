using NetProc;
using System;

namespace PinprocTest
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string text)
        {
            Console.WriteLine(text);
        }

        public void Log(string text, LogLevel logLevel = LogLevel.Info)
        {
            Console.WriteLine(text);
        }
    }
}
