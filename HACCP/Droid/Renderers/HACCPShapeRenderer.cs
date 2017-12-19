using HACCP;
using HACCP.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HACCPShapeView), typeof(HACCPShapeRenderer))]

namespace HACCP.Droid
{
    public class HACCPShapeRenderer : ViewRenderer<HACCPShapeView, Shape>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<HACCPShapeView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
                return;

//			if ((Element as HACCPShapeView).ShapeType == ShapeType.Box)
//				Element.CornerRadius = 4;

            SetNativeControl(new Shape(Resources.DisplayMetrics.Density, Context)
            {
                ShapeView = Element
            });
        }
    }
}