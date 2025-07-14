# MCP.FileSystem

A comprehensive **Model Context Protocol (MCP) Server** that provides powerful file system operations through standardized tools. This server enables AI assistants and other MCP clients to interact with the file system safely and efficiently.

## Features

### File Operations
- **Read/Write Files**: Read content from files or create new files with custom content
- **Append Content**: Add content to existing files without overwriting
- **Delete Files**: Remove files from the file system
- **Copy Files**: Duplicate files to new locations
- **Move/Rename Files**: Relocate or rename files

### Directory Operations
- **Create Directories**: Make new directories with automatic parent creation
- **Delete Directories**: Remove directories with optional recursive deletion
- **List Directory Contents**: Browse files and folders with filtering options

### Advanced Search Capabilities
- **File Name Search**: Find files by name patterns with wildcard support
- **Content Search**: Search for text within files across directories
- **Regex Search**: Advanced pattern matching using regular expressions
- **Recursive Searching**: Search through subdirectories

### File Information & Utilities
- **File Metadata**: Get detailed information about files and directories
- **Path Validation**: Check if files or directories exist
- **Directory Size**: Calculate total size of directories including subdirectories
- **Working Directory**: Get and set current working directory

## Technology Stack

- **.NET 8.0**: Built on the latest .NET platform
- **C# 12.0**: Using modern C# language features
- **Model Context Protocol**: Implements MCP server specification
- **JSON Serialization**: Structured responses for easy integration
- **Docker Support**: Ready for containerized deployment

## Installation

### Prerequisites
- .NET 8.0 SDK or later
- Compatible MCP client (Claude Desktop, VS Code with MCP extension, etc.)

### Building from Source
```bash
git clone https://github.com/jihadkhawaja/MCP.FileSystem.git
cd MCP.FileSystem
dotnet build
```

### Running the Server
```bash
dotnet run --project MCP.FileSystem
```

### Docker Deployment
```bash
docker build -t mcp-filesystem .
docker run -it mcp-filesystem
```

## Usage

### MCP Client Configuration

#### Claude Desktop
Add to your Claude Desktop configuration:
```json
{
  "mcpServers": {
    "MCP.FileSystem": {
      "command": "dotnet",
      "args": ["run", "--project", "path/to/MCP.FileSystem.csproj"]
    }
  }
}
```

#### VS Code with MCP Extension
Configure in your MCP settings:
```json
"servers": {
  "MCP.FileSystem": {
    "type": "stdio",
    "command": "dotnet",
    "args": [
      "run",
      "--project",
      "path/to/MCP.FileSystem.csproj"
    ]
  }
}
```

## Available Tools

### File Operations
| Tool | Description | Parameters |
|------|-------------|------------|
| `ReadFile` | Read content from a file | `filePath` |
| `WriteFile` | Write content to a file | `filePath`, `content`, `overwrite?` |
| `AppendToFile` | Append content to a file | `filePath`, `content` |
| `DeleteFile` | Delete a file | `filePath` |
| `CopyFile` | Copy a file | `sourcePath`, `destinationPath`, `overwrite?` |
| `MoveFile` | Move/rename a file | `sourcePath`, `destinationPath`, `overwrite?` |

### Directory Operations
| Tool | Description | Parameters |
|------|-------------|------------|
| `CreateDirectory` | Create a directory | `directoryPath` |
| `DeleteDirectory` | Delete a directory | `directoryPath`, `recursive?` |
| `ListDirectory` | List directory contents | `directoryPath`, `searchPattern?`, `includeSubdirectories?` |

### Search Operations
| Tool | Description | Parameters |
|------|-------------|------------|
| `SearchFiles` | Search for files by pattern | `directoryPath`, `searchPattern?`, `includeSubdirectories?`, `maxResults?` |
| `SearchInFiles` | Search for text in files | `directoryPath`, `searchText`, `filePattern?`, `includeSubdirectories?`, `caseSensitive?`, `maxResults?` |
| `SearchInFilesRegex` | Search using regex patterns | `directoryPath`, `regexPattern`, `filePattern?`, `includeSubdirectories?`, `caseSensitive?`, `maxResults?` |

### Information & Utilities
| Tool | Description | Parameters |
|------|-------------|------------|
| `GetFileInfo` | Get file/directory metadata | `path` |
| `PathExists` | Check if path exists | `path` |
| `GetCurrentDirectory` | Get current working directory | - |
| `SetCurrentDirectory` | Set current working directory | `path` |
| `GetDirectorySize` | Calculate directory size | `directoryPath` |

## Example Responses

All tools return structured JSON responses:

### Success Response
```json
{
  "success": true,
  "message": "File written successfully",
  "path": "/path/to/file.txt",
  "size": 1024
}
```

### Error Response
```json
{
  "error": "File not found",
  "path": "/path/to/nonexistent.txt"
}
```

### Search Results
```json
{
  "success": true,
  "searchPath": "/search/directory",
  "matches": [
    {
      "file": "/path/to/match.txt",
      "fileName": "match.txt",
      "lineNumber": 42,
      "line": "Found matching content here",
      "matchPosition": 6
    }
  ],
  "count": 1,
  "truncated": false
}
```

## Security Considerations

- **Path Validation**: All file paths are validated to prevent directory traversal attacks
- **Error Handling**: Comprehensive error handling prevents information leakage
- **Permission Respect**: Operations respect file system permissions
- **Sandboxing**: Consider running in a containerized environment for additional security

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## Dependencies

- `Microsoft.Extensions.Hosting` (9.0.7) - Application hosting framework
- `ModelContextProtocol` (0.3.0-preview.2) - MCP server implementation
- `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (1.22.1) - Docker support

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE.md) file for details.

## Changelog

### v1.0.0
- Initial release with comprehensive file system operations
- Support for file and directory operations
- Advanced search capabilities with regex support
- Docker deployment support
- Complete MCP server implementation

## Support

For questions, issues, or contributions, please visit the [GitHub repository](https://github.com/jihadkhawaja/MCP.FileSystem) or open an issue.

---

**Note**: This MCP server provides powerful file system access. Ensure you understand the security implications and run it in appropriate environments with proper access controls.
