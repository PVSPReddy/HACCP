using System;
using System.Globalization;
using Xamarin.Forms;

namespace HACCP
{
    public class BatteryLevelConverter : IValueConverter
    {
        /// <summary>
        /// Convert to BatteryLevel Image
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var percentage = (int) value;

            if (percentage >= 0 && percentage < 25)
            {
                return "batteryEmpty.png";
            }
            if (percentage >= 25 && percentage < 50)
            {
                return "battery25.png";
            }
            if (percentage >= 50 && percentage < 75)
            {
                return "battery50.png";
            }
            if (percentage >= 75 && percentage < 100)
            {
                return "battery75.png";
            }
            if (percentage >= 100)
            {
                return "batteryFull.png";
            }
            return "batteryEmpty.png";
        }

        /// <summary>
        /// ConvertBack
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}