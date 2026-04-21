using AppCliTools.CliTools.Models;
using Microsoft.Extensions.Options;
using SystemTools.SystemToolsShared;

namespace UsbCopy;

public class UsbCopyApplication : IApplication
{
    private readonly string _appName;

    public UsbCopyApplication(IOptions<ApplicationOptions> options)
    {
        _appName = options.Value.AppName;
    }

    public string AppName => _appName;
}
