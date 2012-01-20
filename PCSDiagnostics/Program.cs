using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using NetProcGame.tools;

namespace PCSDiagnostics
{
    static class Program
    {
        public static DiagnosticGame Game;
        public static BackgroundWorker GameThread;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmDiagnostics());
        }

        public static void StartGameThread(ILogger logger = null)
        {
            GameThread = new BackgroundWorker();
            GameThread.DoWork += new DoWorkEventHandler(GameThread_DoWork);
            GameThread.WorkerSupportsCancellation = true;
            GameThread.RunWorkerAsync(logger);
        }

        static void GameThread_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (e.Argument != null)
                    Game = new DiagnosticGame((ILogger)e.Argument);
                else
                    Game = new DiagnosticGame();

                    Game.run_loop();
            }
            catch (Exception ex)
            {
                Game.Logger.Log("Exception in run_loop()\r\n" + ex.ToString());
            }
        }
    }
}
