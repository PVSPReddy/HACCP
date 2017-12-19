using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HACCP.Core
{
    public class BaseViewModel : INotifyPropertyChanged
    {
		#region Member Variables

		public const string IsProgressVisiblePropertyName = "IsProgressVisible";
        public const string IsBusyPropertyName = "IsBusy";
        public const string CanLoadMorePropertyName = "CanLoadMore";
        public const string IsServerAddressExistPropertyName = "IsServerAddressExist";
        public const string IsLoggedInPropertyName = "IsLoggedIn";
        public const string CheckPendingPropertyName = "CheckPendingRecords";
        public const string IsMenuCheckListActivePropertyName = "IsMenuCheckListActive";
        public const string DeviceIdPropertyName = "DeviceID";
        public const string SubtitlePropertyName = "Subtitle";
        public const string IconPropertyName = "Icon";
        public const string TitlePropertyName = "Title";
        public const string LogInCommandPropertyName = "LogInCommand";
        protected IPage Page;
        private Command _backCommand;
        private bool _canLoadMore = true;
        private bool _isBusy;
        private Command _logInCommand;
        private string _message = string.Empty;
        private Command _notImplementedCommand;
        private string _subTitle = string.Empty;
        private string _title = string.Empty;
        public event PropertyChangedEventHandler PropertyChanged;
        private bool _isWindows = HaccpAppSettings.SharedInstance.IsWindows;
        private string _icon;
		private bool _isProgressVisible;
        #endregion

        /// <summary>
        ///     Initializes a new instance of the <see cref="HACCP.Core.BaseViewModel" /> class.
        /// </summary>
        /// <param name="page">Page.</param>
        public BaseViewModel(IPage page)
        {
            //	appSettings = HACCPAppSettings.SharedInstance;
            Page = page;
            MessagingCenter.Subscribe<HaccpAppSettings>(this, HaccpConstant.MsLoginChanged,
                sender => { OnLoginChanged(); });

            MessagingCenter.Subscribe<HaccpAppSettings>(this, HaccpConstant.MsCheckpending,
                sender => { OnCheckPendingChanged(); });
        }

        #region Properties

        /// <summary>
        ///     Gets or sets the "Title" property
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value, TitlePropertyName); }
        }

        /// <summary>
        ///     Gets or sets the "Subtitle" property
        /// </summary>
        public string Subtitle
        {
            get { return _subTitle; }
            set { SetProperty(ref _subTitle, value, SubtitlePropertyName); }
        }

        /// <summary>
        ///     Gets or sets the "Icon" of the viewmodel
        /// </summary>
        public string Icon
        {
            get { return _icon; }
            set { SetProperty(ref _icon, value, IconPropertyName); }
        }

        /// <summary>
        ///     Gets or sets if the view is busy.
        /// </summary>
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value, IsBusyPropertyName); }
        }

        /// <summary>
        ///     Gets or sets if we can load more.
        /// </summary>
        public bool CanLoadMore
        {
            get { return _canLoadMore; }
            set { SetProperty(ref _canLoadMore, value, CanLoadMorePropertyName); }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is server address exist.
        /// </summary>
        /// <value><c>true</c> if this instance is server address exist; otherwise, <c>false</c>.</value>
        public bool IsServerAddressExist
        {
            get
            {
                if (string.IsNullOrEmpty(HaccpAppSettings.SharedInstance.SiteSettings.ServerAddress))
                    return false;
                return true;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is logged in.
        /// </summary>
        /// <value><c>true</c> if this instance is logged in; otherwise, <c>false</c>.</value>
        public bool IsLoggedIn
        {
            get
            {
                if (HaccpAppSettings.SharedInstance.CurrentUserId <= 0)
                    return false;
                return true;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is menu check list active.
        /// </summary>
        /// <value><c>true</c> if this instance is menu check list active; otherwise, <c>false</c>.</value>
        public bool IsMenuCheckListActive
        {
            get
            {
                if (IsLoggedIn && (HaccpAppSettings.SharedInstance.Permission == HaccpConstant.UserPermisionElevated))
                    return true;
                return false;
            }
        }

        /// <summary>
        ///     Gets the device I.
        /// </summary>
        /// <value>The device I.</value>
        public string DeviceId
        {
            get { return HaccpAppSettings.SharedInstance.DeviceId; }
        }

        /// <summary>
        ///     Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }


		public bool IsProgressVisible {
			get { return _isProgressVisible; }
			set { SetProperty (ref _isProgressVisible, value, IsProgressVisiblePropertyName); }
		}

        /// <summary>
        ///     Gets the log in command.
        /// </summary>
        /// <value>The log in command.</value>
        public Command LogInCommand
        {
            get
            {
                return _logInCommand ??
                       (_logInCommand = new Command(async () => await ExecuteLogInCommand(), () => !IsBusy));
            }
        }

        /// <summary>
        ///     Gets the back command.
        /// </summary>
        /// <value>The back command.</value>
        public Command BackCommand
        {
            get
            {
                return _backCommand ??
                       (_backCommand = new Command(async () => await Page.PopPage(), () => !IsBusy));
            }
        }

        /// <summary>
        ///     Gets the not implemented command.
        /// </summary>
        /// <value>The not implemented command.</value>
        public Command NotImplementedCommand
        {
            get
            {
                return _notImplementedCommand ??
                       (_notImplementedCommand =
                           new Command(ExecuteNotImplementedCommand, canExecute: () => !IsBusy));
            }
        }

        /// <summary>
        ///     Gets the not implemented command. Will not execcute if not logined.
        /// </summary>
        /// <value>The not implemented command conditional.</value>
        public Command NotImplementedCommandConditional
        {
            get
            {
                return _notImplementedCommand ??
                       (_notImplementedCommand =
                           new Command(ExecuteNotImplementedCommandConditional, () => !IsBusy));
            }
        }
     
        /// <summary>
        /// IsWindows
        /// </summary>
        public bool IsWindows
        {
            set { SetProperty(ref _isWindows, value); }
            get { return _isWindows; }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Raises the login changed event.
        /// </summary>
        protected virtual void OnLoginChanged()
        {
            OnPropertyChanged(IsLoggedInPropertyName);
            OnPropertyChanged(IsMenuCheckListActivePropertyName);
        }

        /// <summary>
        ///     Raises the check pending record changed event.
        /// </summary>
        protected virtual void OnCheckPendingChanged()
        {
            OnPropertyChanged(CheckPendingPropertyName);
        }

        /// <summary>
        ///     Called when the view appears.
        /// </summary>
        public virtual void OnViewAppearing()
        {
        }

        /// <summary>
        ///     Called when the view disappears.
        /// </summary>
        public virtual void OnViewDisappearing()
        {
        }

        /// <summary>
        ///     Sets the property.
        /// </summary>
        /// <returns><c>true</c>, if property was set, <c>false</c> otherwise.</returns>
        /// <param name="backingStore">Backing store.</param>
        /// <param name="value">Value.</param>
        /// <param name="propertyName">Property name.</param>
        /// <param name="onChanged">On changed.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public bool SetProperty<T>(
            ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;

            if (onChanged != null)
                onChanged();

            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        ///     Raises the property changed event.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// UnSubscribe
        /// </summary>
        public void UnSubscribe()
        {
            MessagingCenter.Unsubscribe<HaccpAppSettings>(this, HaccpConstant.MsCheckpending);
        }


        /// <summary>
        ///     Shows the not implemented.
        /// </summary>
        protected void ShowNotImplemented()
        {
            Page.DisplayAlertMessage(HACCPUtil.GetResourceString("NotImplemented"), "");
        }

        /// <summary>
        ///     Executes the log in command.
        /// </summary>
        /// <returns>The log in command.</returns>
        private async Task ExecuteLogInCommand()
        {
            var shouldShowSelectUser = false;
            if (IsBusy)
                return;

            IsBusy = true;
            LogInCommand.ChangeCanExecute();
            try
            {
                if (IsLoggedIn)
                {
                    var checkLogout = await ShowLogoutConfirm();
                    if (checkLogout)
                        HaccpAppSettings.SharedInstance.CurrentUserId = 0;
                }
                else
                    shouldShowSelectUser = true;

                if (shouldShowSelectUser)
                {
                    IDataStore dataStore = new SQLiteDataStore();
                    var isUserExist = dataStore.CheckUsersExist();
                    if (isUserExist)
                        await Page.NavigateTo(PageEnum.SelectUser, true);
                    else
						await Page.ShowAlert(HACCPUtil.GetResourceString("NoUsersFound"),
                            HACCPUtil.GetResourceString("NousersfoundPleasetapUpdateUserListintheWirelessTasksmenu"));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ooops! Something went wrong while login. Exception: {0}", ex);
            }
            finally
            {
                IsBusy = false;
                LogInCommand.ChangeCanExecute();
            }
        }

        /// <summary>
        ///     Shows the logout confirm.
        /// </summary>
        /// <returns>The logout confirm.</returns>
        protected async Task<bool> ShowLogoutConfirm()
        {
            return
                await
                    Page.ShowConfirmAlert(HACCPUtil.GetResourceString("Logout"),
                        HACCPUtil.GetResourceString("Areyousureyouwanttologout"));
        }

        /// <summary>
        ///     Executes the not implemented command.
        /// </summary>
        private void ExecuteNotImplementedCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            NotImplementedCommand.ChangeCanExecute();
            try
            {
                ShowNotImplemented();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ooops! Something went wrong while perform requested action. Exception: {0}", ex);
            }
            finally
            {
                IsBusy = false;
                NotImplementedCommand.ChangeCanExecute();
            }
        }

        /// <summary>
        ///     Executes the not implemented command conditional (if not logged in not show alert).
        /// </summary>
        private void ExecuteNotImplementedCommandConditional()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            NotImplementedCommandConditional.ChangeCanExecute();
            try
            {
                if (IsLoggedIn)
                    ShowNotImplemented();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ooops! Something went wrong while perform requested action. Exception: {0}", ex);
            }
            finally
            {
                IsBusy = false;
                NotImplementedCommandConditional.ChangeCanExecute();
            }
        }

        #endregion
    }
}