using System;
using System.Globalization;
using AppCliTools.CliMenu;
using AppCliTools.CliParameters.CliMenuCommands;
using AppCliTools.LibDataInput;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using UsbCopy.Models;

namespace UsbCopy.Commands;

public sealed class UsbCopyProjectSubMenuCommand : CliMenuCommand
{
    private readonly ILogger _logger;
    private readonly ParametersManager _parametersManager;

    private readonly string _projectName;

    // ReSharper disable once ConvertToPrimaryConstructor
    public UsbCopyProjectSubMenuCommand(ILogger logger, ParametersManager parametersManager, string projectName) : base(
        projectName, EMenuAction.LoadSubMenu)
    {
        _logger = logger;
        _parametersManager = parametersManager;
        _projectName = projectName;
    }

    public override CliMenuSet GetSubMenu()
    {
        var projectSubMenuSet = new CliMenuSet($"Project => {_projectName}");

        var parameters = (UsbCopyParameters)_parametersManager.Parameters;

        //პროექტის წაშლა
        var deleteProjectCommand = new DeleteUsbCopyProjectCommand(_parametersManager, _projectName);
        projectSubMenuSet.AddMenuItem(deleteProjectCommand);

        //პროექტის პარამეტრი
        var projectCruder = UsbCopyProjectCruder.Create(_logger, _parametersManager);
        var editCommand = new EditItemAllFieldsInSequenceCliMenuCommand(projectCruder, _projectName);
        projectSubMenuSet.AddMenuItem(editCommand);

        projectCruder.FillDetailsSubMenu(projectSubMenuSet, _projectName);

        UsbCopyProjectModel? project = parameters.GetProject(_projectName);

        if (project != null)
        {
            foreach (ETools tool in ToolCommandFactory.ToolsByProjects)
            {
                projectSubMenuSet.AddMenuItem(new ToolTaskCommand(_logger, tool, _projectName, _parametersManager));
            }
        }

        //მთავარ მენიუში გასვლა
        string key = ConsoleKey.Escape.Value().ToLower(CultureInfo.CurrentCulture);
        projectSubMenuSet.AddMenuItem(key, new ExitToMainMenuCliMenuCommand("Exit to Main menu", null), key.Length);

        return projectSubMenuSet;
    }
}
