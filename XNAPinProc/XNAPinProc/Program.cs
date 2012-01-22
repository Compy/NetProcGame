using System;

namespace XNAPinProc
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (XNAPinProcGame game = new XNAPinProcGame())
            {
                game.Run();
            }
        }
    }
#endif
}

