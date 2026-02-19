using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppCliTools.CliParameters;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using SystemTools.SystemToolsShared;
using ToolsManagement.FileManagersMain;
using UsbCopy.Models;

namespace UsbCopy;

public sealed class UsbCopyRunnerCommand : ToolCommand
{
    private const string ActionName = "Copy Files";
    private const string ActionDescription = "This will Copy Files to specified storage";

    private readonly ILogger _logger;
    private readonly bool _useConsole;

    // ReSharper disable once ConvertToPrimaryConstructor
    public UsbCopyRunnerCommand(ILogger logger, bool useConsole, UsbCopyRunnerParameters usbCopyRunnerParameters,
        IParametersManager? parametersManager) : base(logger, ActionName, usbCopyRunnerParameters, parametersManager,
        ActionDescription)
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
        FileManager fileManager = UsbCopyRunnerParameters.FileManager;
        FileManager localFileManager = UsbCopyRunnerParameters.MainFolderFileManager;
        List<string> folderNames = fileManager.GetFolderNames(afterRootPath, null);
        foreach (string folderName in folderNames.OrderBy(o => o))
        {
            string folderAfterRootFullName = fileManager.PathCombine(afterRootPath, folderName);
            if (NeedExclude(folderAfterRootFullName))
            {
                continue;
            }

            if (folderName.Contains('#') || folderName.Contains('@'))
            {
                continue;
            }

            Console.WriteLine(folderAfterRootFullName);
            ProcessFolder(folderAfterRootFullName);
        }

        List<string> files = fileManager.GetFileNames(afterRootPath, null)
            .Where(file => !NeedExclude(fileManager.PathCombine(afterRootPath, file))).ToList();

        if (files.Count == 0)
        {
            return;
        }

        string? localAfterRootPath =
            afterRootPath?.Replace(fileManager.DirectorySeparatorChar, Path.DirectorySeparatorChar);
        string path = localAfterRootPath == null
            ? UsbCopyRunnerParameters.MainFolder
            : Path.Combine(UsbCopyRunnerParameters.MainFolder, localAfterRootPath);
        string? localPatchChecked = FileStat.CreateFolderIfNotExists(path, _useConsole, _logger);
        if (localPatchChecked == null)
        {
            StShared.WriteErrorLine($"Cannot Create Folder {path}", _useConsole, _logger);
            return;
        }

        FolderFilesModel folderFiles = GetFolderFiles(files);
        foreach (string fileName in folderFiles.Files.OrderBy(o => o))
        {
            if (localFileManager.FileExists(afterRootPath, fileName))
            {
                continue;
            }

            Console.WriteLine("File {0}", fileName);
            fileManager.DownloadFile(fileName, "dwn", afterRootPath);
        }

        foreach (KeyValuePair<string, List<BuFileInfo>> kvp in folderFiles.FileByPatterns.OrderBy(k => k.Key))
        {
            BuFileInfo fileInfo = kvp.Value.OrderByDescending(o => o.FileDateTime).First();

            if (localFileManager.FileExists(afterRootPath, fileInfo.FileName))
            {
                continue;
            }

            Console.WriteLine("Mask {0}", kvp.Key);
            Console.WriteLine("File By Mask {0}", fileInfo.FileName);
            fileManager.DownloadFile(fileInfo.FileName, "dwn", afterRootPath);
        }
    }

    private static FolderFilesModel GetFolderFiles(List<string> fileNames)
    {
        var folderFiles = new FolderFilesModel();
        foreach (string fileName in fileNames)
        {
            (DateTime dateTimeByDigits, string? pattern) =
                fileName.GetDateTimeAndPatternByDigits("yyyyMMddHHmmssfffffff");

            if (pattern != null)
            {
                if (!folderFiles.FileByPatterns.TryGetValue(pattern, out List<BuFileInfo>? value))
                {
                    value = [];
                    folderFiles.FileByPatterns.Add(pattern, value);
                }

                value.Add(new BuFileInfo(fileName, dateTimeByDigits));
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
