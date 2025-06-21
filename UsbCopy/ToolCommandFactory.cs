using LibParameters;
using Microsoft.Extensions.Logging;
using SystemToolsShared;
using UsbCopy.Models;

namespace UsbCopy;

public static class ToolCommandFactory
{
    public static readonly ETools[] ToolsByProjects =
    [
        ETools.CopyFiles
    ];

    public static IToolCommand? Create(ILogger logger, ETools tool, string projectName,
        IParametersManager parametersManager)
    {
        switch (tool)
        {
            case ETools.CopyFiles:
                var usbCopyParameters = (UsbCopyParameters)parametersManager.Parameters;

                var usbCopyRunnerParameters = UsbCopyRunnerParameters.Create(logger, usbCopyParameters, projectName);
                if (usbCopyRunnerParameters is not null)
                    return new UsbCopyRunner(logger, true, usbCopyRunnerParameters, parametersManager);
                StShared.WriteErrorLine("UsbCopyRunnerParameters does not created", true);
                return null;
            default:
                return null;
        }
    }
}