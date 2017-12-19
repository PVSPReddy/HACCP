using System;
using System.Globalization;
using Xamarin.Forms;

namespace HACCP
{
    public class LoginStatusConverter : IValueConverter
    {

        /// <summary>
        /// Login Status Converter
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isLoggedIn;
            short convertType;

            if (value == null || !bool.TryParse(value.ToString(), out isLoggedIn))
                isLoggedIn = false;
            if (parameter == null || !short.TryParse(parameter.ToString(), out convertType))
                convertType = 0;
            if (targetType == typeof(ImageSource) || targetType == typeof(FileImageSource))
            {
                switch (convertType)
                {
                    case 1:
                        return isLoggedIn ? "logout.png" : "login.png";
                    case 2:
                        return isLoggedIn ? "location.png" : "locationDisable.png";
                    case 3:
                        return isLoggedIn ? "checklist.png" : "checklistDisable.png";
                    case 4:
                        return isLoggedIn ? "clearcheckmark.png" : "clearcheckmarkDisable.png";
                    case 5:
                        return isLoggedIn ? "selectmenu.png" : "selectmenuDisable.png";
                    case 6:
                        return isLoggedIn ? "checklist.png" : "checklistDisable.png";
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