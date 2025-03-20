//Created by ProjectParametersClassCreator at 6/28/2021 12:52:58

using System;
using System.Collections.Generic;
using LibFileParameters.Interfaces;
using LibFileParameters.Models;

namespace UsbCopy.Models;

public sealed class UsbCopyParameters : IParametersWithFileStorages, IParametersWithExcludeSets
{
    public string? LogFolder { get; set; }
    public Dictionary<string, UsbCopyProjectModel> Projects { get; init; } = new();

    public Dictionary<string, ExcludeSet> ExcludeSets { get; init; } = new();
    //public string? LogFileName { get; set; }

    public Dictionary<string, FileStorageData> FileStorages { get; init; } = new();

    public bool CheckBeforeSave()
    {
        return true;
    }

    public UsbCopyProjectModel? GetProject(string projectName)
    {
        return !Projects.ContainsKey(projectName) ? null : Projects[projectName];
    }

    public UsbCopyProjectModel GetProjectRequired(string projectName)
    {
        return GetProject(projectName) ??
               throw new InvalidOperationException($"project with name {projectName} does not found");
    }
}