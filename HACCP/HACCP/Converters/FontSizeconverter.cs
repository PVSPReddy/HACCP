using System;
using System.Globalization;
using Xamarin.Forms;

namespace HACCP
{
    public class FontSizeconverter : IValueConverter
    {
        /// <summary>
        /// Font size converter
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var str = (string) value;
                if (!string.IsNullOrEmpty(str) && str.Length > 2)
                {
                    double num;
                    var val = str.Substring(0, str.Length - 2);
                    if (double.TryParse(val, out num))
                    {
                        return 48;
                    }
                    return 30;
                }
                return 30;
            }
            catch (Exception)
            {
                return 30;
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