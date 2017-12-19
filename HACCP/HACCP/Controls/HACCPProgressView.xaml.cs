using HACCP.Core;
using Xamarin.Forms;

namespace HACCP
{
	public partial class HACCPProgressView : ContentView
	{
		public HACCPProgressView()
		{
			InitializeComponent();

			Indicator.Scale = Device.OS == TargetPlatform.Android ? 1 : 2;
			contentgrid.HeightRequest = Device.OS == TargetPlatform.Android ? 80 : 70;

		
		}
	}
}