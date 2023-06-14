//Created by ProjectMainClassCreatorForCliAppWithMenu at 6/28/2021 12:52:58

using System;
using System.Linq;
using CliMenu;
using CliParameters.MenuCommands;
using CliTools;
using CliTools.Commands;
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

    public UsbCopy(ILogger logger, ParametersManager parametersManager)
    {
        _logger = logger;
        _parametersManager = parametersManager;
    }

    protected override bool BuildMainMenu()
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
        mainMenuSet.AddMenuItem(new ParametersEditorListCommand(usbCopyParametersEditor),
            "UsbCopy Parameters Editor");

        //საჭირო მენიუს ელემენტები

        //ტასკების განზოგადებული ვარიანტი
        //UsbCopyTasksEditor usbCopyTasksEditor = new UsbCopyTasksEditor(_logger, _parametersManager);
        //usbCopyTasksEditor.TaskMenuElements(mainMenuSet, parameters);

        //TasksEditorMenu<ETools> supportToolsTasksEditor = new TasksEditorMenu<ETools>(_logger, _parametersManager);
        //supportToolsTasksEditor.TaskMenuElements(mainMenuSet);

        UsbCopyProjectCruder usbCopyProjectCruder = new(_logger, _parametersManager);

        //ახალი პროექტის შექმნა
        NewItemCommand newItemCommand =
            new(usbCopyProjectCruder, usbCopyProjectCruder.CrudNamePlural, $"New {usbCopyProjectCruder.CrudName}");
        mainMenuSet.AddMenuItem(newItemCommand);

        //პროექტების ჩამონათვალი
        foreach (var kvp in parameters.Projects.OrderBy(o => o.Key))
            mainMenuSet.AddMenuItem(new UsbCopyProjectSubMenuCommand(_logger, _parametersManager, kvp.Key), kvp.Key);

        //გასასვლელი
        var key = ConsoleKey.Escape.Value().ToLower();
        mainMenuSet.AddMenuItem(key, "Exit", new ExitCommand(), key.Length);

        return true;
    }
}