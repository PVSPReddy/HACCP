using HACCP;
using HACCP.WP.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinRT;
using Thickness = Windows.UI.Xaml.Thickness;

[assembly: ExportRenderer(typeof(HACCPFrame), typeof(HACCPFrameRenderer))]

namespace HACCP.WP.Renderers
{
    public class HACCPFrameRenderer : FrameRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            Control.BorderThickness = new Thickness(1);
            Element.OutlineColor = Color.FromRgb(202, 221, 233);
        }
    }
}