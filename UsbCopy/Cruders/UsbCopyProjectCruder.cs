using System.Collections.Generic;
using AppCliTools.CliParameters;
using AppCliTools.CliParameters.FieldEditors;
using AppCliTools.CliParametersEdit.FieldEditors;
using AppCliTools.CliParametersExcludeSetsEdit.FieldEditors;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using UsbCopy.Models;

namespace UsbCopy.Cruders;

public sealed class UsbCopyProjectCruder : ParCruder<UsbCopyProjectModel>
{
    private UsbCopyProjectCruder(ILogger logger, IParametersManager parametersManager,
        Dictionary<string, UsbCopyProjectModel> currentValuesDictionary) : base(parametersManager,
        currentValuesDictionary, "Project", "Projects")
    {
        FieldEditors.Add(new FolderPathFieldEditor(nameof(UsbCopyProjectModel.LocalPath)));
        FieldEditors.Add(new FileStorageNameFieldEditor(logger, nameof(UsbCopyProjectModel.FileStorageName),
            parametersManager));
        FieldEditors.Add(new ExcludeSetNameFieldEditor(nameof(UsbCopyProjectModel.ExcludeSetName), parametersManager));
    }

    public static UsbCopyProjectCruder Create(ILogger logger, IParametersManager parametersManager)
    {
        var parameters = (UsbCopyParameters)parametersManager.Parameters;
        return new UsbCopyProjectCruder(logger, parametersManager, parameters.Projects);
    }
}
