using System.ComponentModel;
using Android.Text;
using Android.Util;
using Android.Views.InputMethods;
using HACCP;
using HACCP.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HACCPEditor), typeof(HACCPEditorRenderer))]

namespace HACCP.Droid
{
    public class HACCPEditorRenderer : EditorRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                //Control.SetTextColor(Android.Graphics.Color.Rgb(255,255,255));

                Control.SetPadding(20, 10, 20, 0);
                Control.SetTextSize(ComplexUnitType.Sp, 14);
                Control.ImeOptions = ImeAction.None;

                if (Element != null && Element is HACCPEditor && (Element as HACCPEditor).AutoCorrectOff)
                    Control.InputType = InputTypes.TextFlagNoSuggestions;

                //if ((Element as HACCPEntry).IsLastItem) {
                //Control.SetImeActionLabel ("Done", Android.Views.InputMethods.ImeAction.Done);
                //} else {

                //Control.ImeOptions = Android.Views.InputMethods.ImeAction.Next;
                //Control.SetImeActionLabel ("Next", Android.Views.InputMethods.ImeAction.Done);
                //}
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (Control != null)
                Control.SetBackgroundResource(Resource.Drawable.RoundedEntry);
        }
    }
}