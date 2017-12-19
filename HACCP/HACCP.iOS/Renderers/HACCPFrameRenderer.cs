using HACCP;
using HACCP.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HACCPFrame), typeof(HACCPFrameRenderer))]

namespace HACCP.iOS
{
    public class HACCPFrameRenderer : FrameRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);


            NativeView.Layer.BorderColor = UIColor.FromRGB(202, 221, 233).CGColor;
            //this.NativeView.Layer.BorderColor = UIColor.White.CGColor ;
            NativeView.Layer.BorderWidth = 1;
            //Element.BackgroundColor = Color.Transparent;
        }
    }
}