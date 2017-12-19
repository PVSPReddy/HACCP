using System;
using System.Globalization;
using Xamarin.Forms;

namespace HACCP
{
    public class GuidConverter : IValueConverter
    {
        /// <summary>
        /// Convert Guid
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            var guid = (Guid) value;
            return guid.ToString();
        }

        /// <summary>
        /// ConvertBack
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException("GuidConverter is one-way");
        }
    }
}