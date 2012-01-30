using System;
using System.Collections.Generic;
using System.Text;
using System.Yaml.Serialization;
using NetProcGame;
using NetProcGame.config;

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
            System.Threading.Thread.CurrentThread.Name = "Console Thread";
            //Config cfg = Config.CreateFromFile(@"C:\Users\Jimmy\Documents\Pinball\demo_man\config\dm.yaml");

            //MachineConfiguration mc = new MachineConfiguration();
            //mc.PRGame.machineType = MachineType.WPC;
            //mc.PRGame.numBalls = 3;
            //mc.PRFlippers.Add("flipperLwR");
            //mc.PRFlippers.Add("flipperLwL");
            //mc.PRBumpers.Add("leftSling");
            //mc.AddSwitch("leftInlane", "S23");
            //mc.AddLamp("startMultiball", "L34");
            //mc.AddCoil("bottomPopper", "C78");
            //mc.PRBallSave.PulseCoils.Add("leftSlingshot");
            //mc.PRBallSave.PulseCoils.Add("rightSlingshot");
            //mc.PRBallSave.PulseCoils.Add("leftJet");
            //mc.PRBallSave.PulseCoils.Add("rightJet");
            //mc.PRBallSave.PulseCoils.Add("bottomJet");
            //mc.PRBallSave.PulseCoils.Add("eject");
            //mc.PRBallSave.PulseCoils.Add("topPopper");
            //mc.PRBallSave.PulseCoils.Add("bottomPopper");
            //mc.PRBallSave.ResetSwitches.Add("leftSlingshot", "open");

            //string json = mc.ToJSON();

            //MachineConfiguration testmc = MachineConfiguration.FromFile(@"C:\Users\Jimmy\Documents\Pinball\dm_reloaded\config\machine.json");

            logger = new ConsoleLogger();

            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);

            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = false;

            worker.DoWork += new DoWorkEventHandler(worker_DoWork);

            worker.RunWorkerAsync();

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
