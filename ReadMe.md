# MsixInstaller

| English | [简体中文](ReadMe.zh-Hans.md) |

GitHub Action for creating a wrapper around MSIX installers. This tool embeds MSIX packages into standalone .NET Framework executables for easy distribution and deployment.

## Features

- 📦 Automatically embed MSIX packages into .NET Framework 4.8 executables
- 🎨 Modern user interface built with WPF-UI
- 🔄 Automatic system theme following
- 📝 Automatic installer checksum generation
- 🏗️ Configurable output architecture (x86/x64/ARM/ARM64/Any)
- 🔍 Automatically extract version and architecture from MSIX manifest

## Usage

### As a GitHub Action

Add the following step to your workflow file:

```yaml
- name: Create MSIX Installer
  uses: frg2089/MsixInstaller@main
  with:
    msix: 'path/to/your/app.msix'
    name: 'MyAppInstaller'
    arch: 'x64'  # Optional: neutral|x86|x64|arm|arm64
    version: '1.0.0.0'  # Optional: Defaults to reading from MSIX manifest
```

### Output Parameters

This Action outputs the following information:

- `path`: Full path to the installer
- `algorithm`: Checksum algorithm (e.g., SHA256)
- `checksum`: Installer checksum value

Example:

```yaml
- name: Upload Installer
  uses: actions/upload-artifact@v4
  with:
    name: installer
    path: ${{ steps.create_installer.outputs.path }}
```

## Local Build

### Prerequisites

- .NET SDK 9.0.300 or higher
- .NET Framework 4.8

### Build Steps

```powershell
# Build the project
dotnet build

# Publish to specific architecture
dotnet publish --configuration Release --arch x64 --artifacts-path artifacts
```

## Tech Stack

- **Framework**: .NET Framework 4.8 + WPF
- **UI Library**: WPF-UI 4.0.3
- **MVVM**: CommunityToolkit.Mvvm 8.4.0
- **Resource Embedding**: Costura.Fody 6.0.0
- **Language**: C# 12

## Project Structure

```
MsixInstaller/
├── action.yml          # GitHub Action configuration
├── scripts/
│   └── Pack-MsixInstaller.ps1  # Packaging script
├── src/
│   └── MsixInstaller/  # WPF application source
│       ├── MainWindow.xaml    # Main window
│       ├── Program.cs         # Entry point
│       └── Resources/         # Resource files
└── artifacts/          # Build output directory
```

## License

This project is open source. See [LICENSE](LICENSE) for details.

## Author

frg2089
