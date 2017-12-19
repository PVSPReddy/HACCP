using System;
using System.Globalization;
using Xamarin.Forms;

namespace HACCP
{
    public class RecordStatusToImageNameConverter : IValueConverter
    {
        /// <summary>
        /// RecordStatusToImageNameConverter
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var recorStatus = (short) value;
            switch (recorStatus)
            {
                case 0:
                    return "";
                case 1:
                    return "completed.png";
                case 2:
                    return "round.png";
                default:
                    return "";
            }
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