using System;
using System.Globalization;
using Xamarin.Forms;

namespace HACCP
{
    public class SelectedCellColorConverter : IValueConverter
    {

        /// <summary>
        /// SelectedCellColorConverter
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isSelected = false;
            if (value == null || !bool.TryParse(value.ToString(), out isSelected))
                isSelected = false;

            if (isSelected)
                return Color.FromHex("#27596c");
            return Color.Transparent;
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