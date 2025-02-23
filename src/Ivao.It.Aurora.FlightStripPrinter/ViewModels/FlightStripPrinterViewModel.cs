﻿using Caliburn.Micro;
using Ivao.It.Aurora.FlightStripPrinter.HotMouseAndKeys;
using Ivao.It.Aurora.FlightStripPrinter.Services;
using Ivao.It.Aurora.FlightStripPrinter.Services.Models;
using Ivao.It.AuroraConnector.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ivao.It.Aurora.FlightStripPrinter.ViewModels;

public class FlightStripPrinterViewModel : PropertyChangedBase, IViewModel
{
    private readonly ILogger<FlightStripPrinterViewModel> _logger;
    private readonly IAuroraService _aurora;
    private readonly IWindowManager _winmgr;
    private readonly ILogFileWatcherService _logWhatcher;
    private readonly IFlightStripPrintService _stripPrintService;
    private readonly KeyboardHook _hooksKeys;

    private string? _fileShowed;
    public WebBrowser? UiBrowser;
    private PrintPreviewViewModel? _printPreviewVm;

    #region Binded properties
    private bool? _isConnected;
    public bool? IsConnected
    {
        get { return _isConnected; }
        set
        {
            _isConnected = value;
            this.NotifyOfPropertyChange(() => CanConnectToAurora);
            if (value != null)
            {
                this.NotifyOfPropertyChange(() => CanPrintStrip);
                this.NotifyOfPropertyChange(() => CanPrintStripWithPrinterChoice);
                this.NotifyOfPropertyChange(() => CanGenerateStrip);
                this.NotifyOfPropertyChange(() => CanShowPrintPreview);
            }
        }
    }
    public bool CanConnectToAurora => !(this.IsConnected ?? true);
    public bool CanPrintStrip => this.IsConnected ?? false;
    public bool CanPrintStripWithPrinterChoice => this.IsConnected ?? false;
    public bool CanGenerateStrip => this.IsConnected ?? false;
    public bool CanShowPrintPreview => (this.IsConnected ?? false) && _printPreviewVm is null;

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
        IWindowManager winmgr,
        ILogFileWatcherService logWhatcher,
        IFlightStripPrintService stripPrintService)
    {
        _logger = logger;
        _aurora = aurora;
        _winmgr = winmgr;
        _logWhatcher = logWhatcher;
        _stripPrintService = stripPrintService;
        _hooksKeys = new KeyboardHook();
        Logs = new ObservableCollection<string>();
        IsConnected = false;

        _logWhatcher.Init(DataFolderProvider.GetLogsFolder());
        ReadLogs(_logWhatcher.WatchingFile.FullName);
    }

    #region Commands
    public async Task ConnectToAurora()
    {
        _logWhatcher.OnLogfileChanged += LogFileChanged;
        _logWhatcher.Start();
        try
        {
            IsConnected = null;
            if (Environment.GetEnvironmentVariable("AuroraEnabled") != "false")
            {
                await _aurora.ConnectAsync();
            }
            _logger.LogWarning("Connected to Aurora");
            IsConnected = true;

            _hooksKeys.SetPresentationSource(PresentationSourceProvider.Current!);
            _hooksKeys.KeyUp += new KeyEventHandler(HotkeyUp);
            _hooksKeys.Start();

        }
        catch (AuroraException ex)
        {
            if (EnvironmentHandler.IsProduction())
            {
                _logger.LogError("Unable to connect to Aurora. Check you have enabled 3rd party software access.");
            }
            else
            {
                _logger.LogError(ex, "Unable to connect to Aurora");
            }

            IsConnected = false;
        }
    }
    public async Task GenerateStrip() => await this.GerateStripHandlerAsync();
    public async Task PrintStrip() => await this.PrintStripHandler();
    public async Task PrintStripWithPrinterChoice() => await this.PrintStripHandler(true);

    public async Task ShowPrintPreview()
    {
        _printPreviewVm = new PrintPreviewViewModel();
        NotifyOfPropertyChange(() => CanShowPrintPreview);
        _printPreviewVm.OnClosed += (s, e) =>
        {
            _printPreviewVm = null;
            NotifyOfPropertyChange(() => CanShowPrintPreview);
        };


        if (_fileShowed is not null)
        {
            _printPreviewVm.PdfFilePath = _fileShowed!.Replace(".html", ".pdf");
        }
        await _winmgr.ShowWindowAsync(_printPreviewVm, null);
    }

    private async Task<AuroraTraffic?> GerateStripHandlerAsync()
    {
        _logger.LogDebug("Strip requested");
        var tfcData = await _aurora.GetSelectedTrafficAsync();
        if (tfcData is null)
        {
            _logger.LogWarning("Unable to get selected label data from Aurora");
            return null;
        }
        var controlledApts = await _aurora.GetControlledAirportsAsync();
        if (controlledApts is null)
        {
            _logger.LogWarning("Unable to get controlled airports data from Aurora");
            controlledApts = new();
        }

        _fileShowed = await _stripPrintService.BindAndConvertToPdf(tfcData, controlledApts);
        UiBrowser?.Navigate(new Uri($"file:///{_fileShowed}"));
        UiBrowser!.Visibility = System.Windows.Visibility.Visible;

        if (_printPreviewVm is not null) _printPreviewVm.PdfFilePath = _fileShowed!.Replace(".html", ".pdf");

        return tfcData;
    }
    private async Task PrintStripHandler(bool forcePrinterChoice = false)
    {
        var tfcData = await this.GerateStripHandlerAsync();
        if (tfcData is null) return;

        if (_fileShowed is null)
        {
            _logger.LogError("Unable to detect strip to print - {callsign}", tfcData.Callsign);
            return;
        }
        if (_stripPrintService.PrintWholeDocument(_fileShowed, forcePrinterChoice))
        {
            _logger.LogInformation("Flightstrip print request sent - {callsign}", tfcData.Callsign);
        }
        else
        {
            _logger.LogWarning("Flightstrip print failed or refused - {callsign}", tfcData.Callsign);
        }
    }
    #endregion

    #region Hotkeys Commands
    private void HotkeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.F9) return;
        _logger.LogDebug("Strip print requested");
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        this.PrintStrip();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }
    #endregion

    private async void LogFileChanged(object sender, FileSystemEventArgs e)
    {
        ReadLogs(e.FullPath);
    }

    private async void ReadLogs(string fullPath)
    {
        await using var stream = File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(stream);
        var logs = (await reader.ReadToEndAsync()).Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Logs = new ObservableCollection<string>(logs.Reverse());
    }

    public Task ViewLoadedAsync() => Task.CompletedTask;
    public EventHandler<EventArgs>? OnClosed { get; set; }
}


public class FlightStripPrinterViewModelDesign : FlightStripPrinterViewModel
{
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
    public FlightStripPrinterViewModelDesign() : base(null, null, null, null, null)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    {
        this.Logs = new ObservableCollection<string> {
            "12456 [INF] Ciao",
            "12456 [WRN] Ciao 2",
            "Ciao 3",
            "Ciao 4",
        };
    }
}