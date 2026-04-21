using System;
using AppCliTools.CliMenu;
using AppCliTools.CliMenu.DependencyInjection;
using AppCliTools.CliTools.DependencyInjection;
using AppCliTools.CliTools.Models;
using AppCliTools.CliTools.Services.MenuBuilder;
using Microsoft.Extensions.DependencyInjection;
using ParametersManagement.LibParameters;
using Serilog.Events;
using SystemTools.BackgroundTasks;
using SystemTools.SystemToolsShared;
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
            //.AddTransientAllStrategies<IMenuCommandListFactoryStrategy>(
            //    typeof(ProjectGroupsListFactoryStrategy).Assembly)
            //.AddSingleton<IProcesses, Processes>()
            //.AddSingleton<IMenuBuilder, SupportToolsMenuBuilder>()
            //.AddTransientAllStrategies<IMenuCommandFactoryStrategy>(
            //    typeof(SupportToolsParametersEditorListCliMenuCommandFactoryStrategy).Assembly)
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
            //.AddMainParametersManager(x =>
            //{
            //    x.ParametersFileName = parametersFileName;
            //    x.Par = par;
            //})
            ;

        // @formatter:on
        //if (!string.IsNullOrWhiteSpace(par.RecentCommandsFileName) && par.RecentCommandsCount > 0)
        //{
        //    services.AddRecentCommandsService(x =>
        //    {
        //        x.RecentCommandsFileName = par.RecentCommandsFileName;
        //        x.RecentCommandsCount = par.RecentCommandsCount;
        //    });
        //}

        return services;
    }

    private static IServiceCollection AddApplication(this IServiceCollection services,
        Action<ApplicationOptions> setupAction)
    {
        services.AddSingleton<IApplication, UsbCopyApplication>();
        services.Configure(setupAction);
        return services;
    }

}
