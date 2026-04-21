using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using SystemTools.SystemToolsShared;
using UsbCopy.Models;

namespace UsbCopy.ToolCommands;

public static class ToolCommandFactory
{
    public static IToolCommand? Create(ILogger logger, string projectName, IParametersManager parametersManager)
    {
        var usbCopyParameters = (UsbCopyParameters)parametersManager.Parameters;

        var usbCopyRunnerParameters = UsbCopyRunnerParameters.Create(logger, usbCopyParameters, projectName);
        if (usbCopyRunnerParameters is not null)
        {
            return new UsbCopyRunnerToolCommand(logger, true, usbCopyRunnerParameters, parametersManager);
        }

        StShared.WriteErrorLine("UsbCopyRunnerParameters does not created", true);
        return null;
    }
}
