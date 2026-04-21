using System.Collections.Generic;

namespace UsbCopy.Menu;

public static class MenuData
{
    public static List<string> MenuCommandNames { get; } =
    [
        UsbCopyParametersEditor.MenuCommandName
    ];
}
