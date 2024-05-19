using CliMenu;
using LibDataInput;
using LibParameters;
using SystemToolsShared;
using UsbCopy.Models;

namespace UsbCopy.Commands;

public sealed class DeleteUsbCopyProjectCommand : CliMenuCommand
{
    private readonly ParametersManager _parametersManager;
    private readonly string _projectName;

    // ReSharper disable once ConvertToPrimaryConstructor
    public DeleteUsbCopyProjectCommand(ParametersManager parametersManager, string projectName) : base("Delete Project",
        EMenuAction.LevelUp, EMenuAction.Reload, projectName)
    {
        _parametersManager = parametersManager;
        _projectName = projectName;
    }

    protected override bool RunBody()
    {
        var parameters = (UsbCopyParameters)_parametersManager.Parameters;

        var projects = parameters.Projects;
        if (!projects.ContainsKey(_projectName))
        {
            StShared.WriteErrorLine($"Project {_projectName} not found", true);
            return false;
        }

        if (!Inputer.InputBool($"This will Delete Project {_projectName}. are you sure?", false, false))
            return false;

        projects.Remove(_projectName);
        _parametersManager.Save(parameters, $"Project {_projectName} Deleted");

        return true;
    }
}