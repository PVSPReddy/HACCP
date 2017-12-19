using System.ComponentModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using HACCP.WP.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinRT;
using Thickness = Windows.UI.Xaml.Thickness;
using HACCP;

[assembly: ExportRenderer(typeof(HACCPEditor), typeof(HACCPEditorRenderer))]

namespace HACCP.WP.Renderers
{
    public class HACCPEditorRenderer : EditorRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {            
                Control.FontSize = 15;
                Control.Padding = new Thickness(5, 5, 5, 5);
                Control.TextWrapping = TextWrapping.Wrap;
                Control.Background = new SolidColorBrush(Colors.Transparent);
                Control.Foreground = new SolidColorBrush(Colors.White);

                Control.GotFocus += Control_GotFocus;
             
                Control.LostFocus += Control_LostFocus;
            }
        }

        void Control_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Control != null)
                Control.Foreground = new SolidColorBrush(Colors.White);
        }

        void Control_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Control != null)
                Control.Foreground = new SolidColorBrush(Colors.Black);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (Control != null)
            {
                //  Control.Background = new SolidColorBrush(Colors.Transparent);
                // Control.Foreground = new SolidColorBrush(Colors.White);
            }
        }
    }
}