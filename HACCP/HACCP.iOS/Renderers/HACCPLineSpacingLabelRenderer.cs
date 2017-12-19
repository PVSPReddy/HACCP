using System;
using Xamarin.Forms;
using HACCP;
using HACCP.iOS;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using Foundation;


[assembly: ExportRendererAttribute (typeof(HACCPLineSpacingLabel), typeof(HACCPLineSpacingLabelRenderer))]
namespace HACCP.iOS
{
	public class HACCPLineSpacingLabelRenderer:LabelRenderer
	{
		protected override void OnElementChanged (ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged (e);
			// sample only; expand, validate and handle edge cases as needed
			//((UILabel)base.Control).Font.LineHeight = ((HACCPCustomLabel)this.Element).LineHeight;


			var lineSpacingLabel = (HACCPLineSpacingLabel)Element;

			if (lineSpacingLabel != null) {
				var paragraphStyle = new NSMutableParagraphStyle () {
					//LineSpacing = (nfloat)lineSpacingLabel.LineSpacing

					LineSpacing = (nfloat)(7.5),
					Alignment=UITextAlignment.Center,
				};


				var textstring = new NSMutableAttributedString (lineSpacingLabel.Text);
				var style = UIStringAttributeKey.ParagraphStyle;
		
				var range = new NSRange (0, textstring.Length);
				textstring.AddAttribute (style, paragraphStyle, range);
				Control.AttributedText = textstring;

			}
		}
	}
}

