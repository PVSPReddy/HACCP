using HACCP.WP.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinRT;
using Application = Windows.UI.Xaml.Application;
using Style = Windows.UI.Xaml.Style;
using HACCP;

[assembly: ExportRenderer(typeof(HACCPHomePageButton), typeof(HACCPHomePageButtonRenderer))]

namespace HACCP.WP.Renderers
{
    public class HACCPHomePageButtonRenderer : ButtonRenderer
    {
        private Button customControl;
        private FormsButton nativeControl;

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                Style style;
                customControl = e.NewElement;
                nativeControl = new FormsButton();
                if ((e.NewElement as HACCPHomePageButton).RemoveBorderOnClick)
                    style = Application.Current.Resources["BorderLessButtonStyle"] as Style;
                else
                    style = Application.Current.Resources["HomeButtonStyle"] as Style;
                nativeControl.Style = style;
                nativeControl.Command = customControl.Command;
                nativeControl.CommandParameter = customControl.CommandParameter;
                SetNativeControl(nativeControl);
            }
        }

        //    {
        //    Element.Animate("btn", new Xamarin.Forms.Animation((e) =>
        //{


        //public void Clicked(object sender, EventArgs args)
        //        Element.BorderColor = Color.FromHex("#6292A4");
        //        Element.BorderWidth = 1.3;
        //    }, 0, 10), 16, 1000, null, (a, b) =>
        //    {
        //        Element.BorderColor = Color.Transparent;
        //        Element.BorderWidth = 0;
        //    });
        //}
    }
}