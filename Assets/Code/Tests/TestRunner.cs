using System;
using AGOE.Tests;

namespace AGOE
{
    /// <summary>
    /// Simple test runner for running tests without Unity.
    /// Once Unity is integrated, this will be replaced by Unity Test Framework.
    ///
    /// Usage: dotnet run --project TestRunner
    /// </summary>
    public class TestRunner
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("==================================");
            Console.WriteLine("AGOE Test Suite");
            Console.WriteLine("==================================\n");

            try
            {
                // Run all test suites
                CommandBusTests.RunAllTests();
                Console.WriteLine();
                SelectionSystemTests.RunAllTests();

                Console.WriteLine("\n==================================");
                Console.WriteLine("✓ ALL TESTS PASSED");
                Console.WriteLine("==================================");
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n==================================");
                Console.WriteLine($"✗ TEST FAILED: {ex.Message}");
                Console.WriteLine("==================================");
                Console.WriteLine(ex.StackTrace);
                Environment.Exit(1);
            }
        }
    }
}
