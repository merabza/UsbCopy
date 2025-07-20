//Created by ProgramClassCreator at 6/28/2021 12:52:58

using System;
using CliParameters;
using LibParameters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using SystemToolsShared;
using UsbCopy;
using UsbCopy.Models;

ILogger<Program>? logger = null;
try
{
    Console.WriteLine("Loading...");

    const string appName = "UsbCopy";
    const string appKey = "DFD53CBF-F62F-4795-80DA-F4AD7ECC4AD3";

    //პროგრამის ატრიბუტების დაყენება 
    ProgramAttributes.Instance.AppName = appName;
    ProgramAttributes.Instance.AppKey = appKey;

    var key = appKey + Environment.MachineName.Capitalize();

    var argParser = new ArgumentsParser<UsbCopyParameters>(args, "UsbCopy", key);
    switch (argParser.Analysis())
    {
        case EParseResult.Ok: break;
        case EParseResult.Usage: return 1;
        case EParseResult.Error: return 2;
        default: throw new ArgumentOutOfRangeException();
    }

    var par = (UsbCopyParameters?)argParser.Par;
    if (par is null)
    {
        StShared.WriteErrorLine("ApAgentParameters is null", true);
        return 3;
    }

    var parametersFileName = argParser.ParametersFileName;
    var servicesCreator = new ServicesCreator(par.LogFolder, null, "UsbCopy");
    // ReSharper disable once using
    using var serviceProvider = servicesCreator.CreateServiceProvider(LogEventLevel.Information);

    if (serviceProvider == null)
    {
        Console.WriteLine("Logger does not created");
        return 8;
    }

    logger = serviceProvider.GetService<ILogger<Program>>();
    if (logger is null)
    {
        StShared.WriteErrorLine("logger is null", true);
        return 3;
    }

    var usbCopy = new UsbCopyCliAppLoop(logger, new ParametersManager(parametersFileName, par));

    return usbCopy.Run() ? 0 : 1;
}
catch (Exception e)
{
    StShared.WriteException(e, true, logger);
    return 7;
}
finally
{
    Log.CloseAndFlush();
}