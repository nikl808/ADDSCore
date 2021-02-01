using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace ADDSCore.Converters
{
    public class VisibilityToBooleanConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_converter == null)
                _converter = new VisibilityToBooleanConverter();
            return _converter;
        }

        private static VisibilityToBooleanConverter _converter = null;
    }
}
