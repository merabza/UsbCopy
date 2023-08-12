using System;
using System.Collections.Generic;
using System.Linq;
using CliParameters;
using CliParameters.FieldEditors;
using CliParametersEdit.FieldEditors;
using CliParametersExcludeSetsEdit.FieldEditors;
using LibParameters;
using Microsoft.Extensions.Logging;
using UsbCopy.Models;

namespace UsbCopy;

public sealed class UsbCopyProjectCruder : ParCruder
{
    public UsbCopyProjectCruder(ILogger logger, ParametersManager parametersManager) : base(parametersManager,
        "Project",
        "Projects")
    {
        FieldEditors.Add(new FolderPathFieldEditor(nameof(UsbCopyProjectModel.LocalPath)));
        FieldEditors.Add(new FileStorageNameFieldEditor(logger, nameof(UsbCopyProjectModel.FileStorageName),
            parametersManager));
        FieldEditors.Add(new ExcludeSetNameFieldEditor(nameof(UsbCopyProjectModel.ExcludeSetName),
            parametersManager));
    }

    protected override Dictionary<string, ItemData> GetCrudersDictionary()
    {
        var parameters = (UsbCopyParameters)ParametersManager.Parameters;
        return parameters.Projects.ToDictionary(p => p.Key, p => (ItemData)p.Value);
    }

    public override bool ContainsRecordWithKey(string recordKey)
    {
        var parameters = (UsbCopyParameters)ParametersManager.Parameters;
        var projects = parameters.Projects;
        return projects.ContainsKey(recordKey);
    }

    public override void UpdateRecordWithKey(string recordName, ItemData newRecord)
    {
        if (newRecord is not UsbCopyProjectModel newProject)
            throw new Exception("newProject is null in UsbCopyProjectCruder.UpdateRecordWithKey");

        var parameters = (UsbCopyParameters)ParametersManager.Parameters;
        parameters.Projects[recordName] = newProject;
    }

    protected override void AddRecordWithKey(string recordName, ItemData newRecord)
    {
        if (newRecord is not UsbCopyProjectModel newProject)
            throw new Exception("newProject is null in UsbCopyProjectCruder.AddRecordWithKey");

        var parameters = (UsbCopyParameters)ParametersManager.Parameters;
        parameters.Projects.Add(recordName, newProject);
    }

    protected override void RemoveRecordWithKey(string recordKey)
    {
        var parameters = (UsbCopyParameters)ParametersManager.Parameters;
        var projects = parameters.Projects;
        projects.Remove(recordKey);
    }

    protected override ItemData CreateNewItem(ItemData? defaultItemData)
    {
        return new UsbCopyProjectModel();
    }

    //public override void FillDetailsSubMenu(CliMenuSet itemSubMenuSet, string recordName)
    //{
    //    base.FillDetailsSubMenu(itemSubMenuSet, recordName);

    //    UsbCopyParameters parameters = (UsbCopyParameters)ParametersManager.Parameters;
    //    Dictionary<string, UsbCopyProjectModel> projects = parameters.Projects;
    //    UsbCopyProjectModel project = projects[recordName];

    //    RedundantFileNameCruder detailsCruder = new(ParametersManager, recordName);
    //    NewItemCommand newItemCommand = new(detailsCruder, recordName, $"Create New {detailsCruder.CrudName}");
    //    itemSubMenuSet.AddMenuItem(newItemCommand);

    //    if (project.RedundantFileNames == null)
    //        return;

    //    foreach (ItemSubMenuCommand detailListCommand in project.RedundantFileNames.Select(mask =>
    //                 new ItemSubMenuCommand(detailsCruder, mask, recordName, true)))
    //    {
    //        itemSubMenuSet.AddMenuItem(detailListCommand);
    //    }
    //}
}