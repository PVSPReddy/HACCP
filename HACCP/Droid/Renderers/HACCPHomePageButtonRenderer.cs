using System;
using HACCP;
using HACCP.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HACCPHomePageButton), typeof(HACCPHomePageButtonRenderer))]

namespace HACCP.Droid
{
    public class HACCPHomePageButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            var btn = Element as HACCPHomePageButton;
            if (btn != null && !btn.RemoveBorderOnClick)
                Element.Clicked += Clicked;
            //if (Control != null) {
            //Control.Click += Click;
            //Control.ContextClick += ContextClick;
            //Control.LongClick += LongClick;
            //}
        }


//		public void Click (object sender, EventArgs args)
//		{
//			if (Element != null) {
//				Element.Animate ("btn", new Xamarin.Forms.Animation ((e) => {
//					Element.BorderColor = Color.FromHex ("#2D5E70");
//					Element.BorderWidth = 1.3;
//				}, 0, 10), 16, 3000, null, (a, b) => {
//					//Element.BorderColor = Color.Transparent;
//					//Element.BorderWidth = 0;
//				});
//
//
//				Element.Command.Execute (null);
//				Element.BorderColor = Color.Transparent;
//				Element.BorderWidth = 0;
//			}
//			
//		}


        public void Clicked(object sender, EventArgs args)
        {
            Element.Animate("btn", new Animation(e =>
            {
                Element.BorderColor = Color.FromHex("#6292A4");
                Element.BorderWidth = 1.3;
            }, 0, 10), 16, 1000, null, (a, b) =>
            {
                Element.BorderColor = Color.Transparent;
                Element.BorderWidth = 0;
            });
        }
    }
}