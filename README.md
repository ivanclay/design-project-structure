# 🌳 Design Project Structure

> **An elegant tool to visualize and document the directory structure of any software project**

[![.NET](https://img.shields.io/badge/.NET-512BD4?style=flat&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=c-sharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Console](https://img.shields.io/badge/Console-Application-blue)](https://github.com/)

![](/docs/images/DesignProjectStructure.gif)

## ✨ Features

**Design Project Structure** is a C# console application that generates elegant visualizations of software project file and folder structures, with intelligent project type detection and real-time animated interface.

### 🎯 **Key Features**

- **🔍 Automatic Project Detection**: Automatically identifies .NET, Node.js, Python, Java, PHP, Ruby, Go, Rust, and more
- **🎨 Animated Interface**: Real-time visualization with colorful borders and progress tracking
- **📁 Hierarchical Structure**: Clear visual representation with ASCII art connectors
- **🏷️ Contextual Icons**: Specific icons for different file and folder types
- **🚫 Smart Filtering**: Automatically ignores unnecessary files/folders (.git, node_modules, bin, obj, etc.)
- **📊 Real-time Statistics**: Live counters for folders, files, and processing progress
- **💾 Export Capability**: Saves complete structure to text file
- **🔒 Error Handling**: Gracefully handles access denied and other errors

## 🖼️ Interface Preview

```
╔═══════════════════════════════════════════════════════════════════════════════╗
║ ╣ PROJECT INFORMATION ╠                                                      ║
║ [.NET] MyProject - ASP.NET Web API                                           ║
║ Tech: C# | Framework: .NET                                                   ║
║ Desc: ASP.NET Core Web Application                                           ║
║ Path: D:\Projects\MyProject                                                  ║
║ Output: structure.txt | 08/09/2025 15:30:45                                 ║
╚═══════════════════════════════════════════════════════════════════════════════╝

╔═══════════════════════════════════════════════════════════════════════════════╗
║ ╣ FILE STRUCTURE ╠                                                           ║
║ [DIR] MyProject                                                              ║
║ ├── [CTRL] Controllers                                                       ║
║ │   ├── [C#] WeatherController.cs                                            ║
║ │   └── [C#] HomeController.cs                                               ║
║ ├── [MODEL] Models                                                           ║
║ │   └── [C#] WeatherForecast.cs                                              ║
║ ├── [PROJ] MyProject.csproj                                                  ║
║ └── [C#] Program.cs                                                          ║
╚═══════════════════════════════════════════════════════════════════════════════╝

╔═══════════════════════════════════════════════════════════════════════════════╗
║ ╣ PROCESSING DATA ╠                                                          ║
║ [DIR] FOLDERS FOUND: 012                                                     ║
║ [FILE] FILES FOUND: 025                                                      ║
║ [OK] PROCESSED: 037 of 037                                                   ║
║                                                                              ║
║ Progress: [100%] ████████████████████████████████████████████████████████████ ║
║                                                                              ║
║ COMPLETED! File saved: project_structure.txt                                ║
╚═══════════════════════════════════════════════════════════════════════════════╝
```

## 🚀 How to Use

### Basic Execution
```bash
dotnet run
```

### Specifying a Custom Path
```bash
dotnet run "C:\MyProject"
```

### Using Compiled Executable
```bash
DesignProjectStructure.exe "C:\MyProject"
```

## 🏗️ Project Architecture

The project was developed following **Clean Code** principles and **Separation of Concerns**:

```
📁 DesignProjectStructure
├── 📁 Models
│   └── 🔷 StructureItens.cs          # Data model
├── 📁 Helpers
│   ├── 🔷 AnimatorStructureGenerator.cs  # Recursive structure generation
│   ├── 🔷 ProjectTypeDetector.cs         # Intelligent project detection
│   ├── 🔷 ConsoleRenderer.cs             # Console visual interface
│   ├── 🔷 StructureGenerator.cs          # Processing logic
│   ├── 🔷 IconProvider.cs                # Contextual icon provider
│   ├── 🔷 IgnoreFilter.cs                # File filtering system
│   └── 🔷 FileWriter.cs                  # File export functionality
└── 🔷 Program.cs                         # Entry point
```

## 🎛️ Supported Project Types

| Technology | Detection | Console Icon | Examples |
|------------|-----------|--------------|----------|
| **.NET** | `.csproj`, `.sln`, `.cs` files | `[.NET]` | Web API, Console, Library |
| **Node.js** | `package.json` | `[NODE]` | React, Angular, Express |
| **Python** | `requirements.txt`, `.py` files | `[PY]` | Django, Flask, FastAPI |
| **Java** | `pom.xml`, `build.gradle`, `.java` | `[JAVA]` | Spring Boot, Maven |
| **PHP** | `composer.json`, `.php` files | `[PHP]` | Laravel, Symfony |
| **Ruby** | `Gemfile`, `.rb` files | `[RUBY]` | Rails, Scripts |
| **Go** | `go.mod`, `.go` files | `[GO]` | Applications, Services |
| **Rust** | `Cargo.toml`, `.rs` files | `[RUST]` | Applications, Libraries |
| **Frontend** | `.html`, `.css`, `.js` files | `[WEB]` | Static Websites |

## ⚙️ Configuration

### Customizing Paths
Edit the variables in `Program.cs`:

```csharp
string rootPath = "YOUR_PATH_HERE";
string outputFile = "YOUR_OUTPUT_FILE.txt";
```

### Adding Custom Filters
Modify the `IgnoreFilter` class to add new patterns:

```csharp
private static readonly string[] ignore = {
    ".git", ".vs", "node_modules", "bin", "obj",
    // Add your custom filters here
};
```

## 📝 Output File

The generated file contains:
- **Header** with project information and timestamp
- **Complete structure** with elegant Unicode icons
- **Visual hierarchy** using ASCII tree characters
- **Statistics** of processed folders and files

Example output:
```
Project Structure: MyProject
Path: D:\Projects\MyProject
Generated in: 08/09/2025 15:30:45
============================================================

📁 MyProject
├── 📁 Controllers
│   ├── 🔷 WeatherController.cs
│   └── 🔷 HomeController.cs
├── 📁 Models
│   └── 🔷 WeatherForecast.cs
├── 📋 MyProject.csproj
└── 🔷 Program.cs
```

## 🛠️ Technical Requirements

- **.NET 6.0 or higher**
- **Windows, Linux, or macOS**
- **Terminal with Unicode character support** (for best visual experience)

## 🤝 Contributing

Contributions are welcome! Feel free to:

1. 🐛 Report bugs
2. 💡 Suggest improvements
3. 🔧 Add new project types
4. 🎨 Improve visual interface
5. 📚 Enhance documentation

## 📄 License

This project is licensed under the GNU General Public License Version 3 (GPL v3). See the `LICENSE` file for more details.

---

<div align="center">

**Developed with ❤️ for the developer community**

*Transform project structure analysis into an elegant visual experience*

</div>