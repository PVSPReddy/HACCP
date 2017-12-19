using Xamarin.Forms;

namespace HACCP
{
    public class RequiredValidatorBehavior : Behavior<Entry>
    {

        private static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid",
            typeof(bool), typeof(RequiredValidatorBehavior), false);

        private static readonly BindableProperty IsValidProperty = IsValidPropertyKey.BindableProperty;

        /// <summary>
        /// IsValid
        /// </summary>
        public bool IsValid
        {
            get { return (bool) GetValue(IsValidProperty); }
            private set { SetValue(IsValidPropertyKey, value); }
        }

        /// <summary>
        /// OnAttachedTo
        /// </summary>
        /// <param name="bindable"></param>
        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.Unfocused += HandleFocusChanged;

            base.OnAttachedTo(bindable);
        }

        /// <summary>
        /// OnDetachingFrom
        /// </summary>
        /// <param name="bindable"></param>
        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.Unfocused -= HandleFocusChanged;
            base.OnDetachingFrom(bindable);
        }

        /// <summary>
        /// HandleFocusChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleFocusChanged(object sender, FocusEventArgs e)
        {
            IsValid = !string.IsNullOrEmpty(((Entry) sender).Text);
        }
    }
}