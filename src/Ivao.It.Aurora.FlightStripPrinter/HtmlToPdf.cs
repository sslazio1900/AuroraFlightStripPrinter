﻿using Ivao.It.FlightStripper;
using Syncfusion.HtmlConverter;
using Syncfusion.Licensing;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Ivao.It.Aurora.FlightStripPrinter;

public class HtmlToPdf
{
    public static void Init(string key, string appDataPath)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(appDataPath);

        AppDataPath = appDataPath;
        SyncfusionLicenseProvider.RegisterLicense(key);
        WipeoutTempFolder();
    }

    public static void DeInit()
    {
        WipeoutTempFolder();
    }

    private static string? AppDataPath;

    public async Task<string> CreateStripInPathAsync(string flightStripName, string stripContents, CancellationToken cancellationToken = default)
    {
        if (AppDataPath is null) throw new InvalidOperationException("Html2Pdf component not initialized");

        var filepath = Path.Combine(AppDataPath, $"{flightStripName}.html");
        if (!Directory.Exists(AppDataPath))
        {
            Directory.CreateDirectory(AppDataPath);
        }
        await File.WriteAllTextAsync(filepath, stripContents);

        return filepath;
    }

    public async Task ConvertToPdfAsync(string flightStripName)
    {
        if (AppDataPath is null) throw new InvalidOperationException("Html2Pdf component not initialized");

        var sourceFilePath = Path.Combine(AppDataPath, $"{flightStripName}.html");
        var convertedFilePath = Path.Combine(AppDataPath, $"{flightStripName}.pdf");

        var rotatedContents = await RotateFlightStrip(sourceFilePath);

        HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter();
        Syncfusion.Pdf.PdfDocument document = htmlConverter.Convert(rotatedContents, "localhost");
        MemoryStream stream = new MemoryStream();
        document.Save(stream);

        await File.WriteAllBytesAsync(convertedFilePath, stream.ToArray());
    }

    private async Task<string> RotateFlightStrip(string stripPath)
    {
        var contents = await File.ReadAllTextAsync(stripPath);
        contents = contents.Replace(Consts.ToBeReplaced, Consts.StripRotationCss);
        return contents;
    }

    private static void WipeoutTempFolder()
    {
        if (AppDataPath is null) throw new InvalidOperationException("Html2Pdf component not initialized");

        if (!Directory.Exists(AppDataPath))
        {
            Directory.CreateDirectory(AppDataPath);
            return;
        }

        foreach (var file in Directory.EnumerateFiles(AppDataPath!))
        {
            File.Delete(file);
        }
    }
}