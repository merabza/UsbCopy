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

    //პროგრამის ატრიბუტების დაყენება 
    StatProgAttr.SetAttr();

    var key = ProgramAttributes.Instance.GetAttribute<string>("AppKey") + Environment.MachineName.Capitalize();

    IArgumentsParser argParser = new ArgumentsParser<UsbCopyParameters>(args, "UsbCopy", key);
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
    ServicesCreator servicesCreator = new(par.LogFolder, null, "UsbCopy");
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

    UsbCopy.UsbCopy usbCopy = new(logger, new ParametersManager(parametersFileName, par));

    //პროგრამის ატრიბუტების დაყენება 
    StatProgramAttr.SetAttr();

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