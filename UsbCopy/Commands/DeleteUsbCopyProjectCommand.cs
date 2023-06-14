using System;
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

    public DeleteUsbCopyProjectCommand(ParametersManager parametersManager, string projectName) : base(
        "Delete Project", projectName)
    {
        _parametersManager = parametersManager;
        _projectName = projectName;
    }

    protected override void RunAction()
    {
        try
        {
            var parameters = (UsbCopyParameters)_parametersManager.Parameters;

            var projects = parameters.Projects;
            if (!projects.ContainsKey(_projectName))
            {
                StShared.WriteErrorLine($"Project {_projectName} not found", true);
                return;
            }

            if (!Inputer.InputBool($"This will Delete Project {_projectName}. are you sure?", false, false))
                return;

            projects.Remove(_projectName);
            _parametersManager.Save(parameters, $"Project {_projectName} Deleted");
            MenuAction = EMenuAction.LevelUp;
            return;
        }
        catch (DataInputEscapeException)
        {
            Console.WriteLine();
            Console.WriteLine("Escape... ");
            StShared.Pause();
        }
        catch (Exception e)
        {
            StShared.WriteException(e, true);
        }

        MenuAction = EMenuAction.Reload;
    }
}