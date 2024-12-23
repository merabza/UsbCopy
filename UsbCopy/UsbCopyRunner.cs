using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CliParameters;
using LibParameters;
using Microsoft.Extensions.Logging;
using SystemToolsShared;
using UsbCopy.Models;

namespace UsbCopy;

public sealed class UsbCopyRunner : ToolCommand
{
    private const string ActionName = "Copy Files";
    private const string ActionDescription = "This will Copy Files to specified storage";

    private readonly ILogger _logger;
    private readonly bool _useConsole;

    // ReSharper disable once ConvertToPrimaryConstructor
    public UsbCopyRunner(ILogger logger, bool useConsole, UsbCopyRunnerParameters usbCopyRunnerParameters,
        IParametersManager? parametersManager) : base(logger, ActionName,
        usbCopyRunnerParameters, parametersManager, ActionDescription)
    {
        _logger = logger;
        _useConsole = useConsole;
    }

    private UsbCopyRunnerParameters UsbCopyRunnerParameters => (UsbCopyRunnerParameters)Par;

    protected override ValueTask<bool> RunAction(CancellationToken cancellationToken = default)
    {
        try
        {
            ProcessFolder();
            return ValueTask.FromResult(true);
        }
        catch (Exception e)
        {
            StShared.WriteException(e, _useConsole);
            throw;
        }
    }

    private void ProcessFolder(string? afterRootPath = null)
    {
        var fileManager = UsbCopyRunnerParameters.FileManager;
        var localFileManager = UsbCopyRunnerParameters.MainFolderFileManager;
        var folderNames = fileManager.GetFolderNames(afterRootPath, null);
        foreach (var folderName in folderNames.OrderBy(o => o))
        {
            var folderAfterRootFullName = fileManager.PathCombine(afterRootPath, folderName);
            if (NeedExclude(folderAfterRootFullName))
                continue;
            if (folderName.Contains('#') || folderName.Contains('@'))
                continue;
            Console.WriteLine(folderAfterRootFullName);
            ProcessFolder(folderAfterRootFullName);
        }

        var files = fileManager.GetFileNames(afterRootPath, null)
            .Where(file => !NeedExclude(fileManager.PathCombine(afterRootPath, file))).ToList();

        if (files.Count == 0)
            return;

        var localAfterRootPath =
            afterRootPath?.Replace(fileManager.DirectorySeparatorChar, Path.DirectorySeparatorChar);
        var path = localAfterRootPath == null
            ? UsbCopyRunnerParameters.MainFolder
            : Path.Combine(UsbCopyRunnerParameters.MainFolder, localAfterRootPath);
        var localPatchChecked = FileStat.CreateFolderIfNotExists(path, _useConsole, _logger);
        if (localPatchChecked == null)
        {
            StShared.WriteErrorLine($"Cannot Create Folder {path}", _useConsole, _logger);
            return;
        }

        var folderFiles = GetFolderFiles(files);
        foreach (var fileName in folderFiles.Files.OrderBy(o => o))
        {
            if (localFileManager.FileExists(afterRootPath, fileName))
                continue;

            Console.WriteLine("File {0}", fileName);
            fileManager.DownloadFile(fileName, "dwn", afterRootPath);
        }

        foreach (var kvp in folderFiles.FileByPatterns.OrderBy(k => k.Key))
        {
            var fileInfo = kvp.Value.OrderByDescending(o => o.FileDateTime).First();

            if (localFileManager.FileExists(afterRootPath, fileInfo.FileName))
                continue;

            Console.WriteLine("Mask {0}", kvp.Key);
            Console.WriteLine("File By Mask {0}", fileInfo.FileName);
            fileManager.DownloadFile(fileInfo.FileName, "dwn", afterRootPath);
        }
    }


    private static FolderFilesModel GetFolderFiles(List<string> fileNames)
    {
        FolderFilesModel folderFiles = new();
        foreach (var fileName in fileNames)
        {
            var (dateTimeByDigits, pattern) =
                fileName.GetDateTimeAndPatternByDigits("yyyyMMddHHmmssfffffff");

            if (pattern != null)
            {
                if (!folderFiles.FileByPatterns.ContainsKey(pattern))
                    folderFiles.FileByPatterns.Add(pattern, new List<BuFileInfo>());

                folderFiles.FileByPatterns[pattern].Add(new BuFileInfo(fileName, dateTimeByDigits));
            }
            else
            {
                folderFiles.Files.Add(fileName);
            }
        }

        return folderFiles;
    }

    private bool NeedExclude(string name)
    {
        return UsbCopyRunnerParameters.Excludes is { Length: > 0 } &&
               UsbCopyRunnerParameters.Excludes.Any(name.FitsMask);
    }
}