using HACCP;
using HACCP.WP.Renderers;
using Xamarin.Forms.Platform.WinRT;

[assembly: ExportRenderer(typeof(BaseView), typeof(HACCPPageRenderer))]

namespace HACCP.WP.Renderers
{
    public class HACCPPageRenderer : PageRenderer
    {
        public int i = 0;
    }
}