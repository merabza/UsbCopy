using AppCliTools.CliMenu;
using AppCliTools.CliParameters.CliMenuCommands;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using UsbCopy.Models;

namespace UsbCopy.Menu.UsbCopyParametersEdit;

public class UsbCopyParametersEditorListCliMenuCommandFactoryStrategy : IMenuCommandFactoryStrategy
{
    private readonly ILogger<UsbCopyParametersEditorListCliMenuCommandFactoryStrategy> _logger;

    // ReSharper disable once ConvertToPrimaryConstructor
    public UsbCopyParametersEditorListCliMenuCommandFactoryStrategy(
        ILogger<UsbCopyParametersEditorListCliMenuCommandFactoryStrategy> logger)
    {
        _logger = logger;
    }

    public string StrategyName => nameof(UsbCopyParametersEditorListCliMenuCommandFactoryStrategy);

    public CliMenuCommand CreateMenuCommand(IParametersManager parametersManager)
    {
        var parameters = (UsbCopyParameters)parametersManager.Parameters;

        var usbCopyParametersEditor = new UsbCopyParametersEditor(parameters, parametersManager, _logger);
        return new ParametersEditorListCliMenuCommand(usbCopyParametersEditor);
    }
}
