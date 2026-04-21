using AppCliTools.CliTools.Models;
using Microsoft.Extensions.Options;
using SystemTools.SystemToolsShared;

namespace UsbCopy;

public class UsbCopyApplication : IApplication
{
    public UsbCopyApplication(IOptions<ApplicationOptions> options)
    {
        AppName = options.Value.AppName;
    }

    public string AppName { get; }
}
