using System.ComponentModel;
using HACCP.WP.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinRT;
using Application = Windows.UI.Xaml.Application;
using Style = Windows.UI.Xaml.Style;
using Thickness = Windows.UI.Xaml.Thickness;
using HACCP;
using Windows.UI.Xaml.Media;
using Windows.UI.Text;


//[assembly: ExportRenderer(typeof(HACCPDefaultButton), typeof(HACCPDefaultButtonRenderer))]
[assembly: ExportRenderer(typeof(HACCPButton), typeof(HACCPButtonRenderer))]

namespace HACCP.WP.Renderers
{
    public class HACCPButtonRenderer : ButtonRenderer
    {
        private Button customControl;
        private FormsButton nativeControl;

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (Control != null && Element != null)
            {
                Control.BorderThickness = new Thickness(1);
                Control.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                Control.FontWeight = FontWeights.Normal;
                Control.BorderRadius = 5;
                Control.FontSize = 15;
                if (((HACCPButton)Element).IsEnabled)
                {
                    Control.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 252, 245, 195));
                    Control.BackgroundColor = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 253, 219, 0));
                }
                else
                {
                    Control.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 153, 179, 165));
                    Control.BackgroundColor = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 153, 166, 67));
                }
            }           
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Control != null && Element != null)
            {
                Control.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                if (((HACCPButton)Element).IsEnabled)
                {
                    Control.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 252, 245, 195));
                    Control.BackgroundColor = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 253, 219, 0));                  
                }
                else
                {
                    Control.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 153, 179, 165));
                    Control.BackgroundColor = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 153, 166, 67));                  
                }
            }
        }
    }


    //public class HACCPDefaultButtonRenderer : ButtonRenderer
    //{
    //    private Button customControl;
    //    private FormsButton nativeControl;

    //    protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
    //    {
    //        base.OnElementChanged(e);

    //        if (Control != null && Element != null)
    //        {
    //            Control.BorderThickness = new Thickness(1);
    //            Control.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
    //            Control.FontWeight = FontWeights.Normal;
    //            Control.BorderRadius = 5;
    //            Control.FontSize = 15;
    //            if (((HACCPDefaultButton)Element).IsEnabled)
    //            {
    //                Control.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 252, 245, 195));
    //                Control.BackgroundColor = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 253, 219, 0));
    //            }
    //            else
    //            {
    //                Control.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 153, 179, 165));
    //                Control.BackgroundColor = new SolidColorBrush(Windows.UI.Colors.Blue);
    //            }
    //        }
    //    }

    //    protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
    //    {
    //        base.OnElementPropertyChanged(sender, e);

    //        if (Control != null && Element != null)
    //        {
    //            Control.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
    //            if (((HACCPDefaultButton)Element).IsEnabled)
    //            {
    //                Control.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 252, 245, 195));
    //                Control.BackgroundColor = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 253, 219, 0));
    //            }
    //            else
    //            {
    //                Control.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 153, 179, 165));
    //                Control.BackgroundColor = new SolidColorBrush(Windows.UI.Colors.Blue);
    //            }
    //        }
    //    }
    //}
}