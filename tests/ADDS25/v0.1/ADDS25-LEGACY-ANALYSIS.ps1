# ADDS 2025 Legacy Code Analysis Script
# Purpose: Comprehensive analysis of ADDS 2019 source code for ADDS25 migration
# Environment: Dev Computer (kidsg) - Tests Named Pipes system
# Date: September 3, 2025

Write-Host "*** ADDS 2025 LEGACY CODE ANALYSIS ***" -ForegroundColor Cyan
Write-Host "Purpose: Reverse engineer ADDS 2019 for ADDS25 migration" -ForegroundColor Yellow
Write-Host "Architecture: Comprehensive code analysis with Named Pipes integration" -ForegroundColor Green
Write-Host ""

# Configuration
$repoPath = "C:\Users\kidsg\Downloads\ALARM"
$adds25Path = "$repoPath\tests\ADDS25\v0.1"
$analysisResultsPath = "$repoPath\test-results"
$timestamp = Get-Date -Format "yyyy-MM-dd-HHmm"

function Write-AnalysisLog {
    param([string]$Message, [string]$Level = "INFO")
    $logTimestamp = Get-Date -Format "HH:mm:ss"
    $color = switch($Level) { "ERROR" { "Red" } "SUCCESS" { "Green" } "WARNING" { "Yellow" } default { "White" } }
    Write-Host "[$logTimestamp] [$Level] $Message" -ForegroundColor $color
}

function Test-NamedPipesConnection {
    Write-AnalysisLog "Testing Named Pipes connection..." "INFO"
    
    try {
        # Test if Named Pipes server is running
        $pipeName = "ADDS25-CURSOR-ANALYSIS-PIPE"
        $testPipe = New-Object System.IO.Pipes.NamedPipeClientStream(".", $pipeName, [System.IO.Pipes.PipeDirection]::Out)
        $testPipe.Connect(1000) # 1 second timeout
        
        $writer = New-Object System.IO.StreamWriter($testPipe)
        $testMessage = "ANALYZE:test:$timestamp:ADDS25 Legacy Analysis Test"
        $writer.Write($testMessage)
        $writer.Flush()
        
        $writer.Close()
        $testPipe.Close()
        
        Write-AnalysisLog "Named Pipes connection test successful!" "SUCCESS"
        return $true
    } catch {
        Write-AnalysisLog "Named Pipes connection test failed: $($_.Exception.Message)" "WARNING"
        return $false
    }
}

function Analyze-ADDS25ProjectStructure {
    Write-AnalysisLog "Analyzing ADDS25 project structure..." "INFO"
    
    $projectStructure = @{
        "Core" = @{
            "Path" = "$adds25Path\ADDS25.Core"
            "Files" = @()
            "Dependencies" = @()
        }
        "AutoCAD" = @{
            "Path" = "$adds25Path\ADDS25.AutoCAD"
            "Files" = @()
            "Dependencies" = @()
        }
        "Oracle" = @{
            "Path" = "$adds25Path\ADDS25.Oracle"
            "Files" = @()
            "Dependencies" = @()
        }
    }
    
    foreach ($component in $projectStructure.Keys) {
        $componentPath = $projectStructure[$component].Path
        if (Test-Path $componentPath) {
            $files = Get-ChildItem $componentPath -Recurse -File | Where-Object { $_.Extension -match "\.(cs|csproj|xml|json|ps1|bat)$" }
            $projectStructure[$component].Files = $files | ForEach-Object { $_.Name }
            
            Write-AnalysisLog "Component $component`: $($files.Count) files found" "SUCCESS"
        } else {
            Write-AnalysisLog "Component $component path not found: $componentPath" "WARNING"
        }
    }
    
    return $projectStructure
}

