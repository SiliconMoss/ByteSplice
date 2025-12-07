# ByteSplice

A lightweight, portable binary patching tool for Windows, written in C# (.NET Framework 4.0).

## Features

- **Pattern Matching**: Find and replace byte sequences using hex patterns.
- **Wildcards**: Support for `??` wildcards in patterns (e.g., `AA ?? BB`).
- **Safety**: Automatically creates specific `.bak` backups before patching (unless disabled).
- **Portable**: Can be compiled into a single self-extracting batch file with zero external dependencies.
- **Configurable**: run patches from a readable configuration file or passing command line arguments.

## Usage

### Command Line Mode
```bash
ByteSplice.exe <target_file> --pattern "AA BB CC" --replace "DD EE FF"
```

### Config File Mode
```bash
ByteSplice.exe <target_file> --config patches.txt
```

See `patches.txt` or use `--create-sample` for configuration format.

## Building

### Requirements
- Windows (XP or later)
- .NET Framework 4.0 (Installed by default on almost all modern Windows systems)

### Standard Build
Run `build.bat`. This will compile `ByteSplice.exe` using the system C# compiler.

### Portable Build
Run `build.bat`. It will automatically generate `ByteSplice_Portable.bat`.
This batch file is a standalone executable script that contains the full source code and compiles itself on the fly.

## Project Structure

- `src/`: C# Source code
- `Tools/Portable/`: Scripts for generating the portable batch file
- `Tests/`: Automated test suite
