﻿using Caliburn.Micro;
using Ivao.It.Aurora.FlightStripPrinter.HotMouseAndKeys;
using Ivao.It.Aurora.FlightStripPrinter.Services;
using Ivao.It.AuroraConnector.Exceptions;
using Ivao.It.FlightStripper;
using Microsoft.Extensions.Logging;
using Spire.Pdf;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ivao.It.Aurora.FlightStripPrinter.ViewModels;

public class FlightStripPrinterViewModel : PropertyChangedBase, IViewModel
{
    private readonly ILogger<FlightStripPrinterViewModel> _logger;
    private readonly IAuroraService _aurora;
    private readonly ILogFileWatcherService _logWhatcher;
    private readonly IFlightStripPrintService _stripPrintService;
    private readonly KeyboardHook _hooksKeys;

    private string? _fileShowed;
    private string? _printQueueName;

    public WebBrowser? UiBrowser;

    #region Binded properties
    private bool _isConnected;
    public bool IsConnected
    {
        get { return _isConnected; }
        set
        {
            _isConnected = value;
            this.NotifyOfPropertyChange(() => CanConnectToAurora);
            this.NotifyOfPropertyChange(() => CanPrintStrip);
        }
    }
    public bool CanConnectToAurora => !this.IsConnected;
    public bool CanPrintStrip => this.IsConnected;

    private ObservableCollection<string> _logs;
    public ObservableCollection<string> Logs
    {
        get { return _logs; }
        set { _logs = value; this.NotifyOfPropertyChange(); }
    }
    #endregion

    public FlightStripPrinterViewModel(
        ILogger<FlightStripPrinterViewModel> logger,
        IAuroraService aurora,
        ILogFileWatcherService logWhatcher,
        IFlightStripPrintService stripPrintService)
    {
        _logger = logger;
        _aurora = aurora;
        _logWhatcher = logWhatcher;
        _stripPrintService = stripPrintService;
        _hooksKeys = new KeyboardHook();
    }

    #region Commands
    public async Task ConnectToAurora()
    {
        _logWhatcher.OnLogfileChanged += LogFileChanged;
        _logWhatcher.Start(DataFolderProvider.GetLogsFolder());
        try
        {
            if (Environment.GetEnvironmentVariable("AuroraEnabled") != "false")
            {
                await _aurora.ConnectAsync();
            }
            IsConnected = true;
            _logger.LogInformation("Connected to Aurora");

            _hooksKeys.SetPresentationSource(PresentationSourceProvider.Current!);
            _hooksKeys.KeyUp += new KeyEventHandler(HotkeyUp);
            _hooksKeys.Start();

        }
        catch (AuroraException ex)
        {
            _logger.LogError("Unable to connect to Aurora");
        }
    }

    public async Task PrintStrip()
    {
        var callsign = await _aurora.GetSelectedTrafficAsync();
        if (callsign is null)
        {
            _logger.LogWarning("Unable to detect the label selected in Aurora");
        }

        //_fileShowed = await _stripPrintService.ConvertToPdf(callsign);
        //UiBrowser?.Navigate(new Uri($"file:///{_fileShowed}"));
        //UiBrowser!.Visibility = System.Windows.Visibility.Visible;

        //await Task.Delay(800);
        //if (_fileShowed is null)
        //{
        //    _logger.LogError("La strip non c'è!!");
        //    return;
        //}
        //_stripPrintService.PrintWholeDocument(_fileShowed);
        //_logger.LogWarning("Flightstrip printed: {filename}", _fileShowed);
    }
    #endregion

    #region Hotkeys Commands
    private void HotkeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.F9) return;
        _logger.LogDebug("Strip print requested");
        this.PrintStrip();
    }
    #endregion

    private async void LogFileChanged(object sender, FileSystemEventArgs e)
    {
        using (var stream = File.Open(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (var reader = new StreamReader(stream))
        {
            var logs = (await reader.ReadToEndAsync()).Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            Logs = new ObservableCollection<string>(logs.Reverse());
        }
    }
}


public class FlightStripPrinterViewModelDesign : FlightStripPrinterViewModel
{
    public FlightStripPrinterViewModelDesign() : base(null, null, null, null)
    {
        this.Logs = new ObservableCollection<string> {
            "12456 [INF] Ciao",
            "12456 [WRN] Ciao 2",
            "Ciao 3",
            "Ciao 4",
        };
    }
}