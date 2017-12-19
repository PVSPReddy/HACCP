using System;
using HACCP.Core;
using Xamarin.Forms;

namespace HACCP
{
    public class MaxLengthValidator : Behavior<Entry>
    {
        public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create("MaxLength", typeof(int),
            typeof(MaxLengthValidator), 0);


        /// <summary>
        /// MaxLength
        /// </summary>
        public int MaxLength
        {
            get { return (int) GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
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
            try
            {
                //if (MaxLength != null && MaxLength.HasValue)
                if (e.NewTextValue.Length > 0 && e.NewTextValue.Length > MaxLength)
                    ((Entry) sender).Text = e.NewTextValue.Substring(0, MaxLength);
            }
            catch (Exception ex)
            {
                // ignored
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