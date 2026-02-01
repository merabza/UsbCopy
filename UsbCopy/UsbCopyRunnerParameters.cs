using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppCliTools.LibDataInput;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibFileParameters.Models;
using ParametersManagement.LibParameters;
using SystemTools.SystemToolsShared;
using ToolsManagement.FileManagersMain;
using UsbCopy.Models;

namespace UsbCopy;

public sealed class UsbCopyRunnerParameters : IParameters
{
    private UsbCopyRunnerParameters(FileManager mainFolderFileManager, FileManager fileManager, string mainFolder,
        string[] excludes)
    {
        FileManager = fileManager;
        MainFolderFileManager = mainFolderFileManager;
        MainFolder = mainFolder;
        Excludes = excludes;
    }

    public FileManager FileManager { get; }
    public FileManager MainFolderFileManager { get; }
    public string MainFolder { get; }
    public string[] Excludes { get; }

    public bool CheckBeforeSave()
    {
        return true;
    }

    internal static UsbCopyRunnerParameters? Create(ILogger logger, UsbCopyParameters usbCopyParameters,
        string projectName)
    {
        var project = usbCopyParameters.GetProjectRequired(projectName);

        //var drives = DriveInfo.GetDrives()
        //    .Where(drive => drive.IsReady && drive.DriveType == DriveType.Removable).ToList();

        //if (drives.Count == 0)
        //    Console.WriteLine("No Removable drives found");
        ////return true;

        //Console.WriteLine($"Found {drives.Count} Removable drives.");
        //foreach (var driveInfo in drives) Console.WriteLine(driveInfo.RootDirectory.FullName);

        //Check LocalPath
        if (string.IsNullOrWhiteSpace(project.LocalPath))
        {
            StShared.WriteErrorLine($"LocalPath does not specified for Project {projectName}", true, logger);
            return null;
        }

        var localPathChecked = FileStat.CreateFolderIfNotExists(project.LocalPath, true, logger);
        if (localPathChecked == null)
        {
            StShared.WriteErrorLine($"local path {project.LocalPath} can not be created", true, logger);
            return null;
        }

        var fileStorages = new FileStorages(usbCopyParameters.FileStorages);

        //Check FileStorage
        if (string.IsNullOrWhiteSpace(project.FileStorageName))
        {
            StShared.WriteErrorLine("File Storage does not specified", true, logger);
            return null;
        }

        var fileStorage = fileStorages.GetFileStorageDataByKey(project.FileStorageName);

        if (fileStorage == null)
        {
            StShared.WriteErrorLine("File Storage not found", true, logger);
            return null;
        }

        var localFileManager = FileManagersFactory.CreateFileManager(true, logger, project.LocalPath);

        if (localFileManager == null)
        {
            StShared.WriteErrorLine("localFileManager does not created", true, logger);
            return null;
        }

        var folderNames = localFileManager.GetFolderNames(string.Empty, null);

        const string folderMask = "yyyyMMddHHmmss";
        List<BuFileInfo> buFileInfos = [];

        foreach (var folderName in folderNames)
        {
            var (dateTimeByDigits, pattern) = folderName.GetDateTimeAndPatternByDigits(folderMask);
            if (pattern is not null)
                buFileInfos.Add(new BuFileInfo(folderName, dateTimeByDigits));
        }

        BuFileInfo? lastFolderName = null;
        if (buFileInfos.Count > 0) lastFolderName = buFileInfos.MaxBy(ob => ob.FileDateTime);

        if (lastFolderName != null)
            if (!Inputer.InputBool($"Continue with existing folder {lastFolderName.FileName}", false, false))
                lastFolderName = null;

        var mainFolderName = lastFolderName == null ? DateTime.Now.ToString(folderMask) : lastFolderName.FileName;
        var mainFolderFullPath = Path.Combine(project.LocalPath, mainFolderName);

        var mainFolder = FileStat.CreateFolderIfNotExists(mainFolderFullPath, true, logger);
        if (mainFolder is null)
        {
            StShared.WriteErrorLine($"Main folder path {mainFolderFullPath} can not be created", true, logger);
            return null;
        }

        var fileManager = FileManagersFactoryExt.CreateFileManager(true, logger, mainFolder, fileStorage);

        if (fileManager == null)
        {
            StShared.WriteErrorLine("fileManager does not created", true, logger);
            return null;
        }

        var excludes = Array.Empty<string>();
        //Check ExcludeSet
        if (!string.IsNullOrWhiteSpace(project.ExcludeSetName))
        {
            var excludeSets = new ExcludeSets(usbCopyParameters.ExcludeSets);
            var excludeSet = excludeSets.GetExcludeSetByKey(project.ExcludeSetName);

            if (excludeSet is null)
            {
                StShared.WriteErrorLine("Exclude Set does not found", true, logger);
                return null;
            }

            if (excludeSet.FolderFileMasks is { Count: > 0 })
                excludes = excludeSet.FolderFileMasks
                    .Select(s => s.Replace(Path.DirectorySeparatorChar, fileManager.DirectorySeparatorChar)).ToArray();
        }

        var mainFolderFileManager = FileManagersFactory.CreateFileManager(true, logger, mainFolder);

        if (mainFolderFileManager == null)
        {
            StShared.WriteErrorLine("mainFolderFileManager does not created", true, logger);
            return null;
        }

        var usbCopyRunnerParameters = new UsbCopyRunnerParameters(
            //project.LocalPath, fileStorage, 
            mainFolderFileManager, fileManager, mainFolder, excludes);
        return usbCopyRunnerParameters;
    }
}