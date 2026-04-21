using System;
using AppCliTools.CliMenu;
using AppCliTools.CliTools.Services.MenuBuilder;
using ParametersManagement.LibParameters;
using UsbCopy.Menu;

namespace UsbCopy;

public class UsbCopyMenuBuilder : IMenuBuilder
{
    private readonly IParametersManager _parametersManager;
    private readonly IServiceProvider _serviceProvider;

    // ReSharper disable once ConvertToPrimaryConstructor
    public UsbCopyMenuBuilder(IServiceProvider serviceProvider, IParametersManager parametersManager)
    {
        _serviceProvider = serviceProvider;
        _parametersManager = parametersManager;
    }

    public CliMenuSet BuildMainMenu()
    {
        //მთავარი მენიუს ჩატვირთვა
        return CliMenuSetFactory.CreateMenuSet("Main Menu", MenuData.MenuCommandNames, _serviceProvider,
            _parametersManager, true);
    }
}
