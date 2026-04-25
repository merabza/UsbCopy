using AppCliTools.CliMenu;
using AppCliTools.CliMenu.DependencyInjection;
using AppCliTools.CliTools.DependencyInjection;
using AppCliTools.CliTools.Services.MenuBuilder;
using Microsoft.Extensions.DependencyInjection;
using ParametersManagement.LibParameters.DependencyInjection;
using Serilog.Events;
using UsbCopy.Menu.ProjectsList;
using UsbCopy.Menu.UsbCopyParametersEdit;
using UsbCopy.Models;

namespace UsbCopy.DependencyInjection;

public static class UsbCopyServices
{
    public static IServiceCollection AddServices(this IServiceCollection services, string appName,
        UsbCopyParameters par, string parametersFileName)
    {
        // @formatter:off
        services
            .AddSerilogLoggerService(LogEventLevel.Information, appName, par.LogFolder)
            .AddTransientAllStrategies<IMenuCommandListFactoryStrategy>(
                typeof(ProjectsListFactoryStrategy).Assembly)
            .AddSingleton<IMenuBuilder, UsbCopyMenuBuilder>()
            .AddTransientAllStrategies<IMenuCommandFactoryStrategy>(
                typeof(UsbCopyParametersEditorListCliMenuCommandFactoryStrategy).Assembly)
            .AddApplication(x =>
            {
                x.AppName = appName;
            })
            .AddMainParametersManager(x =>
            {
                x.ParametersFileName = parametersFileName;
                x.Par = par;
            })
            ;

        // @formatter:on

        return services;
    }
}
