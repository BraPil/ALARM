# ADDS25 Enhanced File Monitor Startup Script
# Purpose: Start the enhanced file-based communication system
# Environment: Dev Computer (kidsg) - Production-ready startup
# Date: September 3, 2025
# Version: 1.0 - Enhanced startup with configuration and monitoring

param(
    [string]$ConfigPath = ".\ci\config\file-monitor-config.json",
    [int]$MonitorInterval = 2,
    [switch]$EnablePerformanceMonitoring = $true,
    [switch]$EnableHealthChecks = $true,
    [switch]$Verbose = $false,
    [switch]$Background = $false,
    [switch]$InstallService = $false
)

# Script information
$script:ScriptInfo = @{
    Name = "ADDS25 Enhanced File Monitor"
    Version = "2.0"
    Purpose = "Advanced file-based communication system for Cursor automation"
    Author = "ALARM Development Team"
    Date = "2025-09-03"
}

# Display startup banner
function Show-StartupBanner {
    Write-Host ""
    Write-Host "╔══════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
    Write-Host "║                    ADDS25 Enhanced File Monitor             ║" -ForegroundColor Cyan
    Write-Host "║                        Version 2.0                          ║" -ForegroundColor Cyan
    Write-Host "║                                                              ║" -ForegroundColor Cyan
    Write-Host "║  Purpose: Advanced file-based communication system          ║" -ForegroundColor Yellow
    Write-Host "║  Features: JSON validation, performance monitoring,        ║" -ForegroundColor Yellow
    Write-Host "║            health checks, and comprehensive logging         ║" -ForegroundColor Yellow
    Write-Host "╚══════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
    Write-Host ""
}

# Validate system requirements
function Test-SystemRequirements {
    Write-Host "🔍 Validating system requirements..." -ForegroundColor Yellow
    
    $requirements = @{
        PowerShell = $PSVersionTable.PSVersion.Major -ge 5
        Repository = Test-Path "C:\Users\kidsg\Downloads\ALARM"
        ConfigDirectory = Test-Path ".\ci\config"
        LogDirectory = Test-Path ".\ci\logs"
    }
    
    $allMet = $true
    
    foreach ($req in $requirements.Keys) {
        if ($requirements[$req]) {
            Write-Host "  ✅ $req`: Available" -ForegroundColor Green
        } else {
            Write-Host "  ❌ $req`: Missing" -ForegroundColor Red
            $allMet = $false
        }
    }
    
    if (-not $allMet) {
        Write-Host ""
        Write-Host "❌ System requirements not met. Please resolve missing components." -ForegroundColor Red
        return $false
    }
    
    Write-Host "✅ All system requirements met" -ForegroundColor Green
    return $true
}

# Load and validate configuration
function Load-Configuration {
    Write-Host "📋 Loading configuration..." -ForegroundColor Yellow
    
    if (Test-Path $ConfigPath) {
        try {
            $config = Get-Content $ConfigPath -Raw | ConvertFrom-Json
            Write-Host "✅ Configuration loaded from: $ConfigPath" -ForegroundColor Green
            
            # Override with command line parameters
            if ($MonitorInterval -ne 2) {
                $config.MonitorInterval = $MonitorInterval
                Write-Host "  📝 Monitor interval overridden: $MonitorInterval seconds" -ForegroundColor Yellow
            }
            
            if (-not $EnablePerformanceMonitoring) {
                $config.EnablePerformanceMonitoring = $false
                Write-Host "  📝 Performance monitoring disabled" -ForegroundColor Yellow
            }
            
            if (-not $EnableHealthChecks) {
                $config.EnableHealthChecks = $false
                Write-Host "  📝 Health checks disabled" -ForegroundColor Yellow
            }
            
            return $config
            
        } catch {
            Write-Host "❌ Failed to load configuration: $($_.Exception.Message)" -ForegroundColor Red
            Write-Host "📝 Using default configuration" -ForegroundColor Yellow
            return Get-DefaultConfiguration
        }
    } else {
        Write-Host "⚠️ Configuration file not found: $ConfigPath" -ForegroundColor Yellow
        Write-Host "📝 Using default configuration" -ForegroundColor Yellow
        return Get-DefaultConfiguration
    }
}

# Get default configuration
function Get-DefaultConfiguration {
    return @{
        MonitorInterval = $MonitorInterval
        EnablePerformanceMonitoring = $EnablePerformanceMonitoring
        EnableHealthChecks = $EnableHealthChecks
        Verbose = $Verbose
        MaxRetries = 3
        RetryDelay = 5
        LogRetentionDays = 30
    }
}

# Create necessary directories
function Initialize-Directories {
    Write-Host "📁 Initializing directories..." -ForegroundColor Yellow
    
    $directories = @(
        ".\ci\logs\file-monitor",
        ".\ci\logs\file-monitor\performance",
        ".\ci\logs\file-monitor\health",
        ".\triggers",
        ".\triggers\archive"
    )
    
    foreach ($dir in $directories) {
        if (!(Test-Path $dir)) {
            New-Item -ItemType Directory -Path $dir -Force | Out-Null
            Write-Host "  ✅ Created: $dir" -ForegroundColor Green
        } else {
            Write-Host "  ℹ️  Exists: $dir" -ForegroundColor Gray
        }
    }
}

