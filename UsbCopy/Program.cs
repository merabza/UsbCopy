using System;
using System.Runtime.CompilerServices;
using AppCliTools.CliParameters;
using AppCliTools.CliTools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
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

    (CliAppLoopParameters? cliLoopPar, logger) = CliAppLoopParameters.Create<Program>(serviceProvider);
    if (cliLoopPar is null)
    {
        return 3;
    }

    var cliAppLoop = new CliAppLoop(cliLoopPar);

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
