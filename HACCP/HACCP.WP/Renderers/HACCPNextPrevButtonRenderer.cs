using Windows.UI.Xaml;
using HACCP.WP.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinRT;
using Application = Windows.UI.Xaml.Application;
using Style = Windows.UI.Xaml.Style;
using HACCP;
using HACCP.Core;

[assembly: ExportRenderer(typeof(HACCPNextPrevButton), typeof(HACCPNextPrevButtonRenderer))]

namespace HACCP.WP.Renderers
{
    public class HACCPNextPrevButtonRenderer : ButtonRenderer
    {
        private Button customControl;
        private FormsButton nativeControl;

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                customControl = e.NewElement;
                nativeControl = new FormsButton();

                var style = Application.Current.Resources["NextPrevButtonStyle"] as Style;

                nativeControl.Style = style;
                nativeControl.Command = customControl.Command;
                nativeControl.CommandParameter = customControl.CommandParameter;
                nativeControl.Content = customControl.Text;
                nativeControl.Click += nativeControl_Click;
                nativeControl.IsEnabled = customControl.IsEnabled;
                SetNativeControl(nativeControl);
            }
        }


        private void nativeControl_Click(object sender, RoutedEventArgs e)
        {
            if (Element != null)
            {
                var val = (Element as HACCPNextPrevButton).IsNext;
                MessagingCenter.Send(new NextPrevButtonClickMessage(val), HaccpConstant.NextPrevMessage);
            }
        }
    }
}