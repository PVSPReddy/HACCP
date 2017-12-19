using System;
using System.Threading.Tasks;
using HACCP.Core;
using Xamarin.Forms;

namespace HACCP
{
    public partial class RecordItem : BaseView
    {


        private readonly RecordTempViewModel _viewModel;


        /// <summary>
        /// RecordItem Screen Constructor
        /// </summary>
        /// <param name="selectedItem"></param>
        public RecordItem(object selectedItem)
        {
            InitializeComponent();

            Connection_Button.WidthRequest = Device.Idiom == TargetIdiom.Tablet ? 330 : 230;

            if (HaccpAppSettings.SharedInstance.IsWindows)
                TempLabelGrid.IsVisible = false;


            NavigationPage.SetBackButtonTitle(this, string.Empty);


            _viewModel = new RecordTempViewModel(this, selectedItem as LocationMenuItem);
            BindingContext = _viewModel;

            //Button btn;
            toggleBtn.Clicked += (sender, e) =>
            {
                if (_viewModel.IsManualEntryOn)
                    manualEntry.Focus();
            };

            manualEntry.Focused += (sender, e) =>
            {
                if (Device.Idiom != TargetIdiom.Tablet)
                    bottomgrid.IsVisible = bottomline.IsVisible = false;

                if (HaccpAppSettings.SharedInstance.IsWindows)
                {
                    if (!string.IsNullOrEmpty(manualEntry.Text))
                    {
                        string value = manualEntry.Text;
                        value = value.Replace(_viewModel.UnitString, string.Empty);
                        manualEntry.Text = value;
                    }
                }
            };


            editorcontrol.Focused += (sender, e) => { bottomgrid.IsVisible = bottomline.IsVisible = false; };

            manualEntry.Unfocused += (sender, e) =>
            {
                //make visible bottom control after the entry keyboard dismissal
                Task.Delay(200).ContinueWith((t, s) =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        if (!editorcontrol.IsFocused)
                            bottomgrid.IsVisible = bottomline.IsVisible = true;
                    });
                }, new object());


                if (HaccpAppSettings.SharedInstance.IsWindows)
                {
                    if (!string.IsNullOrEmpty(manualEntry.Text))
                    {
                        string value = manualEntry.Text;
                        value = value.Replace(_viewModel.UnitString, string.Empty);
                        manualEntry.Text = string.Format("{0}{1}", value, _viewModel.UnitString);
                    }
                }
            };

            editorcontrol.Unfocused += (sender, e) =>
            {
                //make visible bottom control after the entry keyboard dismissal
                Task.Delay(200).ContinueWith((t, s) =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        if (!manualEntry.IsFocused)
                            bottomgrid.IsVisible = bottomline.IsVisible = true;
                    });
                }, new object());
            };


            AddToolBarButton("home.png", _viewModel.HomeCommand);


            MessagingCenter.Subscribe<RecordSaveCompleteToast>(this, HaccpConstant.ToastMessage, sender =>
            {
                editorcontrol.IsEnabled = false;
                var second = 1;
                ToastFrame.IsVisible = true;
                ToastFrame.Opacity = 1;
                var timerOn = true;
                Device.StartTimer(new TimeSpan(0, 0, 0, second), () =>
                {
                    second++;
                    if (second == 3)
                        ToastFrame.FadeTo(0, 250, Easing.Linear);
                    if (second == 4)
                    {
                        _viewModel.CheckAutoAdvance();
                        timerOn = false;
                    }
                    return timerOn;
                });
            });
        }

        #region Methods

        /// <param name="width">The new width of the element.</param>
        /// <param name="height">The new height of the element.</param>
        /// <summary>
        ///     Raises the size allocated event.
        /// </summary>
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (Device.Idiom == TargetIdiom.Tablet)
            {
                CircleGrid.HeightRequest = CircleGrid.WidthRequest = 120;
            }
            else
            {
                CircleGrid.HeightRequest = CircleGrid.WidthRequest = 90;
            }
        }

        /// <summary>
        /// OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<RecordSaveCompleteToast>(this, HaccpConstant.ToastMessage);
        }

        /// <summary>
        /// OnAppearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.CurrentPageType = typeof(RecordItem);
        }

        #endregion
    }
}