using System.ComponentModel;
using Android.Util;
using HACCP;
using HACCP.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Android.Graphics.Color;

[assembly: ExportRenderer(typeof(HACCPButton), typeof(HACCPButtonRenderer))]

namespace HACCP.Droid
{
    public class HACCPButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);


            var IsTablet = Xamarin.Forms.Device.Idiom == TargetIdiom.Tablet;
            float fontSize = IsTablet ? 20 : 15;

            Control.SetTextSize(ComplexUnitType.Sp, fontSize);
            Control.SetAllCaps(false);

            if (Control != null)
            {
                if (Element.IsEnabled)
                {
                    Control.SetTextColor(Color.White);
                    Control.SetBackgroundResource(Resource.Drawable.RoundedButton);
                }
                else
                {
                    Control.SetTextColor(Color.White);
                    Control.SetBackgroundResource(Resource.Drawable.RoundedButtonDisabled);
                }
                Control.SetPadding(50, 5, 50, 5);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Control != null)
            {
                if (Element.IsEnabled)
                {
                    Control.SetTextColor(Color.White);
                    Control.SetBackgroundResource(Resource.Drawable.RoundedButton);
                }
                else
                {
                    Control.SetTextColor(Color.White);
                    Control.SetBackgroundResource(Resource.Drawable.RoundedButtonDisabled);
                }
            }
        }
    }
}