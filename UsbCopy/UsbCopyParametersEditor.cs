//Created by ProjectParametersEditorClassCreator at 6/28/2021 12:52:58

using AppCliTools.CliParameters;
using AppCliTools.CliParameters.FieldEditors;
using AppCliTools.CliParametersEdit.Cruders;
using AppCliTools.CliParametersExcludeSetsEdit.Cruders;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibFileParameters.Models;
using ParametersManagement.LibParameters;
using UsbCopy.Models;

namespace UsbCopy;

public sealed class UsbCopyParametersEditor : ParametersEditor
{
    public UsbCopyParametersEditor(IParameters parameters, ParametersManager parametersManager, ILogger logger) : base(
        "UsbCopy Parameters Editor", parameters, parametersManager)
    {
        FieldEditors.Add(new FolderPathFieldEditor(nameof(UsbCopyParameters.LogFolder)));
        //FieldEditors.Add(new FileStoragesFieldEditor(logger, nameof(UsbCopyParameters.FileStorages),
        //    parametersManager));
        FieldEditors.Add(
            new DictionaryFieldEditor<FileStorageCruder, FileStorageData>(nameof(UsbCopyParameters.FileStorages),
                logger, parametersManager));

        //FieldEditors.Add(new ExcludeSetsFieldEditor(nameof(UsbCopyParameters.ExcludeSets), parametersManager));

        FieldEditors.Add(new DictionaryFieldEditor<ExcludeSetCruder, ExcludeSet>(nameof(UsbCopyParameters.ExcludeSets),
            logger, parametersManager));
    }
}
