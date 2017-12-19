using System;
using System.Globalization;
using Xamarin.Forms;

namespace HACCP
{
    public class RadioButtonStatusConverter : IValueConverter
    {

        /// <summary>
        /// RadioButtonStatusConverter
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            short radioGroupType = 0;
            short radioGroupValue = 0;
            if (value == null || !short.TryParse(value.ToString(), out radioGroupValue))
                radioGroupValue = 0;
            if (parameter == null || !short.TryParse(parameter.ToString(), out radioGroupType))
                radioGroupType = 0;
            if (targetType == typeof(ImageSource) || targetType == typeof(FileImageSource))
            {
                if (radioGroupType > 0)
                {
                    if (radioGroupType == radioGroupValue)
                        return "select.png";
                    return "deselect_radio.png";
                }
            }

            return string.Empty;
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