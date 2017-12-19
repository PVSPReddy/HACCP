using System.ComponentModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using HACCP.WP.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinRT;
using HACCP;

[assembly: ExportRenderer(typeof(HACCPWindowsProbeEntry), typeof(HACCPWindowsProbeEntryRenderer))]

namespace HACCP.WP.Renderers
{
    public class HACCPWindowsProbeEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null && Element != null)
            {

                Element.BackgroundColor = Xamarin.Forms.Color.Transparent;
                Element.TextColor = Xamarin.Forms.Color.White;
                Control.BorderBrush = new SolidColorBrush(Colors.White);
                Control.BorderThickness = new Windows.UI.Xaml.Thickness(1);
                Control.Height = 33;
                //  Control.BorderBrush = new SolidColorBrush(Colors.Red);

                //   Control.Foreground = new SolidColorBrush(Colors.White);
                //   Control.Background = new SolidColorBrush(Colors.Transparent);

                //Control.SetTextColor(Android.Graphics.Color.Rgb(255,255,255));


                Control.GotFocus += Control_GotFocus;
                Control.LostFocus += Control_LostFocus;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (Control != null && Element != null)
            {
                Element.BackgroundColor = Xamarin.Forms.Color.Transparent;
                Element.TextColor = Xamarin.Forms.Color.White;
                Control.BorderBrush = new SolidColorBrush(Colors.White);
                Control.BorderThickness = new Windows.UI.Xaml.Thickness(1);
                Control.VerticalContentAlignment = VerticalAlignment.Center;
            }
        }


        private void Control_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Control != null)
            {
                // Control.Foreground = new SolidColorBrush(Colors.White);
            }
        }

        private void Control_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Control != null)
            {
                // Control.Foreground = new SolidColorBrush(Colors.Black);
            }
        }
    }
}