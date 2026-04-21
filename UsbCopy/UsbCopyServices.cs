using System;
using AppCliTools.CliMenu;
using AppCliTools.CliMenu.DependencyInjection;
using AppCliTools.CliTools.DependencyInjection;
using AppCliTools.CliTools.Models;
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
            //.AddHttpClient()
            //.AddMemoryCache()
            //.AddSingleton<MenuParameters>()
            .AddTransientAllStrategies<IMenuCommandListFactoryStrategy>(
                typeof(ProjectsListFactoryStrategy).Assembly)
            //.AddSingleton<IProcesses, Processes>()
            .AddSingleton<IMenuBuilder, UsbCopyMenuBuilder>()
            .AddTransientAllStrategies<IMenuCommandFactoryStrategy>(
                typeof(UsbCopyParametersEditorListCliMenuCommandFactoryStrategy).Assembly)
            //.AddTransientAllStrategies<IToolCommandFactoryStrategy>(
            //    typeof(CorrectNewDatabaseToolCommandFactoryStrategy).Assembly,
            //    typeof(JetBrainsCleanupCodeRunnerToolCommandFactoryStrategy).Assembly,
            //    typeof(JsonFromProjectDbProjectGetterFactoryStrategy).Assembly,
            //    typeof(GenerateApiRoutesToolCommandFactoryStrategy).Assembly,
            //    typeof(ApplicationSettingsEncoderToolCommandFactoryStrategy).Assembly)
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
        services.AddSingleton<IApplication, UsbCopyApplication>();
        services.Configure(setupAction);
        return services;
    }

    private static IServiceCollection AddMainParametersManager(this IServiceCollection services,
        Action<MainParametersManagerOptions> setupAction)
    {
        services.AddSingleton<IParametersManager, ParametersManager>();
        services.Configure(setupAction);
        return services;
    }
}
