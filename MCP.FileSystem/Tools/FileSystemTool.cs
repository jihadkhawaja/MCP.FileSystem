using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MCP.FileSystem.Tools
{
    [McpServerToolType]
    public class FileSystemTool
    {
        #region File Operations

        [McpServerTool, Description("Reads the content of a file at the specified path.")]
        public static string ReadFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return JsonSerializer.Serialize(new { error = "File not found", path = filePath });

                var content = File.ReadAllText(filePath);
                return JsonSerializer.Serialize(new { success = true, content, path = filePath, size = content.Length });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { error = ex.Message, path = filePath });
            }
        }

        [McpServerTool, Description("Writes content to a file at the specified path. Creates the file if it doesn't exist.")]
        public static string WriteFile(string filePath, string content, bool overwrite = true)
        {
            try
            {
                if (File.Exists(filePath) && !overwrite)
                    return JsonSerializer.Serialize(new { error = "File already exists and overwrite is false", path = filePath });

                // Ensure directory exists
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                File.WriteAllText(filePath, content);
                return JsonSerializer.Serialize(new { success = true, message = "File written successfully", path = filePath, size = content.Length });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { error = ex.Message, path = filePath });
            }
        }

        [McpServerTool, Description("Appends content to an existing file or creates a new file if it doesn't exist.")]
        public static string AppendToFile(string filePath, string content)
        {
            try
            {
                // Ensure directory exists
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                File.AppendAllText(filePath, content);
                var fileInfo = new FileInfo(filePath);
                return JsonSerializer.Serialize(new { success = true, message = "Content appended successfully", path = filePath, size = fileInfo.Length });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { error = ex.Message, path = filePath });
            }
        }

        [McpServerTool, Description("Deletes a file at the specified path.")]
        public static string DeleteFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return JsonSerializer.Serialize(new { error = "File not found", path = filePath });

                File.Delete(filePath);
                return JsonSerializer.Serialize(new { success = true, message = "File deleted successfully", path = filePath });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { error = ex.Message, path = filePath });
            }
        }

        [McpServerTool, Description("Copies a file from source to destination path.")]
        public static string CopyFile(string sourcePath, string destinationPath, bool overwrite = false)
        {
            try
            {
                if (!File.Exists(sourcePath))
                    return JsonSerializer.Serialize(new { error = "Source file not found", sourcePath });

                if (File.Exists(destinationPath) && !overwrite)
                    return JsonSerializer.Serialize(new { error = "Destination file already exists and overwrite is false", destinationPath });

                // Ensure destination directory exists
                var directory = Path.GetDirectoryName(destinationPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                File.Copy(sourcePath, destinationPath, overwrite);
                return JsonSerializer.Serialize(new { success = true, message = "File copied successfully", sourcePath, destinationPath });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { error = ex.Message, sourcePath, destinationPath });
            }
        }

        [McpServerTool, Description("Moves a file from source to destination path.")]
        public static string MoveFile(string sourcePath, string destinationPath, bool overwrite = false)
        {
            try
            {
                if (!File.Exists(sourcePath))
                    return JsonSerializer.Serialize(new { error = "Source file not found", sourcePath });

                if (File.Exists(destinationPath) && !overwrite)
                    return JsonSerializer.Serialize(new { error = "Destination file already exists and overwrite is false", destinationPath });

                // Ensure destination directory exists
                var directory = Path.GetDirectoryName(destinationPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                if (File.Exists(destinationPath) && overwrite)
                    File.Delete(destinationPath);

                File.Move(sourcePath, destinationPath);
                return JsonSerializer.Serialize(new { success = true, message = "File moved successfully", sourcePath, destinationPath });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { error = ex.Message, sourcePath, destinationPath });
            }
        }

        #endregion

        #region Directory Operations

        [McpServerTool, Description("Creates a directory at the specified path.")]
        public static string CreateDirectory(string directoryPath)
        {
            try
            {
                if (Directory.Exists(directoryPath))
                    return JsonSerializer.Serialize(new { message = "Directory already exists", path = directoryPath });

                Directory.CreateDirectory(directoryPath);
                return JsonSerializer.Serialize(new { success = true, message = "Directory created successfully", path = directoryPath });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { error = ex.Message, path = directoryPath });
            }
        }

        [McpServerTool, Description("Deletes a directory at the specified path. Use recursive=true to delete non-empty directories.")]
        public static string DeleteDirectory(string directoryPath, bool recursive = false)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                    return JsonSerializer.Serialize(new { error = "Directory not found", path = directoryPath });

                Directory.Delete(directoryPath, recursive);
                return JsonSerializer.Serialize(new { success = true, message = "Directory deleted successfully", path = directoryPath });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { error = ex.Message, path = directoryPath });
            }
        }

        [McpServerTool, Description("Lists files and directories in the specified path.")]
        public static string ListDirectory(string directoryPath, string searchPattern = "*", bool includeSubdirectories = false)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                    return JsonSerializer.Serialize(new { error = "Directory not found", path = directoryPath });

                var searchOption = includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

                var files = Directory.GetFiles(directoryPath, searchPattern, searchOption)
                    .Select(f => new
                    {
                        type = "file",
                        name = Path.GetFileName(f),
                        path = f,
                        size = new FileInfo(f).Length,
                        lastModified = File.GetLastWriteTime(f)
                    });

                var directories = Directory.GetDirectories(directoryPath, searchPattern, searchOption)
                    .Select(d => new
                    {
                        type = "directory",
                        name = Path.GetFileName(d),
                        path = d,
                        size = (long?)null,
                        lastModified = Directory.GetLastWriteTime(d)
                    });

                var items = files.Cast<object>().Concat(directories.Cast<object>()).ToArray();

                return JsonSerializer.Serialize(new
                {
                    success = true,
                    path = directoryPath,
                    items,
                    count = items.Length
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { error = ex.Message, path = directoryPath });
            }
        }

        #endregion

        #region Search Operations

        [McpServerTool, Description("Searches for files by name pattern in the specified directory and optionally subdirectories.")]
        public static string SearchFiles(string directoryPath, string searchPattern = "*", bool includeSubdirectories = true, int maxResults = 100)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                    return JsonSerializer.Serialize(new { error = "Directory not found", path = directoryPath });

                var searchOption = includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

                var files = Directory.GetFiles(directoryPath, searchPattern, searchOption)
                    .Take(maxResults)
                    .Select(f => new
                    {
                        name = Path.GetFileName(f),
                        path = f,
                        directory = Path.GetDirectoryName(f),
                        size = new FileInfo(f).Length,
                        lastModified = File.GetLastWriteTime(f)
                    })
                    .ToArray();

                return JsonSerializer.Serialize(new
                {
                    success = true,
                    searchPath = directoryPath,
                    pattern = searchPattern,
                    files,
                    count = files.Length,
                    truncated = files.Length >= maxResults
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { error = ex.Message, path = directoryPath });
            }
        }

        [McpServerTool, Description("Searches for content within files using text pattern matching.")]
        public static string SearchInFiles(string directoryPath, string searchText, string filePattern = "*", bool includeSubdirectories = true, bool caseSensitive = false, int maxResults = 50)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                    return JsonSerializer.Serialize(new { error = "Directory not found", path = directoryPath });

                var searchOption = includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                var stringComparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                var results = new List<object>();

                var files = Directory.GetFiles(directoryPath, filePattern, searchOption);

                foreach (var file in files)
                {
                    if (results.Count >= maxResults) break;

                    try
                    {
                        var content = File.ReadAllText(file);
                        var lines = content.Split('\n');

                        for (int i = 0; i < lines.Length; i++)
                        {
                            if (lines[i].Contains(searchText, stringComparison))
                            {
                                results.Add(new
                                {
                                    file = file,
                                    fileName = Path.GetFileName(file),
                                    lineNumber = i + 1,
                                    line = lines[i].Trim(),
                                    matchPosition = lines[i].IndexOf(searchText, stringComparison)
                                });

                                if (results.Count >= maxResults) break;
                            }
                        }
                    }
                    catch
                    {
                        // Skip files that can't be read as text
                        continue;
                    }
                }

                return JsonSerializer.Serialize(new
                {
                    success = true,
                    searchPath = directoryPath,
                    searchText,
                    filePattern,
                    caseSensitive,
                    matches = results,
                    count = results.Count,
                    truncated = results.Count >= maxResults
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { error = ex.Message, path = directoryPath });
            }
        }

        [McpServerTool, Description("Searches for content within files using regular expression pattern matching.")]
        public static string SearchInFilesRegex(string directoryPath, string regexPattern, string filePattern = "*", bool includeSubdirectories = true, bool caseSensitive = false, int maxResults = 50)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                    return JsonSerializer.Serialize(new { error = "Directory not found", path = directoryPath });

                var searchOption = includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                var regexOptions = caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
                var regex = new Regex(regexPattern, regexOptions);
                var results = new List<object>();

                var files = Directory.GetFiles(directoryPath, filePattern, searchOption);

                foreach (var file in files)
                {
                    if (results.Count >= maxResults) break;

                    try
                    {
                        var content = File.ReadAllText(file);
                        var lines = content.Split('\n');

                        for (int i = 0; i < lines.Length; i++)
                        {
                            var matches = regex.Matches(lines[i]);
                            foreach (Match match in matches)
                            {
                                results.Add(new
                                {
                                    file = file,
                                    fileName = Path.GetFileName(file),
                                    lineNumber = i + 1,
                                    line = lines[i].Trim(),
                                    match = match.Value,
                                    matchPosition = match.Index,
                                    groups = match.Groups.Cast<Group>().Skip(1).Select(g => g.Value).ToArray()
                                });

                                if (results.Count >= maxResults) break;
                            }
                            if (results.Count >= maxResults) break;
                        }
                    }
                    catch
                    {
                        // Skip files that can't be read as text
                        continue;
                    }
                }

                return JsonSerializer.Serialize(new
                {
                    success = true,
                    searchPath = directoryPath,
                    regexPattern,
                    filePattern,
                    caseSensitive,
                    matches = results,
                    count = results.Count,
                    truncated = results.Count >= maxResults
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { error = ex.Message, path = directoryPath, regex = regexPattern });
            }
        }

        #endregion

        #region File Information

        [McpServerTool, Description("Gets detailed information about a file or directory.")]
        public static string GetFileInfo(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    var fileInfo = new FileInfo(path);
                    return JsonSerializer.Serialize(new
                    {
                        success = true,
                        type = "file",
                        name = fileInfo.Name,
                        path = fileInfo.FullName,
                        size = fileInfo.Length,
                        extension = fileInfo.Extension,
                        created = fileInfo.CreationTime,
                        lastModified = fileInfo.LastWriteTime,
                        lastAccessed = fileInfo.LastAccessTime,
                        isReadOnly = fileInfo.IsReadOnly,
                        attributes = fileInfo.Attributes.ToString()
                    });
                }
                else if (Directory.Exists(path))
                {
                    var dirInfo = new DirectoryInfo(path);
                    var fileCount = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly).Length;
                    var subdirCount = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly).Length;

                    return JsonSerializer.Serialize(new
                    {
                        success = true,
                        type = "directory",
                        name = dirInfo.Name,
                        path = dirInfo.FullName,
                        created = dirInfo.CreationTime,
                        lastModified = dirInfo.LastWriteTime,
                        lastAccessed = dirInfo.LastAccessTime,
                        fileCount,
                        subdirectoryCount = subdirCount,
                        attributes = dirInfo.Attributes.ToString()
                    });
                }
                else
                {
                    return JsonSerializer.Serialize(new { error = "Path not found", path });
                }
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { error = ex.Message, path });
            }
        }

        [McpServerTool, Description("Checks if a file or directory exists at the specified path.")]
        public static string PathExists(string path)
        {
            try
            {
                var fileExists = File.Exists(path);
                var directoryExists = Directory.Exists(path);

                return JsonSerializer.Serialize(new
                {
                    success = true,
                    path,
                    exists = fileExists || directoryExists,
                    isFile = fileExists,
                    isDirectory = directoryExists
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { error = ex.Message, path });
            }
        }

        #endregion

        #region Utility Operations

        [McpServerTool, Description("Gets the current working directory.")]
        public static string GetCurrentDirectory()
        {
            try
            {
                var currentDir = Directory.GetCurrentDirectory();
                return JsonSerializer.Serialize(new { success = true, currentDirectory = currentDir });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { error = ex.Message });
            }
        }

        [McpServerTool, Description("Changes the current working directory.")]
        public static string SetCurrentDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                    return JsonSerializer.Serialize(new { error = "Directory not found", path });

                Directory.SetCurrentDirectory(path);
                var newDir = Directory.GetCurrentDirectory();
                return JsonSerializer.Serialize(new { success = true, message = "Directory changed successfully", currentDirectory = newDir });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { error = ex.Message, path });
            }
        }

        [McpServerTool, Description("Calculates the total size of a directory including all subdirectories and files.")]
        public static string GetDirectorySize(string directoryPath)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                    return JsonSerializer.Serialize(new { error = "Directory not found", path = directoryPath });

                long totalSize = 0;
                int fileCount = 0;
                int directoryCount = 0;

                var dirInfo = new DirectoryInfo(directoryPath);

                foreach (var file in dirInfo.GetFiles("*", SearchOption.AllDirectories))
                {
                    totalSize += file.Length;
                    fileCount++;
                }

                foreach (var dir in dirInfo.GetDirectories("*", SearchOption.AllDirectories))
                {
                    directoryCount++;
                }

                return JsonSerializer.Serialize(new
                {
                    success = true,
                    path = directoryPath,
                    totalSize,
                    totalSizeMB = Math.Round(totalSize / (1024.0 * 1024.0), 2),
                    fileCount,
                    directoryCount
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { error = ex.Message, path = directoryPath });
            }
        }

        #endregion
    }
}
