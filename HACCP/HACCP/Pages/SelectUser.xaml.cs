using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HACCP.Core;
using HACCP.Core.Models;
using Xamarin.Forms;

namespace HACCP
{
	public partial class SelectUser : BaseView
	{
		private readonly UsersViewModel _viewModel;

		/// <summary>
		/// Select User
		/// </summary>
		public SelectUser ()
		{
			InitializeComponent ();
			
			NavigationPage.SetBackButtonTitle (this, string.Empty);

			BindingContext = _viewModel = new UsersViewModel (this);

			if (HaccpAppSettings.SharedInstance.IsWindows)
				AddToolBarButton ("home.png", _viewModel.HomeCommand);


			UserListview.ItemSelected += (sender, e) => {
				try {
					//	if (viewModel.IsBusy)
					//	return;
					//


					if (UserListview.SelectedItem == null)
						return;


					//viewModel.IsBusy = true;

					if (_viewModel.isViewAppeared) {
						var selectedUserId = ((User)UserListview.SelectedItem).Id;
						_viewModel.selectedUser = (User)UserListview.SelectedItem;

						if (selectedUserId < 1) {
							//if (!HaccpAppSettings.SharedInstance.IsWindows)
								UserListview.SelectedItem = null;
							//	viewModel.IsBusy = false;
							return;
						}

						if (_viewModel.IsLoggedIn && HaccpAppSettings.SharedInstance.CurrentUserId == selectedUserId) {
							_viewModel.LogInCommand.Execute (null);
							//if (!HaccpAppSettings.SharedInstance.IsWindows)
								UserListview.SelectedItem = null;
							//viewModel.IsBusy = false;
							return;
						}
						_viewModel.SelectedUserId = selectedUserId;


						if (HaccpAppSettings.SharedInstance.DeviceSettings.RequirePin != 1) {
							HaccpAppSettings.SharedInstance.UserName = _viewModel.selectedUser.Name;
							HaccpAppSettings.SharedInstance.CurrentUserId = selectedUserId;
							HaccpAppSettings.SharedInstance.Permission = _viewModel.selectedUser.Permision;
							Settings.LastLoginUserId = selectedUserId;
							EndEditing ();
							PopPage ();
						} else {
							_viewModel.OnPropertyChanged ("DisplayPasswordPopup");
							passwordTextBox.Focus ();
						}

						//if (!HaccpAppSettings.SharedInstance.IsWindows)
							UserListview.SelectedItem = null;

						//viewModel.IsBusy = false;
					} else
						return;
				} catch (Exception ex) {
					Debug.WriteLine ("UserListview.ItemSelected Error : {0}", ex.Message);

					//viewModel.IsBusy = false;
				}
			};


			try {
				if (HaccpAppSettings.SharedInstance.CurrentUserId > 0) {
					if (HaccpAppSettings.SharedInstance.IsWindows) {
						Device.StartTimer (new TimeSpan (0, 0, 1), () => {
							UserListview.ScrollTo (_viewModel.LoggedUser, ScrollToPosition.Center, false);
							return false;
						});
					} else
						UserListview.ScrollTo (_viewModel.LoggedUser, ScrollToPosition.Center, false);
				} else {
					if (HaccpAppSettings.SharedInstance.IsWindows) {
						Device.StartTimer (new TimeSpan (0, 0, 1), () => {
							var lastLoggedUser = _viewModel.Users.SingleOrDefault (u => u.Id == Settings.LastLoginUserId);
							if (lastLoggedUser != null)
								UserListview.ScrollTo (lastLoggedUser, ScrollToPosition.Center, false);
							return false;
						});
					} else {
						var lastLoggedUser = _viewModel.Users.SingleOrDefault (u => u.Id == Settings.LastLoginUserId);
						if (lastLoggedUser != null)
							UserListview.ScrollTo (lastLoggedUser, ScrollToPosition.Center, false);
					}
				}
			} catch (Exception ex) {
				Debug.WriteLine (ex.Message);
			}
		}

		/// <summary>
		/// EndEditing
		/// </summary>
		public override void EndEditing ()
		{
			passwordTextBox.Unfocus ();
		}

		/// <summary>
		/// OnAppearing
		/// </summary>
		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			App.CurrentPageType = typeof(SelectUser);
			MessagingCenter.Subscribe<UserPasswordFocusMessage> (this, HaccpConstant.UserPasswordFocusMessage,
				sender => {
					passwordTextBox.Focus ();
				});
			Device.BeginInvokeOnMainThread (async () => {		
				if (Device.OS != TargetPlatform.iOS)							
					await Task.Delay (500);
				UserListview.IsEnabled = true;
				_viewModel.isViewAppeared = true;
			});
		}

		/// <summary>
		/// OnDisappearing
		/// </summary>
		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();

			MessagingCenter.Unsubscribe<UserPasswordFocusMessage> (this, HaccpConstant.UserPasswordFocusMessage);
		}
	}
}