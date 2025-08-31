using System.Text.Json.Serialization;

namespace ALARM.Indexer;

// Manifest Models
public class FileManifest
{
    public List<string> Roots { get; set; } = new() { "app-legacy" };
    public List<string> Ignore { get; set; } = new() { ".git", "bin", "obj", "packages", "node_modules" };
    public Dictionary<string, List<string>> FileRoles { get; set; } = new();
    public List<string> PriorityPatterns { get; set; } = new();
    public Dictionary<string, List<string>> ExternalDependencies { get; set; } = new();
}

public class ApiManifest
{
    public AutoCadConfig AutoCad { get; set; } = new();
    public OracleConfig Oracle { get; set; } = new();
    public DotNetConfig DotNet { get; set; } = new();
    public Dictionary<string, List<string>> ExternalDependencies { get; set; } = new();
}

public class AutoCadConfig
{
    public string Target { get; set; } = "";
    public string TargetVersion { get; set; } = "";
    public string Advice { get; set; } = "";
    public string ShimNamespace { get; set; } = "";
    public Dictionary<string, string> MigrationPatterns { get; set; } = new();
    public List<string> BreakingChanges { get; set; } = new();
}

public class OracleConfig
{
    public string Driver { get; set; } = "";
    public string TargetVersion { get; set; } = "";
    public string Advice { get; set; } = "";
    public string ShimNamespace { get; set; } = "";
    public Dictionary<string, string> MigrationPatterns { get; set; } = new();
    public Dictionary<string, string> ConnectionStringChanges { get; set; } = new();
}

public class DotNetConfig
{
    public string From { get; set; } = "";
    public string To { get; set; } = "";
    public string Advice { get; set; } = "";
    public Dictionary<string, string> MigrationPatterns { get; set; } = new();
    public Dictionary<string, string> ProjectFileChanges { get; set; } = new();
}

// Index Models
public class DiscoveredFiles
{
    public List<string> CSharpFiles { get; set; } = new();
    public List<string> CppFiles { get; set; } = new();
    public List<string> ConfigFiles { get; set; } = new();
    public List<string> ProjectFiles { get; set; } = new();
    public Dictionary<string, List<string>> FilesByRole { get; set; } = new();
}

public class CodeIndex
{
    public List<FileInfo> Files { get; set; } = new();
    public List<SymbolInfo> Symbols { get; set; } = new();
    public Dictionary<string, List<string>> Dependencies { get; set; } = new();
}

public class FileInfo
{
    public string Path { get; set; } = "";
    public string ProjectName { get; set; } = "";
    public string Language { get; set; } = "";
    public int LineCount { get; set; }
    public DateTime LastModified { get; set; }
    public List<string> Dependencies { get; set; } = new();
}

public class SymbolInfo
{
    public string Name { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Kind { get; set; } = ""; // Class, Method, Property, Field, etc.
    public string FilePath { get; set; } = "";
    public int LineNumber { get; set; }
    public string Accessibility { get; set; } = ""; // Public, Private, Internal, etc.
    public bool IsStatic { get; set; }
    public bool IsAbstract { get; set; }
    public string ReturnType { get; set; } = "";
    public List<string> Parameters { get; set; } = new();
    public List<string> References { get; set; } = new(); // Other symbols this references
    public List<string> ReferencedBy { get; set; } = new(); // Other symbols that reference this
    public string Namespace { get; set; } = "";
    public string ContainingType { get; set; } = "";
    public int CyclomaticComplexity { get; set; }
    public List<string> Attributes { get; set; } = new();
}

public class ExternalApiUsage
{
    public List<ApiUsage> AutoCadUsages { get; set; } = new();
    public List<ApiUsage> OracleUsages { get; set; } = new();
    public List<ApiUsage> FrameworkUsages { get; set; } = new();
}

public class ApiUsage
{
    public string FilePath { get; set; } = "";
    public string ApiName { get; set; } = "";
    public int LineNumber { get; set; }
    public string Context { get; set; } = ""; // Surrounding code context
    public string UsageType { get; set; } = ""; // Direct, Inheritance, Generic, etc.
    public string Severity { get; set; } = ""; // High, Medium, Low
    public string ReplacementSuggestion { get; set; } = "";
}

public class RiskAssessment
{
    public List<RiskItem> HighRiskItems { get; set; } = new();
    public List<RiskItem> MediumRiskItems { get; set; } = new();
    public List<RiskItem> LowRiskItems { get; set; } = new();
    public RiskSummary Summary { get; set; } = new();
}

public class RiskItem
{
    public string FilePath { get; set; } = "";
    public string RiskLevel { get; set; } = "";
    public string Description { get; set; } = "";
    public string Category { get; set; } = "";
    public string Impact { get; set; } = "";
    public string Mitigation { get; set; } = "";
    public string Owner { get; set; } = "";
    public DateTime? DueDate { get; set; }
    public int Priority { get; set; }
}

public class RiskSummary
{
    public int TotalRiskItems { get; set; }
    public int HighRiskCount { get; set; }
    public int MediumRiskCount { get; set; }
    public int LowRiskCount { get; set; }
    public double OverallRiskScore { get; set; }
    public List<string> CriticalAreas { get; set; } = new();
    public List<string> RecommendedActions { get; set; } = new();
}
