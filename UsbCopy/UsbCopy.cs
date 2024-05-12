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

public sealed class UsbCopy : CliAppLoop
{
    private readonly ILogger _logger;
    private readonly ParametersManager _parametersManager;

    // ReSharper disable once ConvertToPrimaryConstructor
    public UsbCopy(ILogger logger, ParametersManager parametersManager)
    {
        _logger = logger;
        _parametersManager = parametersManager;
    }

    protected override void BuildMainMenu()
    {
        var parameters = (UsbCopyParameters)_parametersManager.Parameters;

        //if (parameters == null)
        //{
        //    StShared.WriteErrorLine("minimal parameters not found", true);
        //    return false;
        //}

        CliMenuSet mainMenuSet = new("Main Menu");
        AddChangeMenu(mainMenuSet);

        UsbCopyParametersEditor usbCopyParametersEditor = new(parameters, _parametersManager, _logger);
        mainMenuSet.AddMenuItem(new ParametersEditorListCliMenuCommand(usbCopyParametersEditor),
            "UsbCopy Parameters Editor");

        //საჭირო მენიუს ელემენტები

        UsbCopyProjectCruder usbCopyProjectCruder = new(_logger, _parametersManager);

        //ახალი პროექტის შექმნა
        NewItemCliMenuCommand newItemCommand = new(usbCopyProjectCruder, usbCopyProjectCruder.CrudNamePlural,
            $"New {usbCopyProjectCruder.CrudName}");
        mainMenuSet.AddMenuItem(newItemCommand);

        //პროექტების ჩამონათვალი
        foreach (var kvp in parameters.Projects.OrderBy(o => o.Key))
            mainMenuSet.AddMenuItem(new UsbCopyProjectSubMenuCommand(_logger, _parametersManager, kvp.Key), kvp.Key);

        //გასასვლელი
        var key = ConsoleKey.Escape.Value().ToLower();
        mainMenuSet.AddMenuItem(key, "Exit", new ExitCliMenuCommand(), key.Length);
    }
}