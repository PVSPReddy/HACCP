using HACCP;
using HACCP.WP.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinRT;
using Thickness = Windows.UI.Xaml.Thickness;

[assembly: ExportRenderer(typeof(HACCPLineSpacingLabel), typeof(HACCPLineSpacingLabelRenderer))]

namespace HACCP.WP.Renderers
{
    public class HACCPLineSpacingLabelRenderer : LabelRenderer
    {
        protected HACCPLineSpacingLabel LineSpacingLabel { get; private set; }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                LineSpacingLabel = (HACCPLineSpacingLabel) Element;
            }

            Control.Padding = new Thickness(30, 0, 30, 0);
            UpdateLayout();
        }
    }
}