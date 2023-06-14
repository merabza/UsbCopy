//Created by ProjectParametersEditorClassCreator at 6/28/2021 12:52:58

using CliParameters;
using CliParameters.FieldEditors;
using CliParametersEdit.FieldEditors;
using CliParametersExcludeSetsEdit.FieldEditors;
using LibParameters;
using Microsoft.Extensions.Logging;
using UsbCopy.Models;

namespace UsbCopy;

public sealed class UsbCopyParametersEditor : ParametersEditor
{
    public UsbCopyParametersEditor(IParameters parameters, ParametersManager parametersManager,
        ILogger logger) :
        base("UsbCopy Parameters Editor", parameters, parametersManager)
    {
        FieldEditors.Add(new FolderPathFieldEditor(nameof(UsbCopyParameters.LogFolder)));
        FieldEditors.Add(new FileStoragesFieldEditor(logger, nameof(UsbCopyParameters.FileStorages),
            parametersManager));
        FieldEditors.Add(new ExcludeSetsFieldEditor(nameof(UsbCopyParameters.ExcludeSets), parametersManager));
    }
}