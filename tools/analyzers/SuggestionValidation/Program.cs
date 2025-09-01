using System;
using System.Threading.Tasks;

namespace ALARM.Analyzers.SuggestionValidation
{
    /// <summary>
    /// Entry point for the Suggestion Validation system
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("🚀 ALARM Suggestion Validation System");
            Console.WriteLine("====================================");
            Console.WriteLine();

            try
            {
                if (args.Length > 0 && args[0] == "--debug")
                {
                    await DebugFeatureExtractor.RunDebugTests();
                }
                else
                {
                    await SuggestionValidationTest.RunTestAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Program failed: {ex.Message}");
                Environment.Exit(1);
            }

            Console.WriteLine("\n✅ Program completed successfully!");
        }
    }
}
