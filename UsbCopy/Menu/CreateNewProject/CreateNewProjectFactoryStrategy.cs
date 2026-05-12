using AppCliTools.CliMenu;
using AppCliTools.CliParameters.CliMenuCommands;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using UsbCopy.Cruders;

namespace UsbCopy.Menu.CreateNewProject;

// ReSharper disable once ClassNeverInstantiated.Global
public class CreateNewProjectFactoryStrategy : IMenuCommandFactoryStrategy
{
    private readonly ILogger<CreateNewProjectFactoryStrategy> _logger;
    private readonly IParametersManager _parametersManager;

    public CreateNewProjectFactoryStrategy(ILogger<CreateNewProjectFactoryStrategy> logger,
        IParametersManager parametersManager)
    {
        _logger = logger;
        _parametersManager = parametersManager;
    }

    public CliMenuCommand CreateMenuCommand()
    {
        var usbCopyProjectCruder = UsbCopyProjectCruder.Create(_logger, _parametersManager);

        //ახალი პროექტის შექმნა
        return new NewItemCliMenuCommand(usbCopyProjectCruder, usbCopyProjectCruder.CrudNamePlural,
            $"New {usbCopyProjectCruder.CrudName}");
    }
}
