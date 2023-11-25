using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Timers;

namespace ArkStartServer
{
    internal class Program
    {
        private static System.Timers.Timer checkSteamTimer;
        private static string ArkServerLocation;
        static void Main(string[] args)
        {
            string asciiArt = Figgle.FiggleFonts.Standard.Render("ARK Server Loader");
            Console.WriteLine(asciiArt);

            var configuration = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
          .Build();

            // Read settings
            ArkServerLocation = configuration["ArkServerLocation"];

            // Set up the timer for 10000 milliseconds (10 seconds)
            checkSteamTimer = new System.Timers.Timer(10000);

            // Hook up the Elapsed event for the timer
            checkSteamTimer.Elapsed += OnTimedEvent;

            // Enable the timer
            checkSteamTimer.Enabled = true;

            Console.WriteLine("Monitoring for Steam. The application will exit when Steam is detected and ArkServer has been loaded.");

            // Keep the application running
            Console.ReadLine();
        }
        static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            // Check if Steam is running
            if (IsProcessRunning("Steam"))
            {
                Console.WriteLine("[" + DateTime.Now + "] Steam is running.");
                checkSteamTimer.Stop(); // Stop the timer

                // Start the executable
                Console.WriteLine("[" + DateTime.Now + "] Starting Server...");

                ProcessStartInfo startInfo = new ProcessStartInfo(ArkServerLocation);
                startInfo.UseShellExecute = true;//This should not block your program
                Process.Start(startInfo);

                Environment.Exit(0); // Exit the application
            }
            else
            {
                Console.WriteLine("[" + DateTime.Now + "] Steam is not running.");
            }
        }
        static bool IsProcessRunning(string processName)
        {
            // Get all processes with the specified name
            var processes = Process.GetProcessesByName(processName);
            return processes.Length > 0;
        }
    }
}
