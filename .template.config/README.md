# MudBlazor Electron Template

This template creates a new MudBlazor application with ElectronSharp support for .NET 10.

## Installation

Install the template from the local folder:

```bash
dotnet new install .
```

Or install from NuGet (once published):

```bash
dotnet new install MudBlazor.Electron.Template
```

## Usage

Create a new project:

```bash
dotnet new mudblazor-electron -n MyApp
```

## What's Included

- MudBlazor components with Material Design
- Mixed SSR and WebAssembly rendering modes
- ElectronSharp desktop application support
- Custom theming system with color tints
- Virtualized data tables
- Dark and light mode toggle
- Works in both browser and desktop

## Running the Application

### Browser Mode

```bash
cd MyApp/MyApp
dotnet run
```

### Desktop Mode

```bash
cd MyApp/MyApp.Desktop
dotnet run
```

## Uninstalling

```bash
dotnet new uninstall MudBlazor.Electron.Template
```
