﻿using System;
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
        projectName, EMenuAction.LoadSubMenu)
    {
        _logger = logger;
        _parametersManager = parametersManager;
        _projectName = projectName;
    }

    public override CliMenuSet GetSubMenu()
    {
        CliMenuSet projectSubMenuSet = new($"Project => {_projectName}");

        var parameters = (UsbCopyParameters)_parametersManager.Parameters;

        //პროექტის წაშლა
        DeleteUsbCopyProjectCommand deleteProjectCommand = new(_parametersManager, _projectName);
        projectSubMenuSet.AddMenuItem(deleteProjectCommand);

        //პროექტის პარამეტრი
        UsbCopyProjectCruder projectCruder = new(_logger, _parametersManager);
        EditItemAllFieldsInSequenceCliMenuCommand editCommand = new(projectCruder, _projectName);
        projectSubMenuSet.AddMenuItem(editCommand);

        projectCruder.FillDetailsSubMenu(projectSubMenuSet, _projectName);

        var project = parameters.GetProject(_projectName);

        if (project != null)
            foreach (var tool in ToolCommandFabric.ToolsByProjects)
                projectSubMenuSet.AddMenuItem(new ToolTaskCommand(_logger, tool, _projectName, _parametersManager));

        //მთავარ მენიუში გასვლა
        var key = ConsoleKey.Escape.Value().ToLower();
        projectSubMenuSet.AddMenuItem(key, new ExitToMainMenuCliMenuCommand("Exit to Main menu", null), key.Length);

        return projectSubMenuSet;
    }
}