using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using Foundation;
using HACCP;
using HACCP.Core;
using HACCP.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HACCPTemperatureEntry), typeof(HACCPTemperatureEntryRenderer))]

namespace HACCP.iOS
{
    public class HACCPTemperatureEntryRenderer : EntryRenderer
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
            textField.BorderStyle = UITextBorderStyle.Line;
            var paddingView = new UIView(new RectangleF(0, 0, 7, 20));
            textField.LeftView = paddingView;
            textField.LeftViewMode = UITextFieldViewMode.Always;

            //Border
            textField.Layer.BorderWidth = 1;
            textField.Layer.CornerRadius = 4;
            textField.Layer.BorderColor = UIColor.FromRGB(202, 223, 233).CGColor;

            //Colors
            textField.Layer.BackgroundColor = UIColor.FromRGBA(255, 255, 255, 20).CGColor;
            textField.TintColor = UIColor.FromCGColor(Color.Transparent.ToCGColor());

            //Font size
            var fontSize = UIScreen.MainScreen.Bounds.Width > 500.0 ? 35 : 24;

            textField.Font = UIFont.FromName("Arial", fontSize);

            if (string.IsNullOrEmpty(textField.Placeholder) == false)
            {
                var placeholderString = new NSAttributedString(textField.Placeholder,
                    new UIStringAttributes {ForegroundColor = UIColor.FromRGBA(255, 255, 255, 150)});
                textField.AttributedPlaceholder = placeholderString;
            }

            //Show Done button if entry is last item in the form otherwise shows Next button
            //if (this.Element != null &&(this.Element as HACCPTemperatureEntry).IsLastItem)
            textField.ReturnKeyType = UIReturnKeyType.Done;
            //else
            //	textField.ReturnKeyType = UIReturnKeyType.Next;

            //Show done button if keyboard is numeric
            if (Element != null && Element.Keyboard == Keyboard.Numeric)
            {
                AddDoneButton();
            }
        }

        /// <summary>
        ///     Add toolbar with Done button
        /// </summary>
        protected void AddDoneButton()
        {
            var toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));

            var doneButton = new UIBarButtonItem(HACCPUtil.GetResourceString("Done"), UIBarButtonItemStyle.Plain,
                delegate { Control.ResignFirstResponder(); });

            var haccpTemperatureEntry = Element as HACCPTemperatureEntry;
            if (haccpTemperatureEntry != null && Element != null && haccpTemperatureEntry.ShowPlusMinus)
            {
                var decimalChar = Convert.ToString(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                var plussMinusButton = new UIBarButtonItem("  +/-  ", UIBarButtonItemStyle.Plain, delegate
                {
                    if (!string.IsNullOrEmpty(Element.Text) && Element.Text != decimalChar)
                    {
                        double val;

                        if (double.TryParse(Element.Text, out val))
                        {
                            if (val != 0)
                            {
                                if (!Element.Text.Contains("-"))
                                {
                                    Element.Text = "-" + Element.Text;
                                }
                                else if (Element.Text.Contains("-"))
                                {
                                    Element.Text = Element.Text.Replace("-", "");
                                }
                            }
                        }
                    }
                });

                if (UIScreen.MainScreen.Bounds.Width > 500.0)
                {
                    toolbar.Items = new[]
                    {
                        new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                        plussMinusButton
                    };
                }
                else
                {
                    var space = (float) (UIScreen.MainScreen.Bounds.Width - 140);

                    toolbar.Items = new[]
                    {
                        new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                        plussMinusButton,
                        new UIBarButtonItem(new UIView(new RectangleF(0.0f, 0.0f, space, 44.0f))),
                        doneButton
                    };
                }

                plussMinusButton.TintColor = doneButton.TintColor = UIColor.Black;
            }
            else
            {
                toolbar.Items = new[]
                {
                    new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                    doneButton
                };

                doneButton.TintColor = UIColor.Black;
            }

            Control.InputAccessoryView = toolbar;
        }

        /// <summary>
        ///     The on element property changed callback
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs" /> instance containing the event data.</param>
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "IsFocused")
            {
                if (Control != null && Element != null)
                {
                    Control.Layer.BorderColor = Element.IsFocused
                        ? UIColor.FromRGB(255, 215, 0).CGColor
                        : UIColor.FromRGB(202, 223, 233).CGColor;
                }
            }
        }
    }
}