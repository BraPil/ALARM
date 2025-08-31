using ALARM.DomainLibraries;
using Microsoft.Extensions.Logging;

Console.WriteLine("🔧 Simple Integration Test");
Console.WriteLine("==========================");

using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = loggerFactory.CreateLogger<UnifiedDomainLibraries>();

try
{
    var unified = new UnifiedDomainLibraries(logger);
    var domains = unified.GetAvailableDomains().ToList();
    Console.WriteLine($"✅ SUCCESS: Found {domains.Count} domains");
    foreach (var domain in domains)
    {
        Console.WriteLine($"  - {domain}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ FAILED: {ex.Message}");
    Environment.Exit(1);
}