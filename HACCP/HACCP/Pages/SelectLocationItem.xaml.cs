using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HACCP.Core;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace HACCP
{
	public partial class SelectLocationItem : BaseView
	{
		#region Member Variables

		private readonly LocationItemVIewModel _viewModel;

		private LocationMenuItem _selectedItem;
        private bool IsListViewSelected { get; set; }

		#endregion

		/// <summary>
		/// Select Location Item
		/// </summary>
		/// <param name="location"></param>
		public SelectLocationItem (object location)
		{
			InitializeComponent ();
			NavigationPage.SetBackButtonTitle (this, string.Empty);
			BindingContext = _viewModel = new LocationItemVIewModel (this, location as MenuLocation);


			ToolbarItems.Add (new ToolbarItem {
				Icon = "menu.png",
				Order = ToolbarItemOrder.Primary,
				Command = _viewModel.ReviewListCommand
			});

			//Adding ItemsFilter button in toolbar
			ToolbarItems.Add (new ToolbarItem {
				Icon = "fault.png",
				Order = ToolbarItemOrder.Primary,
				Command = new Command (FaultIconClick)
			});

			searchLocationItem.TextChanged += (object sender, TextChangedEventArgs e) => {
				string searchText = searchLocationItem.Text.ToLower ().TrimStart ();

				if (string.IsNullOrWhiteSpace (searchText)) {
					
					_viewModel.Items =new ObservableCollection<LocationMenuItem>(_viewModel.locationitems);
					//ItemsListview.ItemsSource = _viewModel.Items;
					_viewModel.HasItems = true;
				} else {
					var locationItems = _viewModel.locationitems.Where (i => i.Name.ToLower ().Contains (searchText));
					var locationMenuItems = locationItems as IList<LocationMenuItem> ?? locationItems.ToList ();

					_viewModel.Items = new ObservableCollection<LocationMenuItem> (locationMenuItems);
					// ItemsListview.ItemsSource = locationMenuItems;
					_viewModel.HasItems = locationMenuItems.Any ();
				}

				if (!HaccpAppSettings.SharedInstance.IsWindows) {
					searchLocationItem.Text = searchText;
				}
			};
			//Attaching items selected event
			ItemsListview.ItemSelected += ListItemSelected;
		}

    
		#region Event Handlers

		/// <summary>
		///     List item selected event handler.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void ListItemSelected (object sender, SelectedItemChangedEventArgs e)
		{
			

            if (ItemsListview.SelectedItem == null)
                return;
            if (!IsListViewSelected || !HaccpAppSettings.SharedInstance.IsWindows)
            {
                IsListViewSelected = true;
                _viewModel.SelectedLocationItem = ItemsListview.SelectedItem as LocationMenuItem;
                ItemsListview.SelectedItem = null;
               
            }
            else
            {
                IsListViewSelected = false;
                ItemsListview.SelectedItem = null;
            }
		}

		/// <summary>
		/// Prev Button Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public void PrevButtonClick (object sender, EventArgs args)
		{
			var list = _viewModel.Items;
			var item = list.FirstOrDefault (x => x.ItemId == _selectedItem.ItemId);
			var index = list.IndexOf (item);


			if (index == 1) {
				prevImage.Source = "prevDisable.png";
				prevButton.IsEnabled = false;
			}

			nextImage.Source = "next.png";
			nextButton.IsEnabled = true;

			_selectedItem = list [index - 1];
			var record = _viewModel.HandleItemClick (_selectedItem);

			ShowPopupData (record);
		}

		/// <summary>
		/// Next Button Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public void NextButtonClick (object sender, EventArgs args)
		{
			if (_selectedItem != null) {
				var list = _viewModel.Items;
				var item = list.FirstOrDefault (x => x.ItemId == _selectedItem.ItemId);
				var index = list.IndexOf (item);

				if (index == list.Count - 2) {
					nextImage.Source = "nextDisable.png";
					nextButton.IsEnabled = false;
				}
				prevImage.Source = "prev.png";
				prevButton.IsEnabled = true;

				_selectedItem = list [index + 1];
				var record = _viewModel.HandleItemClick (_selectedItem);
				ShowPopupData (record);
			}
		}

		/// <summary>
		///     Faults the icon click.
		/// </summary>
		public void FaultIconClick ()
		{
			if (_viewModel.IsReviewItemVisible)
				return;


			searchLocationItem.Text = string.Empty;
			searchLocationItem.Unfocus ();
			if (ToolbarItems.Count == 1) {
				ToolbarItems.Insert (0, new ToolbarItem {
					Icon = "menu.png",
					Order = ToolbarItemOrder.Primary,
					Command = _viewModel.ReviewListCommand
				});
			} else {
				ToolbarItems.RemoveAt (0);
			}
			_viewModel.ItemsFilterCommand.Execute (null);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Show Popup Data
		/// </summary>
		/// <param name="record"></param>
		public void ShowPopupData (ItemTemperature record)
		{
			var list = _viewModel.Items;
			var item = list.FirstOrDefault (x => x.ItemId == _selectedItem.ItemId);
			var index = list.IndexOf (item);

			if (index == 0) {
				prevImage.Source = "prevDisable.png";
				prevButton.IsEnabled = false;
			} else {
				prevImage.Source = "prev.png";
				prevButton.IsEnabled = true;
			}

			if (index == list.Count - 1) {
				nextImage.Source = "nextDisable.png";
				nextButton.IsEnabled = false;
			} else {
				nextImage.Source = "next.png";
				nextButton.IsEnabled = true;
			}

			var tempUnit = HaccpAppSettings.SharedInstance.DeviceSettings.TempScale == 0
                ? TemperatureUnit.Fahrenheit
                : TemperatureUnit.Celcius;
			var temperature = HACCPUtil.ConvertToDouble (record.Temperature);
			var min = HACCPUtil.ConvertToDouble (record.Min);
			var max = HACCPUtil.ConvertToDouble (record.Max);

			var unit = HACCPUtil.GetResourceString ("FahrenheitUnit");
			if (tempUnit == TemperatureUnit.Celcius) {
				temperature = Math.Round (HACCPUtil.ConvertFahrenheitToCelsius (temperature), 1);
				min = Math.Round (HACCPUtil.ConvertFahrenheitToCelsius (min));
				max = Math.Round (HACCPUtil.ConvertFahrenheitToCelsius (max));
				unit = HACCPUtil.GetResourceString ("CelsciustUnit");
			}

			_viewModel.IsReviewItemVisible = true;
			questionLabel.Text = record.ItemName;

			RecordedTemp.Text = string.Format ("{0}: {1}{2}", HACCPUtil.GetResourceString ("RecordedTemperature"),
				temperature.ToString ("0.0"), unit);
			CorrectiveAction.IsVisible = true;
			CorrectiveAction.Text = string.Format ("{0}: {1}", HACCPUtil.GetResourceString ("CorrectiveAction"),
				record.CorrAction);
			if (!string.IsNullOrEmpty (record.Note)) {
				Notes.Text = string.Format ("{0}: {1}", HACCPUtil.GetResourceString ("NotesLabel"), record.Note);
				Notes.IsVisible = true;
			} else {
				Notes.IsVisible = false;
			}
			TempRange.Text = string.Format ("{0}: {1}{2}, {3}: {4}{5}", HACCPUtil.GetResourceString ("Min").ToUpper (), min,
				unit, HACCPUtil.GetResourceString ("Max").ToUpper (), max, unit);
			UserName.Text = string.Format ("{0}: {1}", HACCPUtil.GetResourceString ("Recordedby"), record.UserName);

			var date = new DateTime (Convert.ToInt32 (record.Year), Convert.ToInt32 (record.Month),
				                    Convert.ToInt32 (record.Day), Convert.ToInt32 (record.Hour), Convert.ToInt32 (record.Minute),
				                    Convert.ToInt32 (record.Sec));
			var dateString = date.ToString ();


			TimeStamp.Text = string.Format ("{0}: {1}", HACCPUtil.GetResourceString ("Time"),
				HACCPUtil.GetFormattedDate (dateString));
		}

		/// <summary>
		/// OnAppearing
		/// </summary>
		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			App.CurrentPageType = typeof(SelectLocationItem);

			MessagingCenter.Subscribe<ShowReviewMessage> (this, HaccpConstant.ReviewMessage, sender => {
				var item = sender.Item;
				_selectedItem = sender.MenuItem;
				ShowPopupData (item);
			});

			MessagingCenter.Subscribe<NextPrevButtonClickMessage> (this, HaccpConstant.NextPrevMessage, sender => {
				try {
					var val = sender.IsNext;
					if (val)
						NextButtonClick (null, null);
					else
						PrevButtonClick (null, null);
				} catch (Exception ex) {
					Debug.WriteLine ("Error NextPrevButtonClickMessage {0}", ex.Message);
				}
			});
		}

		/// <summary>
		/// OnDisappearing
		/// </summary>
		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();
			searchLocationItem.Text = string.Empty;
			if (HaccpAppSettings.SharedInstance.IsWindows)
				ItemsListview.SelectedItem = null;
			MessagingCenter.Unsubscribe<ShowReviewMessage> (this, HaccpConstant.ReviewMessage);
			MessagingCenter.Unsubscribe<NextPrevButtonClickMessage> (this, HaccpConstant.NextPrevMessage);
		}

		#endregion
	}
}