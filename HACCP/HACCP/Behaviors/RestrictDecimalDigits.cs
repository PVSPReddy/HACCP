using System;
using System.Globalization;
using HACCP.Core;
using Xamarin.Forms;

namespace HACCP
{
    public class RestrictDecimalDigits : Behavior<Entry>
    {
        public static readonly BindableProperty DecimalDigitCountProperty = BindableProperty.Create(
            "DecimalDigitCount", typeof(int), typeof(RestrictDecimalDigits), 0);

        /// <summary>
        /// DecimalDigitCount
        /// </summary>
        public int DecimalDigitCount
        {
            get { return (int) GetValue(DecimalDigitCountProperty); }
            set { SetValue(DecimalDigitCountProperty, value); }
        }

        /// <summary>
        /// Max
        /// </summary>
        public double Max
        {
            get { return HaccpAppSettings.SharedInstance.DeviceSettings.TempScale == 0 ? 9999.9 : 5537.7; }
        }

        /// <summary>
        /// Min
        /// </summary>
        public double Min
        {
            get { return HaccpAppSettings.SharedInstance.DeviceSettings.TempScale == 0 ? -9999.9 : -5537.7; }
        }

        /// <summary>
        /// OnAttachedTo
        /// </summary>
        /// <param name="bindable"></param>
        protected override void OnAttachedTo(Entry bindable)
        {
            if (HaccpAppSettings.SharedInstance.IsWindows)
                return;
            bindable.TextChanged += bindable_TextChanged;
        }

        /// <summary>
        /// bindable_TextChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bindable_TextChanged(object sender, TextChangedEventArgs e)
        {
            var decimalChar = Convert.ToString(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            var falsecharacter = decimalChar == "." ? "," : ".";

            var text = e.NewTextValue;
            double value;

            if (string.IsNullOrEmpty(text) || text == decimalChar || text == "-" ||
                text == string.Format("-{0}", decimalChar))
                return;

            if (!string.IsNullOrEmpty(text) && text.Contains(" "))
            {
                ((Entry) sender).Text = e.OldTextValue;
                return;
            }

            try
            {
                if (text.Contains(falsecharacter))
                {
                    ((Entry) sender).Text = e.OldTextValue;
                    return;
                }
                value = Convert.ToDouble(text);
            }
            catch
            {
                ((Entry) sender).Text = e.OldTextValue;
                return;
            }

            var index = text.IndexOf(decimalChar, StringComparison.Ordinal);
            if (index != -1 && index + DecimalDigitCount < text.Length)
            {
                if (value > Max || value < Min)
                {
                    ((Entry) sender).Text = e.OldTextValue;
                }
                else
                    ((Entry) sender).Text = text.Substring(0, index + DecimalDigitCount + 1);
            }
            else if (value > Max || value < Min)
            {
                ((Entry) sender).Text = e.OldTextValue;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bindable"></param>
        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= bindable_TextChanged;
        }
    }

    /// <summary>
    /// RecordAnswerDecimalDigits
    /// </summary>
    public class RecordAnswerDecimalDigits : Behavior<Entry>
    {
        public static readonly BindableProperty DecimalDigitCountProperty = BindableProperty.Create(
            "DecimalDigitCount", typeof(int), typeof(RestrictDecimalDigits), 0);

        /// <summary>
        /// DecimalDigitCount
        /// </summary>
        public int DecimalDigitCount
        {
            get { return (int) GetValue(DecimalDigitCountProperty); }
            set { SetValue(DecimalDigitCountProperty, value); }
        }

        /// <summary>
        /// Max
        /// </summary>
        public double Max
        {
            get { return 9999.9; }
        }

        /// <summary>
        /// Min
        /// </summary>
        public double Min
        {
            get { return -9999.9; }
        }

        /// <summary>
        /// OnAttachedTo
        /// </summary>
        /// <param name="bindable"></param>
        protected override void OnAttachedTo(Entry bindable)
        {
            if (HaccpAppSettings.SharedInstance.IsWindows)
                return;
            bindable.TextChanged += bindable_TextChanged;
        }

        /// <summary>
        /// bindable_TextChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bindable_TextChanged(object sender, TextChangedEventArgs e)
        {
            var decimalChar = Convert.ToString(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            var falsecharacter = decimalChar == "." ? "," : ".";

            var text = e.NewTextValue;
            double value;

            if (string.IsNullOrEmpty(text) || text == decimalChar || text == "-" ||
                text == string.Format("-{0}", decimalChar))
                return;

            if (!string.IsNullOrEmpty(text) && text.Contains(" "))
            {
                ((Entry) sender).Text = e.OldTextValue;
                return;
            }

            try
            {
                if (text.Contains(falsecharacter))
                {
                    ((Entry) sender).Text = e.OldTextValue;
                    return;
                }
                value = Convert.ToDouble(text);
            }
            catch
            {
                ((Entry) sender).Text = e.OldTextValue;
                return;
            }

            var index = text.IndexOf(decimalChar, StringComparison.Ordinal);
            if (index != -1 && index + DecimalDigitCount < text.Length)
            {
                if (value > Max || value < Min)
                {
                    ((Entry) sender).Text = e.OldTextValue;
                }
                else
                    ((Entry) sender).Text = text.Substring(0, index + DecimalDigitCount + 1);
            }
            else if (value > Max || value < Min)
            {
                ((Entry) sender).Text = e.OldTextValue;
            }
        }

        /// <summary>
        /// OnDetachingFrom
        /// </summary>
        /// <param name="bindable"></param>
        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= bindable_TextChanged;
        }
    }

    /// <summary>
    /// DecimalValidator
    /// </summary>
    public class DecimalValidator : Behavior<Entry>
    {
        /// <summary>
        /// OnAttachedTo
        /// </summary>
        /// <param name="bindable"></param>
        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += bindable_TextChanged;
        }

        /// <summary>
        /// bindable_TextChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bindable_TextChanged(object sender, TextChangedEventArgs e)
        {
            var decimalChar = Convert.ToString(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

            if (string.IsNullOrEmpty(e.NewTextValue) || e.NewTextValue == decimalChar)
                return;

            if (!string.IsNullOrEmpty(e.NewTextValue) && e.NewTextValue.Contains(" "))
            {
                ((Entry) sender).Text = e.OldTextValue;
                return;
            }

            if (e.NewTextValue == "-" || e.NewTextValue == string.Format("-{0}", decimalChar))
            {
                ((Entry) sender).Text = string.Empty;
                return;
            }

            try
            {
                HACCPUtil.ConvertToDouble(e.NewTextValue);
            }
            catch
            {
                ((Entry) sender).Text = e.OldTextValue;
            }
        }

        /// <summary>
        /// OnDetachingFrom
        /// </summary>
        /// <param name="bindable"></param>
        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= bindable_TextChanged;
        }
    }
}