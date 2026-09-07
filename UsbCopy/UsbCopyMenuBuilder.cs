using System;
using System.Threading.Tasks;
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

    public Task<CliMenuSet?> BuildMainMenu()
    {
        //მთავარი მენიუს ჩატვირთვა
        return Task.FromResult(CliMenuSetFactory.CreateMenuSet("Main Menu",
            MenuData.MainMenuCommandFactoryStrategyNames, _serviceProvider, true));
    }
}