function Analyze-CoreComponents {
    Write-AnalysisLog "Analyzing ADDS25.Core components..." "INFO"
    
    $corePath = "$adds25Path\ADDS25.Core"
    $coreAnalysis = @{
        "LispIntegrationBridge" = @{
            "Status" = "Unknown"
            "Lines" = 0
            "Dependencies" = @()
        }
        "SCS" = @{
            "Status" = "Unknown"
            "Lines" = 0
            "Dependencies" = @()
        }
    }
    
    # Analyze LispIntegrationBridge.cs
    $lispFile = "$corePath\LispIntegrationBridge.cs"
    if (Test-Path $lispFile) {
        $content = Get-Content $lispFile
        $coreAnalysis.LispIntegrationBridge.Lines = $content.Count
        $coreAnalysis.LispIntegrationBridge.Status = "Found"
        
        # Extract dependencies
        $dependencies = $content | Where-Object { $_ -match "using\s+\w+" } | ForEach-Object { ($_ -split "\s+")[1] }
        $coreAnalysis.LispIntegrationBridge.Dependencies = $dependencies | Select-Object -Unique
        
        Write-AnalysisLog "LispIntegrationBridge.cs: $($content.Count) lines, $($dependencies.Count) dependencies" "SUCCESS"
    } else {
        Write-AnalysisLog "LispIntegrationBridge.cs not found" "WARNING"
    }
    
    # Analyze SCS.cs
    $scsFile = "$corePath\SCS.cs"
    if (Test-Path $scsFile) {
        $content = Get-Content $scsFile
        $coreAnalysis.SCS.Lines = $content.Count
        $coreAnalysis.SCS.Status = "Found"
        
        # Extract dependencies
        $dependencies = $content | Where-Object { $_ -match "using\s+\w+" } | ForEach-Object { ($_ -split "\s+")[1] }
        $coreAnalysis.SCS.Dependencies = $dependencies | Select-Object -Unique
        
        Write-AnalysisLog "SCS.cs: $($content.Count) lines, $($dependencies.Count) dependencies" "SUCCESS"
    } else {
        Write-AnalysisLog "SCS.cs not found" "WARNING"
    }
    
    return $coreAnalysis
}

function Analyze-AutoCADIntegration {
    Write-AnalysisLog "Analyzing ADDS25.AutoCAD integration..." "INFO"
    
    $autoCADPath = "$adds25Path\ADDS25.AutoCAD"
    $autoCADAnalysis = @{
        "AutoCADIntegration" = @{
            "Status" = "Unknown"
            "Lines" = 0
            "Dependencies" = @()
        }
        "ProjectFile" = @{
            "Status" = "Unknown"
            "TargetFramework" = ""
            "References" = @()
        }
    }
    
    # Analyze AutoCADIntegration.cs
    $integrationFile = "$autoCADPath\AutoCADIntegration.cs"
    if (Test-Path $integrationFile) {
        $content = Get-Content $integrationFile
        $autoCADAnalysis.AutoCADIntegration.Lines = $content.Count
        $autoCADAnalysis.AutoCADIntegration.Status = "Found"
        
        # Extract dependencies
        $dependencies = $content | Where-Object { $_ -match "using\s+\w+" } | ForEach-Object { ($_ -split "\s+")[1] }
        $autoCADAnalysis.AutoCADIntegration.Dependencies = $dependencies | Select-Object -Unique
        
        Write-AnalysisLog "AutoCADIntegration.cs: $($content.Count) lines, $($dependencies.Count) dependencies" "SUCCESS"
    } else {
        Write-AnalysisLog "AutoCADIntegration.cs not found" "WARNING"
    }
    
    # Analyze project file
    $projectFile = "$autoCADPath\ADDS25.AutoCAD.csproj"
    if (Test-Path $projectFile) {
        $content = Get-Content $projectFile
        $autoCADAnalysis.ProjectFile.Status = "Found"
        
        # Extract target framework
        $targetFramework = $content | Where-Object { $_ -match "<TargetFramework>" } | ForEach-Object { ($_ -replace "<TargetFramework>", "" -replace "</TargetFramework>", "").Trim() }
        $autoCADAnalysis.ProjectFile.TargetFramework = $targetFramework
        
        # Extract references
        $references = $content | Where-Object { $_ -match "<Reference\s+Include=" } | ForEach-Object { ($_ -replace '<Reference\s+Include="', "" -replace '".*', "").Trim() }
        $autoCADAnalysis.ProjectFile.References = $references
        
        Write-AnalysisLog "ADDS25.AutoCAD.csproj: TargetFramework=$targetFramework, $($references.Count) references" "SUCCESS"
    } else {
        Write-AnalysisLog "ADDS25.AutoCAD.csproj not found" "WARNING"
    }
    
    return $autoCADAnalysis
}

