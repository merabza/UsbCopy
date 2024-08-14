using System;
using System.Threading;
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

    // ReSharper disable once ConvertToPrimaryConstructor
    public ToolTaskCommand(ILogger logger, ETools tool, string projectName, IParametersManager parametersManager) :
        base(tool.ToString(), EMenuAction.Reload)
    {
        _logger = logger;
        _tool = tool;
        _projectName = projectName;
        _parametersManager = parametersManager;
    }

    protected override bool RunBody()
    {
        var toolCommand = ToolCommandFabric.Create(_logger, _tool, _projectName, _parametersManager);

        if (toolCommand?.Par != null)
            return toolCommand.Run(CancellationToken.None).Result;

        Console.WriteLine("Parameters not loaded. Tool not started.");
        StShared.Pause();
        return false;
    }
}