using System;
using AppCliTools.CliMenu;
using AppCliTools.CliMenu.DependencyInjection;
using AppCliTools.CliTools.App;
using AppCliTools.CliTools.DependencyInjection;
using AppCliTools.CliTools.Services.MenuBuilder;
using Microsoft.Extensions.DependencyInjection;
using ParametersManagement.LibParameters;
using Serilog.Events;
using SystemTools.SystemToolsShared;
using UsbCopy.Menu.ProjectsList;
using UsbCopy.Menu.UsbCopyParametersEdit;
using UsbCopy.Models;

namespace UsbCopy;

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

    private static IServiceCollection AddApplication(this IServiceCollection services,
        Action<ApplicationOptions> setupAction)
    {
        services.AddSingleton<IApplication, Application>();
        services.Configure(setupAction);
        return services;
    }

    // ReSharper disable once UnusedMethodReturnValue.Local
    private static IServiceCollection AddMainParametersManager(this IServiceCollection services,
        Action<MainParametersManagerOptions> setupAction)
    {
        services.AddSingleton<IParametersManager, ParametersManager>();
        services.Configure(setupAction);
        return services;
    }
}
