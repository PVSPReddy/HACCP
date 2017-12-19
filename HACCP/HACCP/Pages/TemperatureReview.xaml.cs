using System;
using System.Diagnostics;
using System.Linq;
using HACCP.Core;
using Xamarin.Forms;

namespace HACCP
{
    public partial class TemperatureReview : BaseView
    {
        #region Member Variables

        private readonly TemperatureReviewViewModel _viewModel;
        private ItemTemperature _selectedItem;

        #endregion

        /// <summary>
        /// TemperatureReview Constructor
        /// </summary>
        /// <param name="location"></param>
        public TemperatureReview(object location)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, string.Empty);
            BindingContext = _viewModel = new TemperatureReviewViewModel(this, location as MenuLocation);
            ItemsListview.ItemSelected += ListItemSelected;
        }

        #region Methods

        /// <summary>
        ///     List item selected event handler.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void ListItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var record = (ItemTemperature) e.SelectedItem;
            if (record != null)
            {
                _selectedItem = record;
                ShowPopupData(record);
            }
            ItemsListview.SelectedItem = null;
        }

        /// <summary>
        /// PrevButtonClick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void PrevButtonClick(object sender, EventArgs args)
        {
            var list = _viewModel.Records;
            var item = list.FirstOrDefault(x => x.RecordNo == _selectedItem.RecordNo);
            var index = list.IndexOf(item);


            if (index == 1)
            {
                prevImage.Source = "prevDisable.png";
                prevButton.IsEnabled = false;
            }

            nextImage.Source = "next.png";
            nextButton.IsEnabled = true;

            _selectedItem = list[index - 1];

            ShowPopupData(_selectedItem);
        }

        /// <summary>
        /// NextButtonClick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void NextButtonClick(object sender, EventArgs args)
        {
            var list = _viewModel.Records;
            var item = list.FirstOrDefault(x => x.RecordNo == _selectedItem.RecordNo);
            var index = list.IndexOf(item);

            if (index == list.Count - 2)
            {
                nextImage.Source = "nextDisable.png";
                nextButton.IsEnabled = false;
            }
            prevImage.Source = "prev.png";
            prevButton.IsEnabled = true;

            _selectedItem = list[index + 1];

            ShowPopupData(_selectedItem);
        }

        /// <summary>
        ///     Shows the popup data.
        /// </summary>
        /// <param name="record">Record.</param>
        public void ShowPopupData(ItemTemperature record)
        {
            var list = _viewModel.Records;
            var index = list.IndexOf(_selectedItem);

            if (index == 0)
            {
                prevImage.Source = "prevDisable.png";
                prevButton.IsEnabled = false;
            }
            else
            {
                prevImage.Source = "prev.png";
                prevButton.IsEnabled = true;
            }

            if (index == list.Count - 1)
            {
                nextImage.Source = "nextDisable.png";
                nextButton.IsEnabled = false;
            }
            else
            {
                nextImage.Source = "next.png";
                nextButton.IsEnabled = true;
            }


            _viewModel.IsReviewItemVisible = true;
            var tempUnit = HaccpAppSettings.SharedInstance.DeviceSettings.TempScale == 0
                ? TemperatureUnit.Fahrenheit
                : TemperatureUnit.Celcius;
            var temperature = HACCPUtil.ConvertToDouble(record.Temperature);
            var min = HACCPUtil.ConvertToDouble(record.Min);
            var max = HACCPUtil.ConvertToDouble(record.Max);
            Debug.WriteLine("{0} temp : {1}", record.Temperature, temperature);
            var unit = HACCPUtil.GetResourceString("FahrenheitUnit");
            if (tempUnit == TemperatureUnit.Celcius)
            {
                temperature = Math.Round(HACCPUtil.ConvertFahrenheitToCelsius(temperature), 1);
                min = Math.Round(HACCPUtil.ConvertFahrenheitToCelsius(min));
                max = Math.Round(HACCPUtil.ConvertFahrenheitToCelsius(max));
                unit = HACCPUtil.GetResourceString("CelsciustUnit");
            }

            if (record.IsNA == 1)
            {
                RecordedTemp.Text = string.Format("{0}: {1}", HACCPUtil.GetResourceString("RecordedTemperature"),
                    HACCPUtil.GetResourceString("NA"));
                CorrectiveAction.IsVisible = false;
            }
            else
            {
                RecordedTemp.Text = string.Format("{0}: {1}{2}", HACCPUtil.GetResourceString("RecordedTemperature"),
                    temperature.ToString("0.0"), unit);
                CorrectiveAction.IsVisible = true;
                CorrectiveAction.Text = string.Format("{0}: {1}", HACCPUtil.GetResourceString("CorrectiveAction"),
                    record.CorrAction);
            }
            if (!string.IsNullOrEmpty(record.Note))
            {
                Notes.Text = string.Format("{0}: {1}", HACCPUtil.GetResourceString("NotesLabel"), record.Note);
                Notes.IsVisible = true;
            }
            else
            {
                Notes.IsVisible = false;
            }
            questionLabel.Text = record.ItemName;
            TempRange.Text = string.Format("{0}: {1}{2}, {3}: {4}{5}", HACCPUtil.GetResourceString("Min").ToUpper(), min,
                unit, HACCPUtil.GetResourceString("Max").ToUpper(), max, unit);
            UserName.Text = string.Format("{0}: {1}", HACCPUtil.GetResourceString("Recordedby"), record.UserName);

            var date = new DateTime(Convert.ToInt32(record.Year), Convert.ToInt32(record.Month),
                Convert.ToInt32(record.Day), Convert.ToInt32(record.Hour), Convert.ToInt32(record.Minute),
                Convert.ToInt32(record.Sec));
            TimeStamp.Text = string.Format("{0}: {1}", HACCPUtil.GetResourceString("Time"),
                HACCPUtil.GetFormattedDate(date, false));
        }

        /// <summary>
        /// OnAppearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            App.CurrentPageType = typeof(TemperatureReview);


            MessagingCenter.Subscribe<NextPrevButtonClickMessage>(this, HaccpConstant.NextPrevMessage, sender =>
            {
                try
                {
                    var val = sender.IsNext;
                    if (val)
                        NextButtonClick(null, null);
                    else
                        PrevButtonClick(null, null);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error NextPrevButtonClickMessage {0}", ex.Message);
                }
            });
        }

        /// <summary>
        /// OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            MessagingCenter.Unsubscribe<NextPrevButtonClickMessage>(this, HaccpConstant.NextPrevMessage);
        }

        #endregion
    }
}