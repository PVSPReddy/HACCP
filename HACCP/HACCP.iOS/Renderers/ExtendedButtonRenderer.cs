using System.ComponentModel;
using HACCP;
using HACCP.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ExtendedButton), typeof(ExtendedButtonRenderer))]

namespace HACCP.iOS
{
    /// <summary>
    ///     Class ExtendedButtonRenderer.
    /// </summary>
    public class ExtendedButtonRenderer : ButtonRenderer
    {
        /// <summary>
        ///     Gets the element.
        /// </summary>
        /// <value>The element.</value>
        public new ExtendedButton Element
        {
            get { return base.Element as ExtendedButton; }
        }

        /// <summary>
        ///     Called when [element changed].
        /// </summary>
        /// <param name="e">The e.</param>
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            var element = Element;

            if (element == null || Control == null)
            {
                return;
            }

            Control.VerticalAlignment = Element.VerticalContentAlignment.ToContentVerticalAlignment();
            Control.HorizontalAlignment = Element.HorizontalContentAlignment.ToContentHorizontalAlignment();
        }

        /// <summary>
        ///     Handles the <see cref="E:ElementPropertyChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs" /> instance containing the event data.</param>
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "VerticalContentAlignment":
                    Control.VerticalAlignment = Element.VerticalContentAlignment.ToContentVerticalAlignment();
                    break;
                case "HorizontalContentAlignment":
                    Control.HorizontalAlignment = Element.HorizontalContentAlignment.ToContentHorizontalAlignment();
                    break;
                default:
                    base.OnElementPropertyChanged(sender, e);
                    break;
            }
        }
    }
}