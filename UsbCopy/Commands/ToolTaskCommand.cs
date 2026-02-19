using System;
using System.Threading;
using System.Threading.Tasks;
using AppCliTools.CliMenu;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using SystemTools.SystemToolsShared;

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

    protected override async ValueTask<bool> RunBody(CancellationToken cancellationToken = default)
    {
        var toolCommand = ToolCommandFactory.Create(_logger, _tool, _projectName, _parametersManager);

        if (toolCommand?.Par != null)
        {
            return await toolCommand.Run(cancellationToken);
        }

        Console.WriteLine("Parameters not loaded. Tool not started.");
        StShared.Pause();
        return false;
    }
}
