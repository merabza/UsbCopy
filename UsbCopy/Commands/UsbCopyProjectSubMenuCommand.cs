using System;
using CliMenu;
using CliParameters.CliMenuCommands;
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

    // ReSharper disable once ConvertToPrimaryConstructor
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
        EditItemAllFieldsInSequenceCliMenuCommand editCommand = new(projectCruder, _projectName);
        projectSubMenuSet.AddMenuItem(editCommand, "Edit All fields in sequence");

        projectCruder.FillDetailsSubMenu(projectSubMenuSet, _projectName);

        var project = parameters.GetProject(_projectName);

        if (project != null)
            foreach (var tool in ToolCommandFabric.ToolsByProjects)
                projectSubMenuSet.AddMenuItem(new ToolTaskCommand(_logger, tool, _projectName, _parametersManager),
                    tool.ToString());

        //მთავარ მენიუში გასვლა
        var key = ConsoleKey.Escape.Value().ToLower();
        projectSubMenuSet.AddMenuItem(key, "Exit to Main menu", new ExitToMainMenuCliMenuCommand(null, null),
            key.Length);

        return projectSubMenuSet;
    }
}