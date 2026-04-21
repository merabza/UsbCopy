using System.Collections.Generic;
using System.Linq;
using AppCliTools.CliMenu;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using UsbCopy.Models;

namespace UsbCopy.Menu.ProjectsList;

public class ProjectsListFactoryStrategy : IMenuCommandListFactoryStrategy
{
    private readonly ILogger<ProjectsListFactoryStrategy> _logger;
    private readonly IParametersManager _parametersManager;

    // ReSharper disable once ConvertToPrimaryConstructor
    public ProjectsListFactoryStrategy(IParametersManager parametersManager,
        ILogger<ProjectsListFactoryStrategy> logger)
    {
        _parametersManager = parametersManager;
        _logger = logger;
    }

    public string StrategyName => nameof(ProjectsListFactoryStrategy);

    public List<CliMenuCommand> CreateMenuCommandsList()
    {
        var parameters = (UsbCopyParameters)_parametersManager.Parameters;
        //პროექტების ჩამონათვალი
        return parameters.Projects.OrderBy(o => o.Key).Select(kvp =>
            new UsbCopyProjectSubMenuCommand(_logger, _parametersManager, kvp.Key)).Cast<CliMenuCommand>().ToList();
    }
}
