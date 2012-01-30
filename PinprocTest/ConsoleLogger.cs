using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetProcGame.config;
using NetProcGame.game;
using NetProcGame.tools;

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
