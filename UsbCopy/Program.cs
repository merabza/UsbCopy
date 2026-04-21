//Created by ProgramClassCreator at 6/28/2021 12:52:58

using System;
using System.Runtime.CompilerServices;
using AppCliTools.CliParameters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using Serilog;
using Serilog.Events;
using SystemTools.SystemToolsShared;
using UsbCopy;
using UsbCopy.Models;

ILogger<Program>? logger = null;
try
{
    Console.WriteLine("Loading...");

    const string appName = "UsbCopy";

    var argParser = new ArgumentsParser<UsbCopyParameters>(args, appName);

    switch (argParser.Analysis())
    {
        case EParseResult.Ok:
            break;
        case EParseResult.Usage:
            return 1;
        case EParseResult.Error:
            return 2;
        default:
            throw new SwitchExpressionException();
    }

    var serviceCollection = new ServiceCollection();

    // ReSharper disable once using
    await using ServiceProvider serviceProvider = serviceCollection
        .AddServices(appName, argParser.Par!, argParser.ParametersFileName!).BuildServiceProvider();



    var cliLoopPar = CliAppLoopParameters<Program>.Create(serviceProvider);
    if (cliLoopPar is null)
    {
        return 3;
    }

    logger = cliLoopPar.Logger;

    var cliAppLoop = new CliAppLoop<Program>(cliLoopPar);

    return await cliAppLoop.Run() ? 0 : 100;
}
catch (Exception e)
{
    StShared.WriteException(e, true, logger);
    return 4;
}
finally
{
    await Log.CloseAndFlushAsync();
}
