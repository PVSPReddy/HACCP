using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HACCP.Core
{
	public class HomeViewModel : BaseViewModel
	{
		private readonly IHACCPService _haccpService;
		private Command _blue2SettingsCommand;

		private readonly IDataStore _dataStore;
		private bool _isAlertActive;
		private bool _onceLoaded;
		private Command _selectCheckListCommand;
		private Command _selectClearCheckMarkCommand;
		private Command _selectLocationCommand;
		private Command _selectUserCommand;
		private Command _thermometerModeCommand;
		private Command _uploadRecordCommand;
		private Command _viewStatusCommand;
		private Command _wirelessTaskCommand;

		/// <summary>
		///     Initializes a new instance of the <see cref="HACCP.Core.HomeViewModel" /> class.
		/// </summary>
		/// <param name="page">Page.</param>
		public HomeViewModel (IPage page)
			: base (page)
		{
			_onceLoaded = false;
			_dataStore = new SQLiteDataStore ();

			_haccpService = new HACCPService (_dataStore);
		}

		#region Properties

		/// <summary>
		///     Gets the name of the user.
		/// </summary>
		/// <value>The name of the user.</value>
		public string UserName {
			get { return IsLoggedIn ? HaccpAppSettings.SharedInstance.UserName : string.Empty; }
		}

		/// <summary>
		///     Gets the select location command.
		/// </summary>
		/// <value>The select location command.</value>
		public Command SelectLocationCommand {
			get {
				return _selectLocationCommand ??
				(_selectLocationCommand =
                           new Command (ExecuteSelectLocationCommand, () => !IsBusy));
			}
		}

		/// <summary>
		///     Gets the select check list command.
		/// </summary>
		/// <value>The select check list command.</value>
		public Command SelectCheckListCommand {
			get {
				return _selectCheckListCommand ??
				(_selectCheckListCommand =
                           new Command (ExecuteSelectCheckListCommand, () => !IsBusy));
			}
		}


		/// <summary>
		///     Gets the selec clear check mark command.
		/// </summary>
		/// <value>The selec clear check mark command.</value>
		public Command SelecClearCheckMarkCommand {
			get {
				return _selectClearCheckMarkCommand ??
				(_selectClearCheckMarkCommand =
                           new Command (ExecuteClearCheckmarksCommand, () => !IsBusy));
			}
		}

		/// <summary>
		///     Gets the select user command.
		/// </summary>
		/// <value>The select user command.</value>
		public Command SelecUserCommand {
			get {
				return _selectUserCommand ??
				(_selectUserCommand = new Command (ExecuteSelecUserCommand, () => !IsBusy));
			}
		}

		/// <summary>
		///     Gets the wireless task command.
		/// </summary>
		/// <value>The wireless task command.</value>
		public Command WirelessTaskCommand {
			get {
				return _wirelessTaskCommand ??
				(_wirelessTaskCommand = new Command (ExecuteWirelessTaskCommand, () => !IsBusy));
			}
		}

		/// <summary>
		///     Gets the blue2 settings command.
		/// </summary>
		/// <value>The blue2 settings command.</value>
		public Command Blue2SettingsCommand {
			get {
				return _blue2SettingsCommand ??
				(_blue2SettingsCommand =
                           new Command (ExecuteBlue2SettingsCommand, () => !IsBusy));
			}
		}


		/// <summary>
		///     Gets the view status command.
		/// </summary>
		/// <value>The view status command.</value>
		public Command ViewStatusCommand {
			get {
				return _viewStatusCommand
				??
				(_viewStatusCommand = new Command (ExecuteViewStatusCommand, () => !IsBusy));
			}
		}

		/// <summary>
		///     Gets the thermometer mode command.
		/// </summary>
		/// <value>The thermometer mode command.</value>
		public Command ThermometerModeCommand {
			get {
				return _thermometerModeCommand ??
				(_thermometerModeCommand =
                           new Command (ExecuteThermometerModeCommand, () => !IsBusy));
			}
		}

		/// Gets the upload record command.
		/// 
		/// <value>The upload record command.</value>
		public Command UploadRecordCommand {
			get {
				return _uploadRecordCommand ??
				(_uploadRecordCommand =
                           new Command ( async () =>  await ExecuteUploadRecordCommand(true), () => !IsBusy));
			}
		}

		#endregion

		#region Methods

		/// <summary>
		///     Called when the view appears.
		/// </summary>
		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();
		
			OnPropertyChanged ("UserName");

			if (IsServerAddressExist == false)
				Page.NavigateTo (PageEnum.WirelessTask, false);

			//block upload checking message after language switch
			if (HaccpAppSettings.SharedInstance.IsLanguageChanged) {   
				HaccpAppSettings.SharedInstance.IsLanguageChanged = false;
				return;
			}

			if (!_onceLoaded) {
				if (!HaccpAppSettings.SharedInstance.IsWindows) {
					//if (Device.OS == TargetPlatform.iOS)
						HaccpAppSettings.SharedInstance.CheckPendingRecords = true;
					//if (Device.OS == TargetPlatform.Android)
						//Device.BeginInvokeOnMainThread (async () => await CheckPendingRecords (false));
				}
				_onceLoaded = true;
			}
		}

		/// <summary>
		///     Raises the login changed event.
		/// </summary>
		protected override void OnLoginChanged ()
		{
			base.OnLoginChanged ();

			SelectLocationCommand.ChangeCanExecute ();
			SelectCheckListCommand.ChangeCanExecute ();
			SelecClearCheckMarkCommand.ChangeCanExecute ();

			OnPropertyChanged ("UserName");
		}

		/// <summary>
		///     Raises the check pending record changed event.
		/// </summary>
		protected override void OnCheckPendingChanged ()
		{
			base.OnCheckPendingChanged ();

			if (Device.OS == TargetPlatform.Android) {
				if (HaccpAppSettings.SharedInstance.AppResuming) {
					HaccpAppSettings.SharedInstance.AppResuming = false;
					Device.BeginInvokeOnMainThread (async () => {
						await CheckPendingRecords (false);
					});
				} else if (!_isAlertActive) {
					Device.BeginInvokeOnMainThread (async () => {
						await CheckPendingRecords (false);
					});
					_isAlertActive = false;
				}
			} else {
				Device.BeginInvokeOnMainThread (async () => {
					await CheckPendingRecords (false);
					_isAlertActive = false;
				});
			}
		}


		/// <summary>
		///     Checks the pending records.
		/// </summary>
		/// <returns>The pending records.</returns>
		private async Task CheckPendingRecords (bool showConfirmation)
		{
			try {
				if (HaccpAppSettings.SharedInstance.CheckPendingRecords &&
				    !HaccpAppSettings.SharedInstance.IsUploadingProgress) {
					var checklists = _dataStore.GetCheckListResponses ();
					var temperatures = _dataStore.GetItemTemperatures ();

					if ((checklists != null && checklists.Count != 0) ||
					    (temperatures != null && temperatures.Count != 0)) {
						Page.DismissPopup ();
						IsBusy = false;

						if (!_isAlertActive) {
							_isAlertActive = true;
							var result =
								await Page.ShowConfirmAlert (string.Empty,
									HACCPUtil.GetResourceString ("PendingCheckmarksMessage"));
							await Task.Delay (500);
							_isAlertActive = false;

							if (result && HaccpAppSettings.SharedInstance.CheckPendingRecords) {
								await ExecuteUploadRecordCommand (showConfirmation);
							}
						}
					}
				}
				HaccpAppSettings.SharedInstance.CheckPendingRecords = false;
			} catch (Exception ex) {
				Debug.WriteLine ("Error on CheckPendingRecords :{0}", ex.Message);
			}
		}

		/// <summary>
		///     Executes the select user command.
		/// </summary>
		/// <returns>The select user command.</returns>
		private void ExecuteSelecUserCommand ()
		{
			if (IsBusy)
				return;

			IsBusy = true;
			Page.ShowProgressIndicator ();
			SelecUserCommand.ChangeCanExecute ();

			try {
				Task.Run (async () => {
					if (HaccpAppSettings.SharedInstance.IsWindows)
						await Task.Delay (500);

					var isUserExist = _dataStore.CheckUsersExist ();

					if (isUserExist)
						Device.BeginInvokeOnMainThread (async () => {
							await Page.NavigateTo (PageEnum.SelectUser, true);

							IsBusy = false;
							Page.DismissPopup ();
							SelecUserCommand.ChangeCanExecute ();
						});
					else {
						Device.BeginInvokeOnMainThread (() => {
							Page.DisplayAlertMessage (HACCPUtil.GetResourceString ("NoUsersFound"),
								HACCPUtil.GetResourceString ("NousersfoundPleasetapUpdateUserListintheWirelessTasksmenu"));
							IsBusy = false;
							Page.DismissPopup ();
							SelecUserCommand.ChangeCanExecute ();
						});
					}
				});
			} catch (Exception ex) {
				Debug.WriteLine ("ExecuteSelecUserCommand Error {0}", ex.Message);
			}
		}

		/// <summary>
		///     Executes the wireless task command.
		/// </summary>
		/// <returns>The wireless task command.</returns>
		private void ExecuteWirelessTaskCommand ()
		{
			if (IsBusy)
				return;
			IsBusy = true;
			Page.ShowProgressIndicator ();
			WirelessTaskCommand.ChangeCanExecute ();

			Task.Run (() => {
				Device.BeginInvokeOnMainThread (async () => {
					await Page.NavigateTo (PageEnum.WirelessTask, true);
					IsBusy = false;
					Page.DismissPopup ();
					WirelessTaskCommand.ChangeCanExecute ();
				});
			});
		}


		/// <summary>
		///     Executes the select check list command.
		/// </summary>
		/// <returns>The select check list command.</returns>
		private void ExecuteSelectCheckListCommand ()
		{
			if (IsBusy)
				return;

			IsBusy = true;
			Page.ShowProgressIndicator ();
			_selectCheckListCommand.ChangeCanExecute ();

			Task.Run (() => {
				if (HaccpAppSettings.SharedInstance.SiteSettings.CheckListId > 0 && _dataStore.CheckCategoryExist ()) {
					Device.BeginInvokeOnMainThread (async () => {
						await Page.NavigateTo (PageEnum.PerformCheckList, true);
						if (HaccpAppSettings.SharedInstance.IsWindows)
							await Task.Delay (500);
						IsBusy = false;
						Page.DismissPopup ();
						_selectCheckListCommand.ChangeCanExecute ();
					});
				} else {
					Device.BeginInvokeOnMainThread (() => {
						Page.DisplayAlertMessage (HACCPUtil.GetResourceString ("NoCategoriesFound"),
							HACCPUtil.GetResourceString (
								"NocategoriesfoundPleasetapSelectChangeChecklistintheWirelessTasksMenu"));
						IsBusy = false;
						Page.DismissPopup ();
						_selectCheckListCommand.ChangeCanExecute ();
					});
				}
			});
		}


		/// <summary>
		///     Executes the select check list command.
		/// </summary>
		/// <returns>The select check list command.</returns>
		private void ExecuteClearCheckmarksCommand ()
		{
			if (IsBusy)
				return;

			IsBusy = true;
			Page.ShowProgressIndicator ();
			_selectClearCheckMarkCommand.ChangeCanExecute ();

			Task.Run (() => {
				Device.BeginInvokeOnMainThread (async () => {
					await Page.NavigateTo (PageEnum.ClearCheckmarks, true);
					IsBusy = false;
					Page.DismissPopup ();
					_selectClearCheckMarkCommand.ChangeCanExecute ();
				});
			});
		}

		/// <summary>
		///     Executes the select user command.
		/// </summary>
		/// <returns>The select user command.</returns>
		private void ExecuteSelectLocationCommand ()
		{
			if (IsBusy || !IsLoggedIn)
				return;

			IsBusy = true;
			Page.ShowProgressIndicator ();
			SelectLocationCommand.ChangeCanExecute ();

			Task.Run (() => {
				if (HaccpAppSettings.SharedInstance.SiteSettings.MenuId > 0 && _dataStore.CheckLocationsExist ()) {
					Device.BeginInvokeOnMainThread (async () => {
						await Page.NavigateTo (PageEnum.SelectLocations, true);
						if (HaccpAppSettings.SharedInstance.IsWindows)
							await Task.Delay (500);

						IsBusy = false;
						Page.DismissPopup ();
						SelectLocationCommand.ChangeCanExecute ();
					});
				} else {
					Device.BeginInvokeOnMainThread (() => {
						Page.DisplayAlertMessage (HACCPUtil.GetResourceString ("NoLocationsFound"),
							HACCPUtil.GetResourceString (
								"NolocationsfoundPleasetapSelectChangeMenuintheWirelessTasksMenu"));
						IsBusy = false;
						Page.DismissPopup ();
						SelectLocationCommand.ChangeCanExecute ();
					});
				}
			});
		}

		/// <summary>
		///     Executes the blue2 settings command.
		/// </summary>
		/// <returns>The blue2 settings command.</returns>
		private void ExecuteBlue2SettingsCommand ()
		{
			if (IsBusy)
				return;


			IsBusy = true;
			Page.ShowProgressIndicator ();
			Blue2SettingsCommand.ChangeCanExecute ();

			Task.Run (() => {
				Device.BeginInvokeOnMainThread (async () => {
					await Page.NavigateTo (PageEnum.Blue2Settings, true);
					IsBusy = false;
					Page.DismissPopup ();
					Blue2SettingsCommand.ChangeCanExecute ();
				});
			});
		}


		/// <summary>
		///     Executes the view status command.
		/// </summary>
		/// <returns>The view status command.</returns>
		private void ExecuteViewStatusCommand ()
		{
			if (IsBusy)
				return;

			IsBusy = true;
			Page.ShowProgressIndicator ();
			ViewStatusCommand.ChangeCanExecute ();

			Task.Run (() => {
				Device.BeginInvokeOnMainThread (async () => {
					await Page.NavigateTo (PageEnum.ViewStatus, true);
					IsBusy = false;
					Page.DismissPopup ();
					ViewStatusCommand.ChangeCanExecute ();
				});
			});
		}

		/// <summary>
		///     Executes the thermometer mode command.
		/// </summary>
		/// <returns>The thermometer mode command.</returns>
		private void ExecuteThermometerModeCommand ()
		{
			if (IsBusy)
				return;

			IsBusy = true;
			Page.ShowProgressIndicator ();
			_thermometerModeCommand.ChangeCanExecute ();

			Task.Run (() => {
				Device.BeginInvokeOnMainThread (async () => {
					await Page.NavigateTo (PageEnum.ThermometerMode, true);
					IsBusy = false;
					Page.DismissPopup ();
					_thermometerModeCommand.ChangeCanExecute ();
				});
			});
		}

		/// <summary>
		///     Executes the upload record command.
		/// </summary>
		private async Task ExecuteUploadRecordCommand (bool showConfirmation)
		{
			if (IsBusy)
				return;

			IsBusy = true;

			try {
				if (_haccpService.IsConnected () == false) {
					Page.DisplayAlertMessage (HACCPUtil.GetResourceString ("EnableNetworkConnection"),
						HACCPUtil.GetResourceString (
							"YourequireanactiveInternetconnectiontoperformsomefunctionsWerecommendthatyouenableWiFiforthispurposeDatachargesmayapplyifWiFiisnotenabled"));
					return;
				}

				Page.ShowUploadProgress ();
				UploadRecordCommand.ChangeCanExecute ();

				if (HaccpAppSettings.SharedInstance.SiteSettings.SiteId > 0) {
					var proceedUpload = false;

					//Getting check lists and temperatures
					var checklists = _dataStore.GetCheckListResponses ();
					var temperatures = _dataStore.GetItemTemperatures ();


					if ((checklists == null || checklists.Count == 0) &&
					    (temperatures == null || temperatures.Count == 0)) {
						IsBusy = false;
						Page.DismissUploadProgress ();
						await Page.ShowAlert (string.Empty, HACCPUtil.GetResourceString ("Norecordsfoundtoupload"));
					} else if (showConfirmation &&
					           await Page.ShowConfirmAlert (HACCPUtil.GetResourceString ("UploadRecords"),
						           HACCPUtil.GetResourceString (
							           "Alltherecordswillbeuploadedandthecheckmarkswillbecleared"))) {
//confirmation alert
						proceedUpload = true;
					} else if (!showConfirmation) {
						proceedUpload = true;
					}


					if (proceedUpload) {
						if (checklists != null && (checklists.Count > 0 || temperatures.Count > 0)) {
							//Calling upload records method
							var response = await _haccpService.UploadRecords (checklists, temperatures);

							if (response.IsSuccess) {
								IsBusy = false;
								Page.DismissUploadProgress ();
								await Page.ShowAlert (string.Empty,
									HACCPUtil.GetResourceString (
										"UploadedalltherecordstoHACCPEnterpriseapplicationsuccessfully"));

								MessagingCenter.Send (new UploadRecordRefreshMessage (), HaccpConstant.UploadRecordRefresh);
							} else if (!string.IsNullOrEmpty (response.Message)) {
								IsBusy = false;
								Page.DismissUploadProgress ();
								await Page.ShowAlert (string.Empty, response.Message);
							} else {
								IsBusy = false;
								Page.DismissUploadProgress ();
								await Page.ShowAlert (string.Empty,
									HACCPUtil.GetResourceString (
										"AnerroroccurredwhileuploadingtherecordsPleasetryagainlater"));
							}
						} else {
							IsBusy = false;
							Page.DismissUploadProgress ();
							await Page.ShowAlert (string.Empty, HACCPUtil.GetResourceString ("Norecordsfoundtoupload"));
						}
					}
				} else {
					IsBusy = false;
					Page.DismissUploadProgress ();
					await Page.ShowAlert (HACCPUtil.GetResourceString ("NoSiteInformationFound"),
						HACCPUtil.GetResourceString (
							"NositeinformationsfoundPleasetapUpdateSiteandSettingsintheWirelessTasksmenu"));
				}
			} catch (Exception ex) {
				Debug.WriteLine ("Ooops! Something went wrong while select menu. Exception: {0}", ex);
			} finally {
				IsBusy = false;
				Page.DismissUploadProgress ();
				UploadRecordCommand.ChangeCanExecute ();
			}
		}

		#endregion
	}
}