using System.Diagnostics;
using HACCP.Core;
using Xamarin.Forms;

namespace HACCP
{
    public partial class ThermometerMode : BaseView
    {

        /// <summary>
        /// ThermometerMode
        /// </summary>
        public ThermometerMode()
        {
            InitializeComponent();
            if (Device.Idiom == TargetIdiom.Tablet)
            {
                ProbeGrid.MinimumHeightRequest = 80;
                Connection_Button.WidthRequest = 330;
                CircleGrid.MinimumHeightRequest = 100;
            }
            else
            {
                ProbeGrid.MinimumHeightRequest = 70;
                Connection_Button.WidthRequest = 230;
                CircleGrid.MinimumHeightRequest = 80;
            }
            NavigationPage.SetBackButtonTitle(this, string.Empty);


            var viewModel = new ThermometerModeViewModel(this);
            BindingContext = viewModel;
        }

        /// <summary>
        /// OnSizeAllocated 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            Debug.WriteLine(width > height ? "Landscape" : "Portrait");


            if (Device.Idiom == TargetIdiom.Tablet)
            {
                CircleGrid.WidthRequest = 100;
                CircleGrid.HeightRequest = 100;
                ProbeGrid.HeightRequest = 80;
            }
            else
            {
                CircleGrid.WidthRequest = 80;
                CircleGrid.HeightRequest = 80;
                ProbeGrid.HeightRequest = 70;
            }
        }
        
        /// <summary>
        /// OnAppearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.CurrentPageType = typeof(ThermometerMode);
        }
    }
}