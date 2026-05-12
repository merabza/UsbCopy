using AppCliTools.CliMenu;
using AppCliTools.CliParameters.CliMenuCommands;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using UsbCopy.Models;

namespace UsbCopy.Menu.UsbCopyParametersEdit;

// ReSharper disable once ClassNeverInstantiated.Global
public class UsbCopyParametersEditorListCliMenuCommandFactoryStrategy : IMenuCommandFactoryStrategy
{
    private readonly ILogger<UsbCopyParametersEditorListCliMenuCommandFactoryStrategy> _logger;
    private readonly IParametersManager _parametersManager;

    public UsbCopyParametersEditorListCliMenuCommandFactoryStrategy(
        ILogger<UsbCopyParametersEditorListCliMenuCommandFactoryStrategy> logger, IParametersManager parametersManager)
    {
        _logger = logger;
        _parametersManager = parametersManager;
    }

    public CliMenuCommand CreateMenuCommand()
    {
        var parameters = (UsbCopyParameters)_parametersManager.Parameters;

        var usbCopyParametersEditor = new UsbCopyParametersEditor(parameters, _parametersManager, _logger);
        return new ParametersEditorListCliMenuCommand(usbCopyParametersEditor);
    }
}
