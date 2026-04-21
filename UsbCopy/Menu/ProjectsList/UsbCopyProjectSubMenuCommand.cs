using System;
using System.Globalization;
using AppCliTools.CliMenu;
using AppCliTools.CliMenu.CliMenuCommands;
using AppCliTools.CliParameters.CliMenuCommands;
using AppCliTools.LibDataInput;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using UsbCopy.Commands;
using UsbCopy.Cruders;
using UsbCopy.Models;

namespace UsbCopy.Menu.ProjectsList;

public sealed class UsbCopyProjectSubMenuCommand : CliMenuCommand
{
    private readonly ILogger _logger;
    private readonly IParametersManager _parametersManager;

    private readonly string _projectName;

    // ReSharper disable once ConvertToPrimaryConstructor
    public UsbCopyProjectSubMenuCommand(ILogger logger, IParametersManager parametersManager, string projectName) :
        base(projectName, EMenuAction.LoadSubMenu)
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
            projectSubMenuSet.AddMenuItem(new CopyFilesCliMenuCommand(_logger, _projectName, _parametersManager));
        }

        //მთავარ მენიუში გასვლა
        string key = ConsoleKey.Escape.Value().ToLower(CultureInfo.CurrentCulture);
        projectSubMenuSet.AddMenuItem(key, new ExitToMainMenuCliMenuCommand("Exit to Main menu", null), key.Length);

        return projectSubMenuSet;
    }
}
