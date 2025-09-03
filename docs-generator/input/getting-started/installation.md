# Installation

StreamDingo is available as a NuGet package and requires .NET 9.0 or later.

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later
- A .NET project targeting `net9.0` or later

## Package Installation

### Using .NET CLI

```bash
dotnet add package StreamDingo
```

### Using Package Manager Console

```powershell
Install-Package StreamDingo
```

### Using PackageReference

Add the following to your `.csproj` file:

```xml
<PackageReference Include="StreamDingo" Version="0.1.0" />
```

## Development Installation

If you want to build StreamDingo from source:

```bash
git clone https://github.com/alexwiese/StreamDingo.git
cd StreamDingo
dotnet restore
dotnet build
```

## Next Steps

- [Quick Start Guide](quickstart.md) - Build your first event-sourced application
- [Basic Concepts](concepts.md) - Understand the core principles