function Analyze-OracleIntegration {
    Write-AnalysisLog "Analyzing ADDS25.Oracle integration..." "INFO"
    
    $oraclePath = "$adds25Path\ADDS25.Oracle"
    $oracleAnalysis = @{
        "ProjectFile" = @{
            "Status" = "Unknown"
            "TargetFramework" = ""
            "References" = @()
        }
    }
    
    # Analyze project file
    $projectFile = "$oraclePath\ADDS25.Oracle.csproj"
    if (Test-Path $projectFile) {
        $content = Get-Content $projectFile
        $oracleAnalysis.ProjectFile.Status = "Found"
        
        # Extract target framework
        $targetFramework = $content | Where-Object { $_ -match "<TargetFramework>" } | ForEach-Object { ($_ -replace "<TargetFramework>", "" -replace "</TargetFramework>", "").Trim() }
        $oracleAnalysis.ProjectFile.TargetFramework = $targetFramework
        
        # Extract references
        $references = $content | Where-Object { $_ -match "<Reference\s+Include=" } | ForEach-Object { ($_ -replace '<Reference\s+Include="', "" -replace '".*', "").Trim() }
        $oracleAnalysis.ProjectFile.References = $references
        
        Write-AnalysisLog "ADDS25.Oracle.csproj: TargetFramework=$targetFramework, $($references.Count) references" "SUCCESS"
    } else {
        Write-AnalysisLog "ADDS25.Oracle.csproj not found" "WARNING"
    }
    
    return $oracleAnalysis
}

function Generate-AnalysisReport {
    param($projectStructure, $coreAnalysis, $autoCADAnalysis, $oracleAnalysis)
    
    Write-AnalysisLog "Generating comprehensive analysis report..." "INFO"
    
    $reportPath = "$analysisResultsPath\adds25-legacy-analysis-$timestamp.md"
    
    $report = @"
# ADDS 2025 Legacy Code Analysis Report
Generated: $timestamp

## Executive Summary
Comprehensive analysis of ADDS 2019 source code structure for ADDS25 migration project.

## Project Structure Analysis

### Core Components
- **ADDS25.Core**: $(if($projectStructure.Core.Files.Count -gt 0) { "$($projectStructure.Core.Files.Count) files" } else { "No files found" })
- **ADDS25.AutoCAD**: $(if($projectStructure.AutoCAD.Files.Count -gt 0) { "$($projectStructure.AutoCAD.Files.Count) files" } else { "No files found" })
- **ADDS25.Oracle**: $(if($projectStructure.Oracle.Files.Count -gt 0) { "$($projectStructure.Oracle.Files.Count) files" } else { "No files found" })

## Detailed Component Analysis

### ADDS25.Core
$(foreach($component in $coreAnalysis.Keys) {
"- **$component**: $($coreAnalysis[$component].Status) - $($coreAnalysis[$component].Lines) lines"
if($coreAnalysis[$component].Dependencies.Count -gt 0) {
"  - Dependencies: $($coreAnalysis[$component].Dependencies -join ', ')"
}
})

### ADDS25.AutoCAD
$(foreach($component in $autoCADAnalysis.Keys) {
"- **$component**: $($autoCADAnalysis[$component].Status)"
if($component -eq "ProjectFile" -and $autoCADAnalysis[$component].TargetFramework) {
"  - Target Framework: $($autoCADAnalysis[$component].TargetFramework)"
}
if($autoCADAnalysis[$component].References.Count -gt 0) {
"  - References: $($autoCADAnalysis[$component].References -join ', ')"
}
})

### ADDS25.Oracle
$(foreach($component in $oracleAnalysis.Keys) {
"- **$component**: $($oracleAnalysis[$component].Status)"
if($component -eq "ProjectFile" -and $oracleAnalysis[$component].TargetFramework) {
"  - Target Framework: $($oracleAnalysis[$component].TargetFramework)"
}
if($oracleAnalysis[$component].References.Count -gt 0) {
"  - References: $($oracleAnalysis[$component].References -join ', ')"
}
})

## Migration Readiness Assessment
- **Code Structure**: $(if($projectStructure.Core.Files.Count -gt 0 -and $projectStructure.AutoCAD.Files.Count -gt 0 -and $projectStructure.Oracle.Files.Count -gt 0) { "Ready" } else { "Needs Review" })
- **Dependencies**: $(if($coreAnalysis.LispIntegrationBridge.Dependencies.Count -gt 0 -or $coreAnalysis.SCS.Dependencies.Count -gt 0) { "Identified" } else { "Unknown" })
- **Framework Compatibility**: $(if($autoCADAnalysis.ProjectFile.TargetFramework -or $oracleAnalysis.ProjectFile.TargetFramework) { "Configured" } else { "Needs Configuration" })

## Next Steps
1. Review identified dependencies
2. Validate framework compatibility
3. Begin component-by-component migration
4. Implement comprehensive testing

## Analysis Metadata
- **Analysis Date**: $timestamp
- **Repository Path**: $repoPath
- **ADDS25 Path**: $adds25Path
- **Named Pipes Test**: $(if(Test-NamedPipesConnection) { "SUCCESS" } else { "FAILED" })
"@

    try {
        $report | Out-File -FilePath $reportPath -Encoding UTF8
        Write-AnalysisLog "Analysis report generated: $reportPath" "SUCCESS"
        return $reportPath
    } catch {
        Write-AnalysisLog "Failed to generate report: $($_.Exception.Message)" "ERROR"
        return $null
    }
}

