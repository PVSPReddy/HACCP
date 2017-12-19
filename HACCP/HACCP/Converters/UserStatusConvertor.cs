using System;
using System.Globalization;
using Xamarin.Forms;

namespace HACCP
{
    public class UserStatusConverter : IValueConverter
    {
        /// <summary>
        /// User Status Converter
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isActive;
            short convertType;

            if (value == null || !bool.TryParse(value.ToString(), out isActive))
                isActive = false;
            if (parameter == null || !short.TryParse(parameter.ToString(), out convertType))
                convertType = 0;
            if (targetType == typeof(ImageSource) || targetType == typeof(FileImageSource))
            {
                switch (convertType)
                {
                    case 1:
                        return isActive ? "selectmenu.png" : "selectmenuDisable.png";
                    case 2:
                        return isActive ? "checklist.png" : "checklistDisable.png";
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