using System.Drawing;
using HACCP;
using HACCP.Core;
using HACCP.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HACCPEditor), typeof(HACCPEditorRenderer))]

namespace HACCP.iOS
{
    public class HACCPEditorRenderer : EditorRenderer
    {
        /// <summary>
        ///     The on element changed callback.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            var textField = Control;

            //Entry Styling

            //Padding
            //textField.BorderStyle = UITextBorderStyle.Line;
            //UIView paddingView = new UIView (new RectangleF (0, 0, 7, 20));
            //textField.LeftView = paddingView;
            //textField.LeftViewMode = UITextFieldViewMode.Always;

            //Border
            textField.Layer.BorderWidth = (float) 0.4;
            textField.Layer.CornerRadius = 4;
            //textField.Layer.BorderColor = UIColor.FromRGB (202, 223, 233).CGColor;
            if (Element != null && Element.IsEnabled)
                textField.Layer.BorderColor = UIColor.FromRGB(202, 223, 233).CGColor;
            else
                textField.Layer.BorderColor = Color.FromHex("#a3b9c1").ToCGColor();
            //Colors
            textField.Layer.BackgroundColor = UIColor.FromRGBA(255, 255, 255, 20).CGColor;
            textField.TintColor = UIColor.FromRGB(202, 223, 233);

            var fontSize = UIScreen.MainScreen.Bounds.Width > 700.0 ? 20 : 15;

            //Font size
            textField.Font = UIFont.FromName("Arial", fontSize);

            Control.TextColor = UIColor.White;

            if (Element is HACCPEditor && (Element as HACCPEditor).AutoCorrectOff)
                Control.AutocorrectionType = UITextAutocorrectionType.No;


            //if (string.IsNullOrEmpty (textField.Placeholder) == false) {
            //NSAttributedString placeholderString = new NSAttributedString (textField.Placeholder, new UIStringAttributes (){ ForegroundColor = UIColor.FromRGBA (255, 255, 255, 150) });
            //textField.AttributedPlaceholder = placeholderString;
            //}

            //Show Done button if entry is last item in the form otherwise shows Next button
            //	if (this.Element != null &&(this.Element as HACCPEntry).IsLastItem)
            //textField.ReturnKeyType = UIReturnKeyType.Done;
            //else
            textField.ReturnKeyType = UIReturnKeyType.Default;
            AddDoneButton();

            //Show done button if keyboard is numeric
            if (Element != null && Element.Keyboard == Keyboard.Numeric)
            {
                //this.AddDoneButton ();
            }

            //Control.R
        }

        /// <summary>
        ///     Add toolbar with Done button
        /// </summary>
        protected void AddDoneButton()
        {
            var toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));

//			var doneButton = new UIBarButtonItem (UIBarButtonSystemItem.Done, delegate {
//				this.Control.ResignFirstResponder ();
//			});

            var doneButton = new UIBarButtonItem(HACCPUtil.GetResourceString("Done"), UIBarButtonItemStyle.Plain,
                delegate { Control.ResignFirstResponder(); });

            toolbar.Items = new[]
            {
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                doneButton
            };
            doneButton.TintColor = UIColor.Black;
            Control.InputAccessoryView = toolbar;
        }
    }
}