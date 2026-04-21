using System.Collections.Generic;
using UsbCopy.Menu.CreateNewProject;
using UsbCopy.Menu.ProjectsList;
using UsbCopy.Menu.UsbCopyParametersEdit;

namespace UsbCopy.Menu;

public static class MenuData
{
    public static List<string> MainMenuCommandFactoryStrategyNames { get; } =
    [
        //ძირითადი პარამეტრების რედაქტირება
        nameof(UsbCopyParametersEditorListCliMenuCommandFactoryStrategy),
        //ახალი პროექტის შექმნა
        nameof(CreateNewProjectFactoryStrategy),
        //პროექტების ჩამონათვალი
        nameof(ProjectsListFactoryStrategy)
    ];
}
