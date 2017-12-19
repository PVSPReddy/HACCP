using HACCP;
using HACCP.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HACCPDefaultButton), typeof(HACCPDefaultButtonRenderer))]

namespace HACCP.Droid
{
    public class HACCPDefaultButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            Control.SetAllCaps(false);
        }
    }
}