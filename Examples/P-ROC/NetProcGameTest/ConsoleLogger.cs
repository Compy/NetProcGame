using NetProcGame.Tools;
using System;

namespace PinprocTest
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string text)
        {
            Console.WriteLine(text);
        }
    }
}
