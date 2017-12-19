using HACCP;
using HACCP.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HACCPProgressBar), typeof(HACCPProgressBarRenderer))]

namespace HACCP.iOS
{
    public class HACCPProgressBarRenderer : ProgressBarRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ProgressBar> e)
        {
            base.OnElementChanged(e);
            var ctrl = Element as HACCPProgressBar;
            if (ctrl != null)
            {
                var color = ctrl.ProgresColor;
                if (!string.IsNullOrEmpty(color))
                {
                    Control.ProgressTintColor = Color.FromHex(color).ToUIColor();
                }
            }
        }
    }
}