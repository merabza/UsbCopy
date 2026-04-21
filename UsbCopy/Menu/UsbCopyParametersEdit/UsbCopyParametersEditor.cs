using AppCliTools.CliParameters;
using AppCliTools.CliParameters.FieldEditors;
using AppCliTools.CliParametersEdit.Cruders;
using AppCliTools.CliParametersExcludeSetsEdit.Cruders;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibFileParameters.Models;
using ParametersManagement.LibParameters;
using UsbCopy.Models;

namespace UsbCopy.Menu.UsbCopyParametersEdit;

public sealed class UsbCopyParametersEditor : ParametersEditor
{
    public UsbCopyParametersEditor(IParameters parameters, IParametersManager parametersManager, ILogger logger) : base(
        "UsbCopy Parameters Editor", parameters, parametersManager)
    {
        FieldEditors.Add(new FolderPathFieldEditor(nameof(UsbCopyParameters.LogFolder)));
        FieldEditors.Add(
            new DictionaryFieldEditor<FileStorageCruder, FileStorageData>(nameof(UsbCopyParameters.FileStorages),
                logger, parametersManager));
        FieldEditors.Add(new DictionaryFieldEditor<ExcludeSetCruder, ExcludeSet>(nameof(UsbCopyParameters.ExcludeSets),
            logger, parametersManager));
    }
}
