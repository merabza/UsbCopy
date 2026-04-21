using System;
using System.Threading;
using System.Threading.Tasks;
using AppCliTools.CliMenu;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using SystemTools.SystemToolsShared;
using UsbCopy.ToolCommands;

namespace UsbCopy.Commands;

public sealed class CopyFilesCliMenuCommand : CliMenuCommand
{
    private readonly ILogger _logger;
    private readonly IParametersManager _parametersManager;
    private readonly string _projectName;

    // ReSharper disable once ConvertToPrimaryConstructor
    public CopyFilesCliMenuCommand(ILogger logger, string projectName, IParametersManager parametersManager) : base(
        "CopyFiles", EMenuAction.Reload)
    {
        _logger = logger;
        _projectName = projectName;
        _parametersManager = parametersManager;
    }

    protected override async ValueTask<bool> RunBody(CancellationToken cancellationToken = default)
    {
        IToolCommand? toolCommand = ToolCommandFactory.Create(_logger, _projectName, _parametersManager);

        if (toolCommand?.Par != null)
        {
            return await toolCommand.Run(cancellationToken);
        }

        Console.WriteLine("Parameters not loaded. Tool not started.");
        StShared.Pause();
        return false;
    }
}
