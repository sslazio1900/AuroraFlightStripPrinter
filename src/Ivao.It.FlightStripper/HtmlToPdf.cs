using Syncfusion.HtmlConverter;
using Syncfusion.Licensing;

namespace Ivao.It.FlightStripper;

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
    //TODO Test overwrite of the strip
}