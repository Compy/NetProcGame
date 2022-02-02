using NetProcGame;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetProc.ProcDevices
{
    class Program
    {
        static CancellationTokenSource source = new CancellationTokenSource();
        static async Task Main(string[] args)
        {
            ProcDevice proc = null;
            try
            {
                Console.WriteLine("Creating PROC");
                proc = new ProcDevice(MachineType.PDB);
                await Task.Delay(100);
                proc?.Reset(1);

                //listen for cancel keypress and end run loop
                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    Console.WriteLine("ctrl+C triggered");
                    source.Cancel();
                    eventArgs.Cancel = true;
                };

                //run game loop
                await run_loop(proc);

                //close console
                Console.WriteLine("netprocgame closing...");              
                await Task.Delay(500);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static Task run_loop(ProcDevice proc)
        {
            long loops = 0;
            Event[] events;
            while (!source.IsCancellationRequested)
            {
                loops++;
                events = proc.Getevents();
                if (events != null)
                {
                    foreach (Event evt in events)
                    {
                        Console.WriteLine("  {0}", evt);
                    }
                }

                proc.WatchDogTickle();
            }

            proc.Close();
            return Task.CompletedTask;
        }
    }
}
