using System;
using System.ServiceProcess;
using Microsoft.Owin.Hosting;

namespace Puzzle.API
{
    public class Program
    {
        internal static string BaseAddress { get; private set; }

        static void Main(string[] args)
        {
            BaseAddress = Configuration.ServicesURL;

            string arg0 = string.Empty;
            if (args.Length > 0)
                arg0 = (args[0] ?? string.Empty).ToLower();

            if (arg0 == "--process" || arg0 == "--console")
            {
                ConsoleProcess();
            }
            else
            {
                RunService();
            }
        }

        private static void RunService()
        {
            var ServicesToRun = new ServiceBase[]
            {
                new PuzzleServices()
            };
            ServiceBase.Run(ServicesToRun);
        }

        private static void ConsoleProcess()
        {
            // Start OWIN host
            using (WebApp.Start<Startup>(url: BaseAddress))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Running {0} Web Api Host on {1}.\nPress any key to exit", Configuration.AppName, BaseAddress);

                Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exiting...");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
