using System;
using HACCP;
using HACCP.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Switch = Android.Widget.Switch;

[assembly: ExportRenderer(typeof(HACCPSwitch), typeof(HACCPSwitchRenderer))]

namespace HACCP.Droid
{
    public class HACCPSwitchRenderer : ViewRenderer<HACCPSwitch, Switch>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<HACCPSwitch> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null)
            {
                Element.Toggled -= ElementToggled;
                return;
            }

            if (Element == null)
            {
                return;
            }

            var switchControl = new Switch(Forms.Context)
            {
                TextOn = Element.TextOn,
                TextOff = Element.TextOff
            };

            switchControl.CheckedChange += ControlValueChanged;
            Element.Toggled += ElementToggled;

            SetNativeControl(switchControl);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Control.CheckedChange -= ControlValueChanged;
                Element.Toggled -= ElementToggled;
            }

            base.Dispose(disposing);
        }

        private void ElementToggled(object sender, ToggledEventArgs e)
        {
            Control.Checked = Element.IsToggled;
        }

        private void ControlValueChanged(object sender, EventArgs e)
        {
            Element.IsToggled = Control.Checked;
        }
    }
}