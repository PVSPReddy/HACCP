using System;
using System.Globalization;
using Xamarin.Forms;

namespace HACCP
{
    public class EmptyStringConverter : IValueConverter
    {
      /// <summary>
        /// Convert Empty string 
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
            var str = (string) value;
            return string.IsNullOrWhiteSpace(str) ? "<un-named device>" : str;
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
            throw new NotImplementedException("EmptyStringConverter is one-way");
        }
    }
}