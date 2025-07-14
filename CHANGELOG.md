# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2025-07-14

### Added
- Initial release of MCP.FileSystem server
- Comprehensive file operations (read, write, append, delete, copy, move)
- Directory operations (create, delete, list with filtering)
- Advanced search capabilities:
  - File name pattern search
  - Text content search within files
  - Regular expression search
- File and directory information tools
- Utility operations (working directory management, directory size calculation)
- Docker support with multi-stage builds
- Structured JSON responses for all operations
- Error handling and path validation
- Support for recursive operations
- Case-sensitive and case-insensitive search options
- Configurable result limits for search operations

### Technical Details
- Built on .NET 8.0
- Uses C# 12.0 language features
- Implements Model Context Protocol (MCP) server specification
- Uses Microsoft.Extensions.Hosting for application framework
- Includes comprehensive documentation and examples

### Security Features
- Path validation to prevent directory traversal
- Respect for file system permissions
- Comprehensive error handling
- Safe file operations with overwrite protection

## [Unreleased]

### Planned Features
- File watching capabilities
- Batch operations
- Compression/decompression tools
- File hash calculation
- Advanced file filtering options
- Performance optimizations for large directories