using Android.Views;
using HACCP;
using HACCP.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HACCPPasswordEntry), typeof(HACCPPasswordEntryRenderer))]

namespace HACCP.Droid
{
    public class HACCPPasswordEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
			if (Control != null)
			{
				Control.SetBackgroundResource(Resource.Drawable.RoundedEntry);
				Control.Gravity = GravityFlags.CenterVertical;
			}
        }
    }
}