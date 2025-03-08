using System;
using System.Globalization;
using System.Windows.Data;

namespace VolumeSlider.Converters
{
    public class BoolToMuteTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "Unmute" : "Mute";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 