# Start file monitor
function Start-FileMonitor {
    param([object]$Config)
    
    Write-Host "🚀 Starting enhanced file monitor..." -ForegroundColor Green
    
    # Build command arguments
    $args = @(
        "-ConfigPath", $ConfigPath,
        "-MonitorInterval", $Config.MonitorInterval
    )
    
    if ($Config.EnablePerformanceMonitoring) {
        $args += "-EnablePerformanceMonitoring"
    }
    
    if ($Config.EnableHealthChecks) {
        $args += "-EnableHealthChecks"
    }
    
    if ($Config.Verbose -or $Verbose) {
        $args += "-Verbose"
    }
    
    # Start the file monitor
    if ($Background) {
        Write-Host "📝 Starting in background mode..." -ForegroundColor Yellow
        
        $job = Start-Job -ScriptBlock {
            param($Args, $MonitorPath)
            Set-Location $MonitorPath
            & ".\ci\CURSOR-FILE-MONITOR-ENHANCED.ps1" @Args
        } -ArgumentList $args, (Get-Location).Path
        
        Write-Host "✅ File monitor started in background (Job ID: $($job.Id))" -ForegroundColor Green
        Write-Host "📝 Use 'Get-Job' to view background jobs" -ForegroundColor Yellow
        Write-Host "📝 Use 'Receive-Job $($job.Id)' to view output" -ForegroundColor Yellow
        Write-Host "📝 Use 'Stop-Job $($job.Id)' to stop the monitor" -ForegroundColor Yellow
        
        return $job
        
    } else {
        Write-Host "📝 Starting in foreground mode..." -ForegroundColor Yellow
        Write-Host "📝 Press Ctrl+C to stop the monitor" -ForegroundColor Yellow
        Write-Host ""
        
        # Start the file monitor in foreground
        & ".\ci\CURSOR-FILE-MONITOR-ENHANCED.ps1" @args
    }
}

# Install as Windows service (optional)
function Install-WindowsService {
    Write-Host "🔧 Installing as Windows service..." -ForegroundColor Yellow
    
    try {
        # Check if running as administrator
        if (-not ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
            Write-Host "❌ Administrator privileges required to install service" -ForegroundColor Red
            return $false
        }
        
        # Service installation logic would go here
        Write-Host "⚠️ Service installation not yet implemented" -ForegroundColor Yellow
        Write-Host "📝 Please run the monitor manually for now" -ForegroundColor Yellow
        
        return $false
        
    } catch {
        Write-Host "❌ Failed to install service: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Display usage information
function Show-Usage {
    Write-Host "📖 USAGE:" -ForegroundColor Green
    Write-Host "  .\START-ENHANCED-FILE-MONITOR.ps1 [OPTIONS]" -ForegroundColor White
    Write-Host ""
    Write-Host "OPTIONS:" -ForegroundColor Green
    Write-Host "  -ConfigPath <path>              Configuration file path" -ForegroundColor White
    Write-Host "  -MonitorInterval <seconds>      Monitoring interval (default: 2)" -ForegroundColor White
    Write-Host "  -EnablePerformanceMonitoring    Enable performance monitoring" -ForegroundColor White
    Write-Host "  -EnableHealthChecks             Enable health monitoring" -ForegroundColor White
    Write-Host "  -Verbose                        Enable verbose output" -ForegroundColor White
    Write-Host "  -Background                     Run in background mode" -ForegroundColor White
    Write-Host "  -InstallService                 Install as Windows service" -ForegroundColor White
    Write-Host ""
    Write-Host "EXAMPLES:" -ForegroundColor Green
    Write-Host "  # Start with default settings" -ForegroundColor White
    Write-Host "  .\START-ENHANCED-FILE-MONITOR.ps1" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  # Start with custom interval" -ForegroundColor White
    Write-Host "  .\START-ENHANCED-FILE-MONITOR.ps1 -MonitorInterval 5" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  # Start in background mode" -ForegroundColor White
    Write-Host "  .\START-ENHANCED-FILE-MONITOR.ps1 -Background" -ForegroundColor Gray
    Write-Host ""
}

# Main execution
function Main {
    # Show startup banner
    Show-StartupBanner
    
    # Show usage if help requested
    if ($PSBoundParameters.ContainsKey("Help") -or $PSBoundParameters.ContainsKey("?")) {
        Show-Usage
        return
    }
    
    Write-Host "🚀 Starting ADDS25 Enhanced File Monitor..." -ForegroundColor Green
    Write-Host "📅 Date: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor Gray
    Write-Host "💻 Computer: $env:COMPUTERNAME" -ForegroundColor Gray
    Write-Host "👤 User: $env:USERNAME" -ForegroundColor Gray
    Write-Host ""
    
    # Validate system requirements
    if (-not (Test-SystemRequirements)) {
        Write-Host "❌ System requirements validation failed" -ForegroundColor Red
        exit 1
    }
    
    # Load configuration
    $config = Load-Configuration
    
    # Initialize directories
    Initialize-Directories
    
    # Install service if requested
    if ($InstallService) {
        if (Install-WindowsService) {
            Write-Host "✅ Service installed successfully" -ForegroundColor Green
            return
        } else {
            Write-Host "⚠️ Service installation failed, continuing with manual start" -ForegroundColor Yellow
        }
    }
    
    # Start file monitor
    try {
        $result = Start-FileMonitor -Config $config
        
        if ($Background -and $result) {
            Write-Host ""
            Write-Host "🎉 Enhanced file monitor started successfully in background!" -ForegroundColor Green
            Write-Host "📝 Monitor is now watching for trigger files" -ForegroundColor Cyan
            Write-Host "📝 Use the trigger generator to create test triggers" -ForegroundColor Cyan
        }
        
    } catch {
        Write-Host ""
        Write-Host "❌ Failed to start file monitor: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host "📝 Check the configuration and try again" -ForegroundColor Yellow
        exit 1
    }
}

# Execute main function
try {
    Main
} catch {
    Write-Host ""
    Write-Host "❌ Critical error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "📝 Stack trace: $($_.ScriptStackTrace)" -ForegroundColor Red
    exit 1
}
