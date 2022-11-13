using Caliburn.Micro;
using Ivao.It.Aurora.FlightStripPrinter.Services;
using Ivao.It.Aurora.FlightStripPrinter.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Filters;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace Ivao.It.Aurora.FlightStripPrinter;
public class Bootstrapper : BootstrapperBase
{
    private ServiceProvider? _serviceProvider;
    private ILoggerFactory? _lf;

    public Bootstrapper()
    {
        Initialize();
    }

    protected override void Configure()
    {
        CreateLogger();
        var sc = new ServiceCollection();

        //Config - Json like aspnetcore
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddUserSecrets(Assembly.GetExecutingAssembly())
            .Build();

        //sc.AddSingleton<ILoggerFactory>(_lf);
        sc.AddLogging(conf => conf.AddSerilog());

        //ViewModels
        sc.AddScoped<ShellViewModel>();
        sc.AddScoped<FlightStripPrinterViewModel>();
        sc.AddScoped<SettingsViewModel>();

        //Services
        sc.AddScoped<ILogFileWatcherService, LogFileWatcherService>();
        sc.AddTransient<IAuroraService, AuroraService>();
        sc.AddScoped<IFlightStripPrintService, FlightStripPrintService>();

        //Caliburn
        sc.AddScoped<IWindowManager, WindowManager>();
        sc.AddScoped<IEventAggregator, EventAggregator>();

        //Init Syncfusion
        HtmlToPdf.Init(config.GetSection("SyncfusionLicenseKey").Value, DataFolderProvider.GetStripsFolder());

        //Wiring up with Bootstrapper
        _serviceProvider = sc.BuildServiceProvider();
        Log.Logger.Warning("App started & initialized!");
    }


    protected override object GetInstance(Type service, string key)
        => _serviceProvider!.GetService(service)!;

    protected override IEnumerable<object> GetAllInstances(Type service)
        => _serviceProvider!.GetServices(service)!;

    protected override async void OnStartup(object sender, StartupEventArgs e)
    {
        await DisplayRootViewForAsync<ShellViewModel>(
            new Dictionary<string, object>{
                {"Title", "IVAO IT Aurora Flight Strip Printer" },
                {"MinWidth", 800 },
                {"MinHeight", 600 },
            });
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

    private void CreateLogger()
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
                                flushToDiskInterval: TimeSpan.FromMilliseconds(500))
            )
            .CreateLogger();
        _lf = new SerilogLoggerFactory(Log.Logger);
    }
}
