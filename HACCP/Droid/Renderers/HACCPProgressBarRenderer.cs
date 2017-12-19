using HACCP;
using HACCP.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HACCPProgressBar), typeof(HACCPProgressBarRenderer))]

namespace HACCP.Droid
{
    public class HACCPProgressBarRenderer : ProgressBarRenderer
    {
    }
}