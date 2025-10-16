using System;
using AGOE.Demo;

namespace AGOE
{
    /// <summary>
    /// Entry point for the interactive demo.
    /// Run this to see the game systems in action (console mode).
    /// </summary>
    public class DemoRunner
    {
        public static void Main(string[] args)
        {
            try
            {
                var demo = new GameDemo();
                demo.Run();
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ Demo failed: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Environment.Exit(1);
            }
        }
    }
}
