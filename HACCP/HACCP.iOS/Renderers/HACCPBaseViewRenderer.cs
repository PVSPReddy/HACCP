using HACCP;
using HACCP.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BaseLayout), typeof(HACCPBaseViewRenderer))]

namespace HACCP.iOS
{
    public class HACCPBaseViewRenderer : VisualElementRenderer<BaseLayout>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<BaseLayout> e)
        {
            base.OnElementChanged(e);

            if (Xamarin.Forms.Device.Idiom == TargetIdiom.Tablet)
                BackgroundColor = UIColor.FromPatternImage(UIImage.FromFile("bg1024.png"));
        }
    }
}