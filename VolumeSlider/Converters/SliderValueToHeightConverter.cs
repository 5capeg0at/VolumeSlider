using System;
using System.Globalization;
using System.Windows.Data;

namespace VolumeSlider.Converters
{
    public class SliderValueToHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
            {
                // Calculate height based on slider value (0-1)
                return doubleValue * 100; // Scale to percentage of total height
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 