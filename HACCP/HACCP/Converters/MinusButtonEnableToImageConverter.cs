﻿using System;
using System.Globalization;
using Xamarin.Forms;

namespace HACCP
{
    public class MinusButtonEnableToImageConverter : IValueConverter
    {
        /// <summary>
        /// Minus Button Enable To Image Converter
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isEnabled = (bool) value;
            if (isEnabled)
                return "minus.png";
            return "minusDisable.png";
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