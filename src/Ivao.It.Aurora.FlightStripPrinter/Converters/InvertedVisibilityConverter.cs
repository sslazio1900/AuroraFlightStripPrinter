using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Ivao.It.Aurora.FlightStripPrinter.Converters;

[ValueConversion(typeof(Visibility), typeof(Visibility))]
public class InvertedVisibilityConverter : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (Visibility)value switch
    {
        Visibility.Visible => Visibility.Collapsed,
        Visibility.Hidden => Visibility.Visible,
        Visibility.Collapsed => Visibility.Visible,
        _ => Visibility.Collapsed,
    };

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}
