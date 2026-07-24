# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

UsbCopy is an interactive .NET 10 console application (CLI menu loop) that copies files from a backup storage (local path or FTP) down to a local folder such as a USB drive. Configuration lives in a JSON parameters file passed on the command line; the app edits that file through its own menus.

## Multi-repo workspace

This repository only works when cloned side-by-side with its sibling repositories, which the `.csproj`/`.slnx` reference via `../`:

```
<workspace>/
├── AppCliTools/            (CLI menu, parameter editing, data input)
├── ConnectionTools/        (FTP and connection tooling)
├── ParametersManagement/   (parameters file loading/saving)
├── SystemTools/            (shared utilities, Serilog setup)
├── ToolsManagement/        (FileManagers: local + remote file abstraction)
└── UsbCopy/                ← this repo (contains UsbCopy.slnx)
```

Each sibling is its own git repository (see README.md for clone commands). Most of the framework code (menu system, cruders, field editors, FileManager) lives in the siblings — changes there must be committed in the corresponding repo, not here.

## Commands

```powershell
# Build (solution includes sibling projects)
dotnet build UsbCopy.slnx

# Run (parameters JSON path is required via --use)
dotnet run --project UsbCopy -- --use "<path>\UsbCopy.json"
```

There are no test projects in this solution.

Builds are strict: `TreatWarningsAsErrors`, `AnalysisMode=All`, `EnforceCodeStyleInBuild`, and SonarAnalyzer are enabled via `Directory.Build.props`, so any analyzer warning fails the build. `ImplicitUsings` is **disabled** — every file needs explicit `using System;` etc. NuGet versions are centrally managed in `Directory.Packages.props`.

## Architecture

**Startup** ([Program.cs](UsbCopy/Program.cs)): top-level statements parse args with `ArgumentsParser<UsbCopyParameters>`, build the DI container via [UsbCopyServices.AddServices](UsbCopy/DependencyInjection/UsbCopyServices.cs), then run `CliAppLoop` (from AppCliTools), which drives the interactive menu. Distinct exit codes: 0 ok, 1 usage, 2 arg error, 3 DI/loop-params failure, 4 exception, 100 loop returned false.

**Menu system** (strategy pattern, resolved from DI): [UsbCopyMenuBuilder](UsbCopy/UsbCopyMenuBuilder.cs) builds the main menu from `MenuData.MainMenuCommandFactoryStrategyNames` — a list of strategy *class names*. Strategies implement `IMenuCommandFactoryStrategy` (single command) or `IMenuCommandListFactoryStrategy` (one command per project) and are auto-registered by assembly scanning (`AddTransientAllStrategies`) in `UsbCopyServices`. To add a main-menu entry: create a strategy class in `Menu/` and add its `nameof` to [MenuData.cs](UsbCopy/Menu/MenuData.cs).

**Parameters model**: [UsbCopyParameters](UsbCopy/Models/UsbCopyParameters.cs) is the persisted JSON root — dictionaries of `Projects`, `FileStorages`, `ExcludeSets`. It implements `IParametersWithFileStorages`/`IParametersWithExcludeSets` so generic cruders/field editors from AppCliTools can edit those dictionaries. Each [UsbCopyProjectModel](UsbCopy/Models/UsbCopyProjectModel.cs) is just `LocalPath` + `FileStorageName` + `ExcludeSetName`. Editing goes through the *cruder* pattern ([UsbCopyProjectCruder](UsbCopy/Cruders/UsbCopyProjectCruder.cs) with `FieldEditors`) and [UsbCopyParametersEditor](UsbCopy/Menu/UsbCopyParametersEdit/UsbCopyParametersEditor.cs); `IParametersManager.Save` persists the file after mutations.

**Copy engine** (the actual work): `CopyFilesCliMenuCommand` → `ToolCommandFactory` → [UsbCopyRunnerParameters.Create](UsbCopy/UsbCopyRunnerParameters.cs) → [UsbCopyRunnerToolCommand](UsbCopy/ToolCommands/UsbCopyRunnerToolCommand.cs).

- `UsbCopyRunnerParameters.Create` validates the project, builds two `FileManager`s (remote source from the file storage, local destination), and picks the destination folder: a `yyyyMMddHHmmss`-named subfolder of `LocalPath` — reusing the latest existing one only if the user confirms, otherwise creating a new one from the current time.
- `UsbCopyRunnerToolCommand.ProcessFolder` recursively walks the remote storage: skips paths matching the exclude set masks and folders whose names contain `#` or `@`; downloads files that don't already exist locally. File names containing a `yyyyMMddHHmmssfffffff` digit sequence are treated as timestamped backups: they're grouped by name pattern and **only the newest file per pattern** is downloaded.

## Conventions

- Comments in Georgian (e.g. `//მთავარი მენიუს ჩატვირთვა`) are intentional — keep them and don't translate or remove them.
- Explicit constructors are used instead of primary constructors, marked with `// ReSharper disable once ConvertToPrimaryConstructor`. Follow this style.
- Classes are `sealed` where possible; error reporting uses `StShared.WriteErrorLine`/`WriteException` and "return null on failure" rather than exceptions.
