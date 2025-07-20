//Created by ProjectMainClassCreatorForCliAppWithMenu at 6/28/2021 12:52:58

using System;
using System.Linq;
using CliMenu;
using CliParameters.CliMenuCommands;
using CliTools;
using CliTools.CliMenuCommands;
using LibDataInput;
using LibParameters;
using Microsoft.Extensions.Logging;
using UsbCopy.Commands;
using UsbCopy.Models;

namespace UsbCopy;

public sealed class UsbCopyCliAppLoop : CliAppLoop
{
    private readonly ILogger _logger;
    private readonly ParametersManager _parametersManager;

    // ReSharper disable once ConvertToPrimaryConstructor
    public UsbCopyCliAppLoop(ILogger logger, ParametersManager parametersManager)
    {
        _logger = logger;
        _parametersManager = parametersManager;
    }

    public override CliMenuSet BuildMainMenu()
    {
        var parameters = (UsbCopyParameters)_parametersManager.Parameters;

        var mainMenuSet = new CliMenuSet("Main Menu");

        var usbCopyParametersEditor = new UsbCopyParametersEditor(parameters, _parametersManager, _logger);
        mainMenuSet.AddMenuItem(new ParametersEditorListCliMenuCommand(usbCopyParametersEditor));

        //საჭირო მენიუს ელემენტები

        var usbCopyProjectCruder = UsbCopyProjectCruder.Create(_logger, _parametersManager);

        //ახალი პროექტის შექმნა
        var newItemCommand = new NewItemCliMenuCommand(usbCopyProjectCruder, usbCopyProjectCruder.CrudNamePlural,
            $"New {usbCopyProjectCruder.CrudName}");
        mainMenuSet.AddMenuItem(newItemCommand);

        //პროექტების ჩამონათვალი
        foreach (var kvp in parameters.Projects.OrderBy(o => o.Key))
            mainMenuSet.AddMenuItem(new UsbCopyProjectSubMenuCommand(_logger, _parametersManager, kvp.Key));

        //გასასვლელი
        var key = ConsoleKey.Escape.Value().ToLower();
        mainMenuSet.AddMenuItem(key, new ExitCliMenuCommand(), key.Length);

        return mainMenuSet;
    }
}