# Main execution
Write-AnalysisLog "Starting ADDS 2025 Legacy Code Analysis..." "INFO"
Write-AnalysisLog "Repository Path: $repoPath" "INFO"
Write-AnalysisLog "ADDS25 Path: $adds25Path" "INFO"
Write-Host ""

# Test Named Pipes connection
$namedPipesTest = Test-NamedPipesConnection
Write-Host ""

# Analyze project structure
$projectStructure = Analyze-ADDS25ProjectStructure
Write-Host ""

# Analyze core components
$coreAnalysis = Analyze-CoreComponents
Write-Host ""

# Analyze AutoCAD integration
$autoCADAnalysis = Analyze-AutoCADIntegration
Write-Host ""

# Analyze Oracle integration
$oracleAnalysis = Analyze-OracleIntegration
Write-Host ""

# Generate comprehensive report
$reportPath = Generate-AnalysisReport -projectStructure $projectStructure -coreAnalysis $coreAnalysis -autoCADAnalysis $autoCADAnalysis -oracleAnalysis $oracleAnalysis

Write-Host ""
Write-Host "*** ADDS 2025 LEGACY ANALYSIS COMPLETE ***" -ForegroundColor Green
Write-Host "Report Generated: $reportPath" -ForegroundColor Cyan
Write-Host "Named Pipes Test: $(if($namedPipesTest) { "SUCCESS" } else { "FAILED" })" -ForegroundColor $(if($namedPipesTest) { "Green" } else { "Red" })
Write-Host ""

if ($namedPipesTest) {
    Write-Host "🎉 NAMED PIPES SYSTEM TEST SUCCESSFUL!" -ForegroundColor Green
    Write-Host "Real-time communication is operational" -ForegroundColor White
    Write-Host "Ready for GitHub push detection and analysis" -ForegroundColor White
} else {
    Write-Host "⚠️ NAMED PIPES SYSTEM TEST FAILED" -ForegroundColor Yellow
    Write-Host "Check if Named Pipes server is running" -ForegroundColor White
    Write-Host "Run: .\ci\CURSOR-NAMED-PIPE-SERVER-FIXED.ps1" -ForegroundColor Cyan
}

Write-Host ""
Write-Host "Press any key to continue..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
