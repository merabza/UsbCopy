using System;
using CliMenu;
using LibParameters;
using Microsoft.Extensions.Logging;
using SystemToolsShared;

namespace UsbCopy.Commands;

public sealed class ToolTaskCommand : CliMenuCommand
{
    private readonly ILogger _logger;
    private readonly IParametersManager _parametersManager;
    private readonly string _projectName;
    private readonly ETools _tool;

    public ToolTaskCommand(ILogger logger, ETools tool, string projectName,
        IParametersManager parametersManager)
    {
        _logger = logger;
        _tool = tool;
        _projectName = projectName;
        _parametersManager = parametersManager;
    }


    protected override void RunAction()
    {
        var toolCommand = ToolCommandFabric.Create(_logger, _tool, _projectName, _parametersManager);

        if (toolCommand?.Par == null)
        {
            Console.WriteLine("Parameters not loaded. Tool not started.");
            StShared.Pause();
            return;
        }

        //დავინიშნოთ დრო
        var startDateTime = DateTime.Now;

        Console.WriteLine("Tools is running...");
        Console.WriteLine("---");

        toolCommand.Run();

        Console.WriteLine("---");

        Console.WriteLine($"Tool Finished. {StShared.TimeTakenMessage(startDateTime)}");


        StShared.Pause();
        MenuAction = EMenuAction.Reload;
    }
}