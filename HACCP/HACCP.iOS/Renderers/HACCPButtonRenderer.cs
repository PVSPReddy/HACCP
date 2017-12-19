using System.ComponentModel;
using HACCP;
using HACCP.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HACCPButton), typeof(HACCPButtonRenderer))]

namespace HACCP.iOS
{
    /// <summary>
    ///     HACCP button renderer class for iOS.
    /// </summary>
    public class HACCPButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            var btnField = Control;

            btnField.Layer.BorderWidth = Styles.IsTablet ? 2 : 1;
            btnField.Layer.CornerRadius = Styles.IsTablet ? 6 : 4;


            btnField.SetTitleColor(UIColor.Black, UIControlState.Normal);

            var fontSize = Styles.IsTablet ? 20 : 15;

            //Font size
            btnField.Font = UIFont.FromName("Arial", fontSize);


            btnField.SetTitleColor(UIColor.Black, UIControlState.Normal);
            btnField.SetTitleColor(UIColor.DarkGray, UIControlState.Disabled);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            var btnField = Control;


            if (((HACCPButton) sender).IsEnabled)
            {
                btnField.Layer.BorderColor = UIColor.FromRGB(252, 245, 195).CGColor;
                btnField.Layer.BackgroundColor = UIColor.FromRGB(253, 219, 0).CGColor;
            }
            else
            {
                btnField.Layer.BorderColor = UIColor.FromRGB(153, 179, 165).CGColor;
                btnField.Layer.BackgroundColor = UIColor.FromRGB(153, 166, 67).CGColor;
            }
        }
    }
}