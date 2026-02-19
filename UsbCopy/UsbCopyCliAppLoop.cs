//Created by ProjectMainClassCreatorForCliAppWithMenu at 6/28/2021 12:52:58

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AppCliTools.CliMenu;
using AppCliTools.CliParameters.CliMenuCommands;
using AppCliTools.CliTools;
using AppCliTools.CliTools.CliMenuCommands;
using AppCliTools.LibDataInput;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
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
        foreach (KeyValuePair<string, UsbCopyProjectModel> kvp in parameters.Projects.OrderBy(o => o.Key))
        {
            mainMenuSet.AddMenuItem(new UsbCopyProjectSubMenuCommand(_logger, _parametersManager, kvp.Key));
        }

        //გასასვლელი
        string key = ConsoleKey.Escape.Value().ToLower(CultureInfo.CurrentCulture);
        mainMenuSet.AddMenuItem(key, new ExitCliMenuCommand(), key.Length);

        return mainMenuSet;
    }
}
