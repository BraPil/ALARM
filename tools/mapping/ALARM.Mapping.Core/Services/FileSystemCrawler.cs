using ALARM.Mapping.Core.Interfaces;
using ALARM.Mapping.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ALARM.Mapping.Core.Services
{
    /// <summary>
    /// File system crawler implementation for comprehensive directory analysis
    /// </summary>
    public class FileSystemCrawler : IFileSystemCrawler
    {
        private readonly ILogger<FileSystemCrawler> _logger;
        private static readonly Dictionary<string, FileType> FileTypeMapping = new()
        {
            // Source code files
            [".cs"] = FileType.SourceCode,
            [".vb"] = FileType.SourceCode,
            [".cpp"] = FileType.SourceCode,
            [".c"] = FileType.SourceCode,
            [".h"] = FileType.SourceCode,
            [".hpp"] = FileType.SourceCode,
            [".java"] = FileType.SourceCode,
            [".js"] = FileType.SourceCode,
            [".ts"] = FileType.SourceCode,
            [".py"] = FileType.SourceCode,
            [".sql"] = FileType.SourceCode,
            [".ps1"] = FileType.SourceCode,
            [".psm1"] = FileType.SourceCode,
            [".bat"] = FileType.SourceCode,
            [".cmd"] = FileType.SourceCode,

            // Configuration files
            [".config"] = FileType.Configuration,
            [".xml"] = FileType.Configuration,
            [".json"] = FileType.Configuration,
            [".yaml"] = FileType.Configuration,
            [".yml"] = FileType.Configuration,
            [".ini"] = FileType.Configuration,
            [".properties"] = FileType.Configuration,
            [".settings"] = FileType.Configuration,
            [".resx"] = FileType.Configuration,

            // Resource files
            [".resx"] = FileType.Resource,
            [".resources"] = FileType.Resource,
            [".ico"] = FileType.Resource,
            [".png"] = FileType.Resource,
            [".jpg"] = FileType.Resource,
            [".jpeg"] = FileType.Resource,
            [".gif"] = FileType.Resource,
            [".bmp"] = FileType.Resource,
            [".svg"] = FileType.Resource,

            // Documentation files
            [".md"] = FileType.Documentation,
            [".txt"] = FileType.Documentation,
            [".rtf"] = FileType.Documentation,
            [".doc"] = FileType.Documentation,
            [".docx"] = FileType.Documentation,
            [".pdf"] = FileType.Documentation,
            [".html"] = FileType.Documentation,
            [".htm"] = FileType.Documentation,

            // Binary files
            [".dll"] = FileType.Binary,
            [".exe"] = FileType.Binary,
            [".pdb"] = FileType.Binary,
            [".lib"] = FileType.Binary,
            [".obj"] = FileType.Binary,

            // Archive files
            [".zip"] = FileType.Archive,
            [".rar"] = FileType.Archive,
            [".7z"] = FileType.Archive,
            [".tar"] = FileType.Archive,
            [".gz"] = FileType.Archive
        };

        public FileSystemCrawler(ILogger<FileSystemCrawler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Crawls the file system starting from root path
        /// </summary>
        public async Task<FileSystemAnalysis> CrawlAsync(
            string rootPath, 
            CrawlOptions options, 
            CancellationToken cancellationToken = default)
        {
            return await CrawlAsync(rootPath, options, null, cancellationToken);
        }

        /// <summary>
        /// Crawls the file system with progress reporting
        /// </summary>
        public async Task<FileSystemAnalysis> CrawlAsync(
            string rootPath, 
            CrawlOptions options, 
            IProgress<CrawlProgress>? progress, 
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(rootPath))
                throw new ArgumentException("Root path cannot be null or empty", nameof(rootPath));
            
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (!Directory.Exists(rootPath))
                throw new DirectoryNotFoundException($"Root path does not exist: {rootPath}");

            _logger.LogInformation("Starting file system crawl from {RootPath}", rootPath);

            var analysis = new FileSystemAnalysis();
            var allFiles = new List<Models.FileInfo>();
            var crawlProgress = new CrawlProgress();

            try
            {
                // Build directory structure and collect files
                analysis.RootStructure = await BuildDirectoryStructureAsync(
                    rootPath, 
                    rootPath, 
                    options, 
                    allFiles, 
                    crawlProgress, 
                    progress, 
                    0, 
                    cancellationToken);

                // Categorize files
                await CategorizeFilesAsync(allFiles, analysis, cancellationToken);

                // Calculate statistics
                analysis.TotalFiles = allFiles.Count;
                analysis.TotalDirectories = CountDirectories(analysis.RootStructure);
                analysis.TotalSizeBytes = allFiles.Sum(f => f.SizeBytes);
                analysis.FileTypeDistribution = CalculateFileTypeDistribution(allFiles);

                _logger.LogInformation("File system crawl complete. Found {TotalFiles} files in {TotalDirectories} directories, total size: {TotalSize:N0} bytes",
                    analysis.TotalFiles, analysis.TotalDirectories, analysis.TotalSizeBytes);

                return analysis;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "File system crawl failed for {RootPath}", rootPath);
                throw;
            }
        }

        /// <summary>
        /// Enumerates files asynchronously for streaming processing
        /// </summary>
        public async IAsyncEnumerable<Models.FileInfo> EnumerateFilesAsync(
            string rootPath, 
            CrawlOptions options, 
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(rootPath))
                throw new ArgumentException("Root path cannot be null or empty", nameof(rootPath));
            
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (!Directory.Exists(rootPath))
                throw new DirectoryNotFoundException($"Root path does not exist: {rootPath}");

            await foreach (var fileInfo in EnumerateFilesRecursiveAsync(rootPath, rootPath, options, 0, cancellationToken))
            {
                yield return fileInfo;
            }
        }

        #region Private Methods

        private async Task<DirectoryStructure> BuildDirectoryStructureAsync(
            string rootPath,
            string currentPath,
            CrawlOptions options,
            List<Models.FileInfo> allFiles,
            CrawlProgress progress,
            IProgress<CrawlProgress>? progressReporter,
            int currentDepth,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (currentDepth > options.MaxDepth)
            {
                _logger.LogWarning("Maximum depth {MaxDepth} exceeded at {CurrentPath}", options.MaxDepth, currentPath);
                return new DirectoryStructure
                {
                    Name = Path.GetFileName(currentPath),
                    FullPath = currentPath,
                    RelativePath = Path.GetRelativePath(rootPath, currentPath)
                };
            }

            var directory = new DirectoryStructure
            {
                Name = Path.GetFileName(currentPath),
                FullPath = currentPath,
                RelativePath = Path.GetRelativePath(rootPath, currentPath)
            };

            try
            {
                // Process files in current directory
                var files = Directory.GetFiles(currentPath);
                foreach (var filePath in files)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (ShouldExcludeFile(filePath, rootPath, options))
                        continue;

                    var fileInfo = await CreateFileInfoAsync(filePath, rootPath, options, cancellationToken);
                    if (fileInfo != null)
                    {
                        directory.Files.Add(fileInfo);
                        allFiles.Add(fileInfo);

                        progress.FilesProcessed++;
                        progress.BytesProcessed += fileInfo.SizeBytes;
                        progress.CurrentPath = filePath;
                        progressReporter?.Report(progress);
                    }
                }

                // Process subdirectories
                var subdirectories = Directory.GetDirectories(currentPath);
                foreach (var subdirectoryPath in subdirectories)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (ShouldExcludeDirectory(subdirectoryPath, rootPath, options))
                        continue;

                    if (!options.FollowSymlinks && IsSymbolicLink(subdirectoryPath))
                        continue;

                    var subdirectory = await BuildDirectoryStructureAsync(
                        rootPath,
                        subdirectoryPath,
                        options,
                        allFiles,
                        progress,
                        progressReporter,
                        currentDepth + 1,
                        cancellationToken);

                    directory.Subdirectories.Add(subdirectory);
                    progress.DirectoriesProcessed++;
                }

                // Calculate totals
                directory.TotalFiles = directory.Files.Count + directory.Subdirectories.Sum(s => s.TotalFiles);
                directory.TotalDirectories = directory.Subdirectories.Count + directory.Subdirectories.Sum(s => s.TotalDirectories);
                directory.TotalSizeBytes = directory.Files.Sum(f => f.SizeBytes) + directory.Subdirectories.Sum(s => s.TotalSizeBytes);

                return directory;
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Access denied to directory {DirectoryPath}: {Message}", currentPath, ex.Message);
                return directory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing directory {DirectoryPath}", currentPath);
                throw;
            }
        }

        private async IAsyncEnumerable<Models.FileInfo> EnumerateFilesRecursiveAsync(
            string rootPath,
            string currentPath,
            CrawlOptions options,
            int currentDepth,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (currentDepth > options.MaxDepth)
                yield break;

            // Process files in current directory
            string[] files;
            try
            {
                files = Directory.GetFiles(currentPath);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Access denied to directory {DirectoryPath}", currentPath);
                yield break;
            }

            foreach (var filePath in files)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (ShouldExcludeFile(filePath, rootPath, options))
                    continue;

                var fileInfo = await CreateFileInfoAsync(filePath, rootPath, options, cancellationToken);
                if (fileInfo != null)
                {
                    yield return fileInfo;
                }
            }

            // Process subdirectories
            string[] subdirectories;
            try
            {
                subdirectories = Directory.GetDirectories(currentPath);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Access denied to directory {DirectoryPath}", currentPath);
                yield break;
            }

            foreach (var subdirectoryPath in subdirectories)
            {
                if (ShouldExcludeDirectory(subdirectoryPath, rootPath, options))
                    continue;

                if (!options.FollowSymlinks && IsSymbolicLink(subdirectoryPath))
                    continue;

                await foreach (var fileInfo in EnumerateFilesRecursiveAsync(rootPath, subdirectoryPath, options, currentDepth + 1, cancellationToken))
                {
                    yield return fileInfo;
                }
            }
        }

        private async Task<Models.FileInfo?> CreateFileInfoAsync(
            string filePath, 
            string rootPath, 
            CrawlOptions options, 
            CancellationToken cancellationToken)
        {
            try
            {
                var systemFileInfo = new System.IO.FileInfo(filePath);
                
                if (systemFileInfo.Length > options.MaxFileSize)
                {
                    _logger.LogWarning("File {FilePath} exceeds maximum size {MaxSize:N0} bytes", filePath, options.MaxFileSize);
                    return null;
                }

                var fileInfo = new Models.FileInfo
                {
                    FullPath = systemFileInfo.FullName,
                    RelativePath = Path.GetRelativePath(rootPath, systemFileInfo.FullName),
                    FileName = systemFileInfo.Name,
                    Extension = systemFileInfo.Extension.ToLowerInvariant(),
                    SizeBytes = systemFileInfo.Length,
                    CreatedUtc = systemFileInfo.CreationTimeUtc,
                    ModifiedUtc = systemFileInfo.LastWriteTimeUtc,
                    Type = DetermineFileType(systemFileInfo.Extension)
                };

                // Extract additional metadata if requested
                if (options.ExtractMetadata)
                {
                    await ExtractMetadataAsync(fileInfo, systemFileInfo, cancellationToken);
                }

                // Calculate hash if requested
                if (options.CalculateHashes)
                {
                    fileInfo.Hash = await CalculateFileHashAsync(systemFileInfo.FullName, cancellationToken);
                }

                return fileInfo;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to process file {FilePath}", filePath);
                return null;
            }
        }

        private async Task ExtractMetadataAsync(Models.FileInfo fileInfo, System.IO.FileInfo systemFileInfo, CancellationToken cancellationToken)
        {
            try
            {
                // Detect encoding for text files
                if (IsTextFile(fileInfo.Extension))
                {
                    fileInfo.Encoding = await DetectEncodingAsync(systemFileInfo.FullName, cancellationToken);
                    fileInfo.LineCount = await CountLinesAsync(systemFileInfo.FullName, cancellationToken);
                }

                // Add custom metadata based on file type
                fileInfo.Metadata["IsReadOnly"] = systemFileInfo.IsReadOnly;
                fileInfo.Metadata["Attributes"] = systemFileInfo.Attributes.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract metadata for {FilePath}", fileInfo.FullPath);
            }
        }

        private async Task<string> CalculateFileHashAsync(string filePath, CancellationToken cancellationToken)
        {
            try
            {
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
                using var sha256 = SHA256.Create();
                var hashBytes = await Task.Run(() => sha256.ComputeHash(stream), cancellationToken);
                return Convert.ToHexString(hashBytes);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to calculate hash for {FilePath}", filePath);
                return string.Empty;
            }
        }

        private async Task<string> DetectEncodingAsync(string filePath, CancellationToken cancellationToken)
        {
            try
            {
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var buffer = new byte[1024];
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

                // Simple BOM detection
                if (bytesRead >= 3 && buffer[0] == 0xEF && buffer[1] == 0xBB && buffer[2] == 0xBF)
                    return "UTF-8";
                if (bytesRead >= 2 && buffer[0] == 0xFF && buffer[1] == 0xFE)
                    return "UTF-16LE";
                if (bytesRead >= 2 && buffer[0] == 0xFE && buffer[1] == 0xFF)
                    return "UTF-16BE";

                // Default assumption for text files
                return "UTF-8";
            }
            catch
            {
                return "Unknown";
            }
        }

        private async Task<int> CountLinesAsync(string filePath, CancellationToken cancellationToken)
        {
            try
            {
                var lines = await File.ReadAllLinesAsync(filePath, cancellationToken);
                return lines.Length;
            }
            catch
            {
                return 0;
            }
        }

        private bool ShouldExcludeFile(string filePath, string rootPath, CrawlOptions options)
        {
            var relativePath = Path.GetRelativePath(rootPath, filePath);
            
            // Check exclude patterns
            foreach (var pattern in options.ExcludePatterns)
            {
                if (MatchesPattern(relativePath, pattern))
                    return true;
            }

            // Check include patterns (if any specified)
            if (options.IncludePatterns.Any() && !options.IncludePatterns.Contains("*.*"))
            {
                var included = false;
                foreach (var pattern in options.IncludePatterns)
                {
                    if (MatchesPattern(relativePath, pattern))
                    {
                        included = true;
                        break;
                    }
                }
                if (!included)
                    return true;
            }

            return false;
        }

        private bool ShouldExcludeDirectory(string directoryPath, string rootPath, CrawlOptions options)
        {
            var relativePath = Path.GetRelativePath(rootPath, directoryPath);
            
            // Check exclude patterns
            foreach (var pattern in options.ExcludePatterns)
            {
                if (MatchesPattern(relativePath, pattern) || MatchesPattern(relativePath + "/", pattern))
                    return true;
            }

            return false;
        }

        private bool MatchesPattern(string path, string pattern)
        {
            // Convert glob pattern to regex
            var regexPattern = "^" + Regex.Escape(pattern)
                .Replace(@"\*", ".*")
                .Replace(@"\?", ".") + "$";
            
            return Regex.IsMatch(path, regexPattern, RegexOptions.IgnoreCase);
        }

        private bool IsSymbolicLink(string path)
        {
            try
            {
                var info = new System.IO.FileInfo(path);
                return info.Attributes.HasFlag(FileAttributes.ReparsePoint);
            }
            catch
            {
                return false;
            }
        }

        private FileType DetermineFileType(string extension)
        {
            return FileTypeMapping.GetValueOrDefault(extension.ToLowerInvariant(), FileType.Unknown);
        }

        private bool IsTextFile(string extension)
        {
            var textExtensions = new[] { ".cs", ".vb", ".cpp", ".c", ".h", ".hpp", ".java", ".js", ".ts", ".py", 
                ".sql", ".ps1", ".psm1", ".bat", ".cmd", ".config", ".xml", ".json", ".yaml", ".yml", 
                ".ini", ".properties", ".md", ".txt", ".html", ".htm" };
            
            return textExtensions.Contains(extension.ToLowerInvariant());
        }

        private async Task CategorizeFilesAsync(List<Models.FileInfo> allFiles, FileSystemAnalysis analysis, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                foreach (var file in allFiles)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    switch (file.Type)
                    {
                        case FileType.SourceCode:
                            analysis.SourceFiles.Add(file);
                            break;
                        case FileType.Configuration:
                            analysis.ConfigurationFiles.Add(file);
                            break;
                        case FileType.Resource:
                            analysis.ResourceFiles.Add(file);
                            break;
                        case FileType.Documentation:
                            analysis.DocumentationFiles.Add(file);
                            break;
                    }
                }
            }, cancellationToken);
        }

        private int CountDirectories(DirectoryStructure directory)
        {
            return 1 + directory.Subdirectories.Sum(CountDirectories);
        }

        private Dictionary<string, int> CalculateFileTypeDistribution(List<Models.FileInfo> files)
        {
            return files.GroupBy(f => f.Extension)
                       .ToDictionary(g => g.Key, g => g.Count());
        }

        #endregion
    }
}

