using Xamarin.Forms;

namespace HACCP
{
    /// <summary>
    ///     Extends <see cref="Xamarin.Forms.Button" />.
    /// </summary>
    public class ExtendedButton : Button
    {
        /// <summary>
        ///     Bindable property for button content vertical alignment.
        /// </summary>
        public static readonly BindableProperty VerticalContentAlignmentProperty =
            BindableProperty.Create<ExtendedButton, TextAlignment>(
                p => p.VerticalContentAlignment, TextAlignment.Center);

        /// <summary>
        ///     Bindable property for button content horizontal alignment.
        /// </summary>
        public static readonly BindableProperty HorizontalContentAlignmentProperty =
            BindableProperty.Create<ExtendedButton, TextAlignment>(
                p => p.HorizontalContentAlignment, TextAlignment.Center);

        #region Methods

        /// <summary>
        ///     Gets or sets the content vertical alignment.
        /// </summary>
        public TextAlignment VerticalContentAlignment
        {
            get { return this.GetValue<TextAlignment>(VerticalContentAlignmentProperty); }
            set { SetValue(VerticalContentAlignmentProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the content horizontal alignment.
        /// </summary>
        public TextAlignment HorizontalContentAlignment
        {
            get { return this.GetValue<TextAlignment>(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }

        #endregion
    }
}