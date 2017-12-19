using System.ComponentModel;
using Android.Views;
using Android.Views.InputMethods;
using HACCP;
using HACCP.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

//[assembly: ExportRenderer(typeof(Entry), typeof(DefaultEntryRenderer))]

[assembly: ExportRenderer(typeof(HACCPSearch), typeof(HACCPSearchRenderer))]

namespace HACCP.Droid
{
    public class HACCPSearchRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                //Control.SetTextColor(Android.Graphics.Color.Rgb(255,255,255));

                Control.SetPadding(15, 0, 15, 0);

                Control.ImeOptions = ImeAction.Done;

                Control.SetImeActionLabel("Search", ImeAction.Done);

				Control.Gravity = GravityFlags.CenterVertical;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Control != null)
                Control.SetBackgroundResource(Resource.Drawable.SearchEntry);
        }
    }

    //	public class DefaultEntryRenderer :EntryRenderer
    //	{
    //		protected override void OnElementChanged (ElementChangedEventArgs<Entry> e)
    //		{
    //			base.OnElementChanged (e);
    //			if (Control != null) {
    //				Control.SetHintTextColor (Android.Graphics.Color.DarkSlateGray);
    //			}
    //		}
    //
    //		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
    //		{
    //			if (Control != null) {
    //				Control.SetBackgroundResource (HACCP.Droid.Resource.Drawable.RoundedEntry);
    //			}
    //		}
    //	
    //	}
}