using HACCP.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ListView), typeof(HACCPListViewRenderer))]
[assembly: ExportRenderer(typeof(ViewCell), typeof(HACCPViewCellwRenderer))]

namespace HACCP.Droid
{
    public class HACCPListViewRenderer : ListViewRenderer
    {
    }


    public class HACCPViewCellwRenderer : ViewCellRenderer
    {
    }
}