using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;

namespace Ivao.It.Aurora.FlightStripPrinter.Converters;

[ValueConversion(typeof(string), typeof(SolidColorBrush))]
public class LogLevelToColorConverter : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var strVal = value.ToString()!;

        if (strVal.Contains(" [INF] "))
        {
            return new SolidColorBrush(Colors.MediumBlue);
        }

        if (strVal.Contains(" [WRN] "))
        {
            return new SolidColorBrush(Colors.Orange);
        }

        if (strVal.Contains(" [ERR] "))
        {
            return new SolidColorBrush(Colors.Red);
        }

        return new SolidColorBrush(Colors.LightGray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}
