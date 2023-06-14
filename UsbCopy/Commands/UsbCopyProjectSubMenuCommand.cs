using System;
using CliMenu;
using CliParameters.MenuCommands;
using LibDataInput;
using LibParameters;
using Microsoft.Extensions.Logging;
using UsbCopy.Models;

namespace UsbCopy.Commands;

public sealed class UsbCopyProjectSubMenuCommand : CliMenuCommand
{
    private readonly ILogger _logger;
    private readonly ParametersManager _parametersManager;

    private readonly string _projectName;

    public UsbCopyProjectSubMenuCommand(ILogger logger, ParametersManager parametersManager, string projectName) : base(
        projectName)
    {
        _logger = logger;
        _parametersManager = parametersManager;
        _projectName = projectName;
    }

    protected override void RunAction()
    {
        MenuAction = EMenuAction.LoadSubMenu;
    }


    public override CliMenuSet GetSubmenu()
    {
        CliMenuSet projectSubMenuSet = new($"Project => {_projectName}");

        var parameters = (UsbCopyParameters)_parametersManager.Parameters;

        //პროექტის წაშლა
        DeleteUsbCopyProjectCommand deleteProjectCommand = new(_parametersManager, _projectName);
        projectSubMenuSet.AddMenuItem(deleteProjectCommand);

        //პროექტის პარამეტრი
        UsbCopyProjectCruder projectCruder = new(_logger, _parametersManager);
        EditItemAllFieldsInSequenceCommand editCommand = new(projectCruder, _projectName);
        projectSubMenuSet.AddMenuItem(editCommand, "Edit All fields in sequence");

        projectCruder.FillDetailsSubMenu(projectSubMenuSet, _projectName);

        //projectSubMenuSet.AddMenuItem(new GitSubMenuCommand(_logger, _parametersManager, _projectName, EGitCol.Main),
        //    "Git");

        var project = parameters.GetProject(_projectName);

        if (project != null)
            foreach (var tool in ToolCommandFabric.ToolsByProjects)
                projectSubMenuSet.AddMenuItem(new ToolTaskCommand(_logger, tool, _projectName, _parametersManager),
                    tool.ToString());

        //ServerInfoCruder serverInfoCruder = new(_logger, _parametersManager, _projectName);

        ////ახალი სერვერის ინფორმაციის შექმნა
        //NewItemCommand newItemCommand =
        //    new(serverInfoCruder, serverInfoCruder.CrudNamePlural, $"New {serverInfoCruder.CrudName}");
        //projectSubMenuSet.AddMenuItem(newItemCommand);

        ////სერვერების ჩამონათვალი
        //if (project?.ServerInfos != null)
        //{
        //    foreach (KeyValuePair<string, ServerInfoModel> kvp in project.ServerInfos.OrderBy(o => o.Key))
        //        projectSubMenuSet.AddMenuItem(
        //            new ServerInfoSubMenuCommand(_logger, _parametersManager, _projectName, kvp.Key), kvp.Key);
        //}

        //მთავარ მენიუში გასვლა
        var key = ConsoleKey.Escape.Value().ToLower();
        projectSubMenuSet.AddMenuItem(key, "Exit to Main menu", new ExitToMainMenuCommand(null, null), key.Length);

        return projectSubMenuSet;
    }
}