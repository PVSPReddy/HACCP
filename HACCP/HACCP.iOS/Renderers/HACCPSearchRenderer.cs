using System.Drawing;
using HACCP;
using HACCP.Core.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HACCPSearch), typeof(HACCPSearchRenderer))]

namespace HACCP.Core.iOS
{
    /// <summary>
    ///     HACCP entry renderer class for iOS.
    /// </summary>
    public class HACCPSearchRenderer : EntryRenderer
    {
        /// <summary>
        ///     The on element changed callback.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            var textField = Control;

            //Entry Styling

            //Padding
            var paddingView = new UIView(new RectangleF(0, 0, 7, 20));
            textField.LeftView = paddingView;
            textField.LeftViewMode = UITextFieldViewMode.Always;

            //Border
            textField.Layer.CornerRadius = 4;

            //Colors
            textField.Layer.BackgroundColor = UIColor.White.CGColor;
            textField.TintColor = UIColor.DarkGray;
            textField.TextColor = UIColor.Black;

            //Font size
            textField.Font = UIFont.FromName("Arial", 14);

            textField.ReturnKeyType = UIReturnKeyType.Done;

            AddDoneButton();
        }

        /// <summary>
        ///     Add toolbar with Done button
        /// </summary>
        protected void AddDoneButton()
        {
            var toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));

            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done,
                delegate { Control.ResignFirstResponder(); });

            toolbar.Items = new[]
            {
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                doneButton
            };

            Control.InputAccessoryView = toolbar;
        }
    }
}