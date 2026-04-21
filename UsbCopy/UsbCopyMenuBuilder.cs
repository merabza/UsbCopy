using System;
using AppCliTools.CliMenu;
using AppCliTools.CliTools.Services.MenuBuilder;
using UsbCopy.Menu;

namespace UsbCopy;

public class UsbCopyMenuBuilder : IMenuBuilder
{
    private readonly IServiceProvider _serviceProvider;

    // ReSharper disable once ConvertToPrimaryConstructor
    public UsbCopyMenuBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public CliMenuSet BuildMainMenu()
    {
        //მთავარი მენიუს ჩატვირთვა
        return CliMenuSetFactory.CreateMenuSet("Main Menu", MenuData.MainMenuCommandFactoryStrategyNames,
            _serviceProvider, true);
    }
}
