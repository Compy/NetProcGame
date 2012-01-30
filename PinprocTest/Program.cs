using System;
using System.Collections.Generic;
using System.Text;
using System.Yaml.Serialization;
using NetProcGame;
using NetProcGame.config;
using NetProcGame.dmd;

using System.ComponentModel;

namespace PinprocTest
{
    class Program
    {
        public static StarterGame.StarterGame game;

        private static BackgroundWorker worker;

        private static ConsoleLogger logger;

        static void Main(string[] args)
        {
            // Do unit tests 
            Font f = new Font(@"fonts\Jazz18-18px.dmd");
            Frame testFrame = new Frame(128, 32);
            f.draw(testFrame, "NETProcGame", 1, 1);

            bool allZeros = true;
            for (int i = 0; i < testFrame.frame.buffer.Length; i++)
            {
                if (testFrame.frame.buffer[i] != 0)
                {
                    Console.WriteLine("Non-zero byte in frame buffer at " + i.ToString());
                    allZeros = false;
                }
            }

            // Ascii checks out good

            string ascii = f.bitmap.ascii();

            if (allZeros)
                Console.WriteLine("Drawn buffer is all 0s");

            System.Threading.Thread.CurrentThread.Name = "Console Thread";
            logger = new ConsoleLogger();
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);

            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = false;

            worker.DoWork += new DoWorkEventHandler(worker_DoWork);

            //worker.RunWorkerAsync();

            string line = Console.ReadLine();
            while (line != "q" && line != "quit" && line != "exit")
            {
                if (line == "e")
                {
                    game.FlippersEnabled = true;
                }
                if (line == "d")
                {
                    game.FlippersEnabled = false;
                }
                if (line == "r")
                {
                    game.PROC.reset(1);
                }
                if (line == "o")
                {
                    game.open_divertor();
                }
                if (line == "c")
                {
                    game.close_divertor();
                }
                if (line == "b")
                    game.Coils["bottomPopper"].Pulse();
                if (line == "t")
                    game.Coils["topPopper"].Pulse();

                line = Console.ReadLine();
            }

        }

        static void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                System.Threading.Thread.CurrentThread.Name = "p-roc thread";
                game = new StarterGame.StarterGame(logger);
                game.setup();
                game.run_loop();
            }
            catch (Exception ex)
            {
                Console.WriteLine("FATAL ERROR: Could not load P-ROC device.");
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
        }



        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            game.end_run_loop();

            game = null;
        }
    }
}
