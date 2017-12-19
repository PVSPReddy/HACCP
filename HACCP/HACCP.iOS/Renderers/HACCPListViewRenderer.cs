using HACCP.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ListView), typeof(HACCPListViewRenderer))]
[assembly: ExportRenderer(typeof(ViewCell), typeof(HACCPViewCellwRenderer))]

namespace HACCP.iOS
{
    public class HACCPListViewRenderer : ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);
            var tableView = Control;
            tableView.SectionIndexBackgroundColor = UIColor.Clear;
            tableView.SectionIndexTrackingBackgroundColor = UIColor.Clear;
            tableView.SectionIndexColor = UIColor.White;
        }
    }

    public class HACCPViewCellwRenderer : ViewCellRenderer
    {
        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            var cell = base.GetCell(item, reusableCell, tv);


            if (cell != null)
            {
                cell.BackgroundColor = UIColor.Clear;


                cell.SelectedBackgroundView = new UIView
                {
                    BackgroundColor =
                        string.IsNullOrEmpty(item.StyleId)
                            ? UIColor.FromRGB(98, 146, 164)
                            : UIColor.FromRGB(207, 224, 235)
                    // style id not empty means ==> highlight color must be light color (corrective actions,language list)
                };
            }

            return cell;
        }
    }
}