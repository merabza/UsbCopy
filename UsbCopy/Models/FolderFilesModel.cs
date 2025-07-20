using System.Collections.Generic;
using SystemToolsShared;

namespace UsbCopy.Models;

public sealed class FolderFilesModel
{
    public Dictionary<string, List<BuFileInfo>> FileByPatterns { get; set; } = new();
    public List<string> Files { get; set; } = [];
}