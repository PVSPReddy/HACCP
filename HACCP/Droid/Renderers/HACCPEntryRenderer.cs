using System.ComponentModel;
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

//[assembly: ExportRenderer(typeof(Entry), typeof(DefaultEntryRenderer))]

[assembly: ExportRenderer(typeof(HACCPEntry), typeof(HACCPEntryRenderer))]

namespace HACCP.Droid
{
	public class HACCPEntryRenderer : EntryRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);

			if (Control != null)
			{
				//Control.SetTextColor(Android.Graphics.Color.Rgb(255,255,255));

				if (Element != null && ((HACCPEntry)Element).IsSearchbox)
				{
					Control.SetPadding(Xamarin.Forms.Device.Idiom == TargetIdiom.Tablet ? 50 : 70, 0, 20, 0);
					Control.Gravity = GravityFlags.CenterVertical;
				}
				else
				{
					Control.SetPadding(20, 0, 20, 0);
					Control.Gravity = GravityFlags.CenterVertical;
				}

				Control.SetHintTextColor(Color.LightGray);
				//	Element.VerticalOptions = LayoutOptions.Center;
				if (Element != null && ((HACCPEntry)Element).ShowPlusMinus && !((HACCPEntry)Element).NeedDot)
				{
					Control.SetRawInputType(InputTypes.ClassNumber | InputTypes.NumberFlagSigned);
				}


				var IsTablet = Xamarin.Forms.Device.Idiom == TargetIdiom.Tablet;
				float fontSize = IsTablet ? 20 : 15;


				Control.SetTextSize(ComplexUnitType.Sp, fontSize);

				Control.ImeOptions = ImeAction.Done;
				var haccpEntry = (HACCPEntry)Element;
				if (haccpEntry != null && haccpEntry.IsLastItem)
				{
					Control.SetImeActionLabel("Done", ImeAction.Done);
				}
				else
				{
					//Control.ImeOptions = Android.Views.InputMethods.ImeAction.Next;
					Control.SetImeActionLabel("Next", ImeAction.Done);
				}

				if (Element != null && ((HACCPEntry)Element).ShowPlusMinus)
				{
					var native = Control as EditText;
					native.InputType = InputTypes.ClassNumber | InputTypes.NumberFlagSigned |
									   InputTypes.NumberFlagDecimal;
				}
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (Control != null)
			{
				Control.SetBackgroundResource(Resource.Drawable.RoundedEntry);
			}
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