using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using HACCP;
using HACCP.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Android.Graphics.Color;

[assembly: ExportRenderer(typeof(HACCPTemperatureEntry), typeof(HACCPTemperatureEntryRenderer))]

namespace HACCP.Droid
{
    public class HACCPTemperatureEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
//				Control.SetTextColor(Android.Graphics.Color.Rgb(255,255,255));

                Control.SetTextColor(Color.Transparent);

                Control.SetPadding(15, 0, 15, 0);
				Control.Gravity = GravityFlags.CenterVertical;

                //if (this.Element != null && (this.Element as HACCPEntry).ShowPlusMinus) {

                //Control.SetRawInputType(Android.Text.InputTypes.ClassNumber | Android.Text.InputTypes.NumberFlagSigned);

                //}
                Control.SetHintTextColor(Color.LightGray);
                Control.SetCursorVisible(false);

                var IsTablet = Xamarin.Forms.Device.Idiom == TargetIdiom.Tablet;
                float fontSize = IsTablet ? 35 : 26;


                Control.SetTextSize(ComplexUnitType.Sp, fontSize);
                Control.SetImeActionLabel("Done", ImeAction.Done);

                Control.SetBackgroundResource(Resource.Drawable.RoundedEntry);

                var native = Control as EditText;

                native.InputType = InputTypes.ClassNumber | InputTypes.NumberFlagSigned | InputTypes.NumberFlagDecimal;


                Control.FocusChange += FocusChanged;
            }
        }


        public void FocusChanged(object sender, FocusChangeEventArgs args)
        {
            Control.SetBackgroundResource(args.HasFocus
                ? Resource.Drawable.HighlightEntry
                : Resource.Drawable.RoundedEntry);
        }
    }
}