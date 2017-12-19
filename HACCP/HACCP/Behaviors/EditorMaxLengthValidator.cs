using Xamarin.Forms;

namespace HACCP
{
    /// <summary>
    /// EditorMaxLengthValidator
    /// </summary>
    public class EditorMaxLengthValidator : Behavior<Editor>
    {
        public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create("MaxLength", typeof(int),
            typeof(EditorMaxLengthValidator), 0);

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
        protected override void OnAttachedTo(Editor bindable)
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
            //if (MaxLength != null && MaxLength.HasValue)
            if (!string.IsNullOrEmpty(e.NewTextValue) && e.NewTextValue.Length > 0 && e.NewTextValue.Length > MaxLength)
                ((Editor) sender).Text = e.NewTextValue.Substring(0, MaxLength);
        }


        /// <summary>
        /// OnDetachingFrom
        /// </summary>
        /// <param name="bindable"></param>
        protected override void OnDetachingFrom(Editor bindable)
        {
            bindable.TextChanged -= bindable_TextChanged;
        }
    }
}