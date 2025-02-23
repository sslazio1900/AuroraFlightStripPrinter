﻿using Caliburn.Micro;
using Ivao.It.Aurora.FlightStripPrinter.Services;
using Ivao.It.Aurora.FlightStripPrinter.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;
using Serilog.Events;

namespace Ivao.It.Aurora.FlightStripPrinter;
public class Bootstrapper : BootstrapperBase
{
    private ServiceProvider? _serviceProvider;

    public Bootstrapper()
    {
        Initialize();
    }

    protected override void Configure()
    {
        EnvironmentHandler.ForceEnvIfNotSet();
        var sc = new ServiceCollection();

        //Config - Json like aspnetcore
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{EnvironmentHandler.GetCurrentEnvironment()}.json", optional: true)
#if DEBUG || BETA
            .AddUserSecrets(Assembly.GetExecutingAssembly())
#else
            .AddSyncfusionLicensing()
#endif
            .Build();

        sc.AddLogging(conf => conf.AddSerilog(CreateLogger()));

        sc.AddSyncfusionLicensing();

        //ViewModels
        sc.AddScoped<ShellViewModel>();
        sc.AddScoped<FlightStripPrinterViewModel>();
        sc.AddScoped<SettingsViewModel>();
        sc.AddScoped<PrintPreviewViewModel>();

        //Services
        sc.AddScoped<ILogFileWatcherService, LogFileWatcherService>();
        sc.AddTransient<IAuroraService, AuroraService>();
        sc.AddTransient<ISettingsService, SettingsService>();
        sc.AddScoped<IFlightStripPrintService, FlightStripPrintService>();

        //Caliburn
        sc.AddScoped<IWindowManager, WindowManager>();
        sc.AddScoped<IEventAggregator, EventAggregator>();

        //Init Syncfusion
        HtmlToPdf.Init(DataFolderProvider.GetStripsFolder());

        //Wiring up with Bootstrapper
        _serviceProvider = sc.BuildServiceProvider();

        Log.Logger.Information("App started & initialized!");
    }


    protected override object GetInstance(Type service, string key)
        => _serviceProvider!.GetService(service)!;

    protected override IEnumerable<object> GetAllInstances(Type service)
        => _serviceProvider!.GetServices(service)!;

    protected override async void OnStartup(object sender, StartupEventArgs e)
    {
        await IoC.Get<ISettingsService>().InitNewSettingsIfNotExisting();

        await DisplayRootViewForAsync<ShellViewModel>(
            new Dictionary<string, object>{
                {"Title", "IVAO IT Aurora Flight Strip Printer" },
                {"MinWidth", 800 },
                {"MinHeight", 600 },
            });

        await new GitHubUpdateManager().CheckForUpdates();
    }

    protected override void OnExit(object sender, EventArgs e)
    {
        HtmlToPdf.DeInit();
        base.OnExit(sender, e);
        Log.Logger.Warning("App Closed");
    }
    protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        HtmlToPdf.DeInit();
        Log.Logger.Fatal(e.Exception, "App Crashed!");
        base.OnUnhandledException(sender, e);
    }


    private ILogger CreateLogger()
    {
        var auroraSources = Matching.FromSource("Ivao.It.AuroraConnector");
        var traceId = Guid.NewGuid();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Logger(l => l
                .Filter.ByIncludingOnly(auroraSources)
                .WriteTo.File($"{DataFolderProvider.GetLogsFolder()}/aurora-{traceId}.txt",
                                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            )
            .WriteTo.Logger(l => l
                .Filter.ByExcluding(auroraSources)
                .WriteTo.File($"{DataFolderProvider.GetLogsFolder()}/log-{traceId}.txt",
                                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                                flushToDiskInterval: TimeSpan.FromMilliseconds(500), 
                                restrictedToMinimumLevel: EnvironmentHandler.IsProduction() ? LogEventLevel.Information : LogEventLevel.Verbose
                                )
            )
            .CreateLogger();

        //Manual file retaining policy: trace-id custom named file breakes Serilogs retaining policy
        var files = new DirectoryInfo(DataFolderProvider.GetLogsFolder()).GetFiles().OrderByDescending(f => f.LastWriteTime).Skip(20);
        foreach (var file in files)
            file.Delete();

        return Log.Logger;
    }
}
