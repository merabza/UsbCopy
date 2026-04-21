using System.Collections.Generic;
using UsbCopy.Menu.UsbCopyParametersEdit;

namespace UsbCopy.Menu;

public static class MenuData
{
    public static List<string> MenuCommandNames { get; } =
    [
        //ძირითადი პარამეტრების რედაქტირება
        nameof(UsbCopyParametersEditorListCliMenuCommandFactoryStrategy)
    ];
}
