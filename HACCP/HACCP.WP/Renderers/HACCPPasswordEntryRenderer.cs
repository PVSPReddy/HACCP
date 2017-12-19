using Windows.System;
using Windows.UI.Xaml.Input;
using HACCP.WP.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinRT;
using HACCP;

[assembly: ExportRenderer(typeof(HACCPPasswordEntry), typeof(HACCPPasswordEntryRenderer))]

namespace HACCP.WP.Renderers
{
    public class HACCPPasswordEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.KeyUp += Control_KeyUp;
                Control.KeyDown += Control_KeyDown;

                var scope = new InputScope();
                var name = new InputScopeName();

                name.NameValue = InputScopeNameValue.Number;
                scope.Names.Add(name);

                Control.InputScope = scope;
            }
        }


        private void Control_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            int maxLength = ((HACCPPasswordEntry) Element).MaxLength;
            if (Control.Text.Length >= maxLength && e.Key != VirtualKey.Back)
            {
                e.Handled = true;
            }
        }

        private void Control_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            var val = Control.Text;
            var maxLength = 0;

            if (Element != null)
            {
                maxLength = ((HACCPPasswordEntry) Element).MaxLength;
            }

            if (maxLength > 0)
            {
                if (!string.IsNullOrEmpty(val) && val.Length >= maxLength)
                {
                    Control.Text = Control.Text.Substring(0, Control.Text.Length);
                }
            }
        }
    }
}