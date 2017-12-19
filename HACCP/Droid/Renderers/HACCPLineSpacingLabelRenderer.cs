using HACCP;
using HACCP.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HACCPLineSpacingLabel), typeof(HACCPLineSpacingLabelRenderer))]

namespace HACCP.Droid
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

            //var lineSpacing = this.LineSpacingLabel.LineSpacing;

            var lineSpacing = 1.4;

            Control.SetLineSpacing(1f, (float) lineSpacing);
            Control.SetPadding(30, 0, 30, 0);
            UpdateLayout();
        }
    }
}