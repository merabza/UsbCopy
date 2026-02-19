using SystemTools.SystemToolsShared;

namespace UsbCopy.Models;

public sealed class UsbCopyProjectModel : ItemData
{
    public string? LocalPath { get; set; }
    public string? FileStorageName { get; set; }
    public string? ExcludeSetName { get; set; }
}
