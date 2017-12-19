using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HACCP.Core.Models;
using Xamarin.Forms;

namespace HACCP.Core
{
    public class UsersViewModel : BaseViewModel
    {
        private readonly long currentLoggedInUserID;
        private Command cancelCommand;
        private Command checkLoginCommand;
        private Command homeCommand;
        public bool isViewAppeared = false;
        private User loggedUser;
        private string loginPassword = string.Empty;
        public User selectedUser;
        private long selectedUserId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="HACCP.Core.UsersViewModel" /> class.
        /// </summary>
        /// <param name="page">Page.</param>
        public UsersViewModel(IPage page)
            : base(page)
        {
            try
            {
                IDataStore dataStore = new SQLiteDataStore();
                var userList = dataStore.GetUsers();
                Users = new ObservableCollection<User>(userList);
                UsersGrouped = new ObservableCollection<Grouping<string, User>>();
                Sort();
                currentLoggedInUserID = HaccpAppSettings.SharedInstance.CurrentUserId;

                if (currentLoggedInUserID > 0)
                {
                    LoggedUser = Users.SingleOrDefault(u => u.Id == currentLoggedInUserID);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error User : {0}", ex.Message);
            }
        }

        #region Properties

        /// <summary>
        ///     Gets or sets the users.
        /// </summary>
        /// <value>The users.</value>
        public ObservableCollection<User> Users { get; set; }


        /// <summary>
        ///     Gets or sets the users grouped.
        /// </summary>
        /// <value>The users grouped.</value>
        public ObservableCollection<Grouping<string, User>> UsersGrouped { get; set; }

        /// <summary>
        ///     Gets or sets the login password.
        /// </summary>
        /// <value>The login password.</value>
        public string LoginPassword
        {
            get { return loginPassword; }
            set
            {
                if (SetProperty(ref loginPassword, value))
                {
                    CheckLoginCommand.ChangeCanExecute();
                    OnPropertyChanged("IsLoginEnabled");
                }
            }
        }


        /// <summary>
        ///     Gets a value indicating whether this instance is login enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is login enabled; otherwise, <c>false</c>.</value>
        public bool IsLoginEnabled
        {
            get { return LoginPassword.Trim().Length > 0; }
        }


        /// <summary>
        ///     Gets or sets the selected user identifier.
        /// </summary>
        /// <value>The selected user identifier.</value>
        public long SelectedUserId
        {
            get { return selectedUserId; }
            set
            {
                if (SetProperty(ref selectedUserId, value))
                    OnPropertyChanged("DisplayPasswordPopup");
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="HACCP.Core.UsersViewModel" /> display password popup.
        /// </summary>
        /// <value><c>true</c> if display password popup; otherwise, <c>false</c>.</value>
        public bool DisplayPasswordPopup
        {
            get
            {
                if (HaccpAppSettings.SharedInstance.DeviceSettings.RequirePin == 1)
                    return selectedUserId > 0;
                return false;
            }
        }

        /// <summary>
        ///     Gets or sets the logged user.
        /// </summary>
        /// <value>The logged user.</value>
        public User LoggedUser
        {
            get { return loggedUser; }
            set { SetProperty(ref loggedUser, value); }
        }

        /// <summary>
        ///     Gets the check login command.
        /// </summary>
        /// <value>The check login command.</value>
        public Command CheckLoginCommand
        {
            get
            {
                return checkLoginCommand ??
                       (checkLoginCommand =
                           new Command(async () => await CheckLogin(), () => !IsBusy ));
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance cancel command.
        /// </summary>
        /// <value><c>true</c> if this instance cancel command; otherwise, <c>false</c>.</value>
        public Command CancelCommand
        {
            get
            {
                return cancelCommand ??
                       (cancelCommand = new Command(CancelLogin, () => !IsBusy));
            }
        }


        public Command HomeCommand
        {
            get { return homeCommand ?? (homeCommand = new Command(() => { Page.ReloadHomePage(); })); }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when the view appears.
        /// </summary>
        public override void OnViewAppearing()
        {
            base.OnViewAppearing();
            if (Users.Count < 1)
            {
                Page.DisplayAlertMessage(HACCPUtil.GetResourceString("NoUsersFound"),
                    HACCPUtil.GetResourceString("NousersfoundPleasetapUpdateUserListintheWirelessTasksmenu"));
            }
        }

        /// <summary>
        ///     Raises the login changed event.
        /// </summary>
        protected override void OnLoginChanged()
        {
            base.OnLoginChanged();
            try
            {
                if (IsLoggedIn == false && currentLoggedInUserID > 0)
                {
                    var user = Users.First(u => u.Id == currentLoggedInUserID);
                    user.RaisePropertyChangeEvent("IsSelected");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error OnLoginChanged {0}", ex.Message);
            }
        }

        /// <summary>
        ///     Sort this instance.
        /// </summary>
        private void Sort()
        {
            try
            {
                UsersGrouped.Clear();

                /*var sorted = from user in Users.Where(u=>u.ID != HACCPAppSettings.SharedInstance.CurrentUserID)
                         orderby user.NAME
                         group user by user.NAME.Substring (0, 1) into userGroup
                         select new Grouping<string, User> (userGroup.Key, userGroup);


            var currentUser = Users.Where(u=>u.ID == HACCPAppSettings.SharedInstance.CurrentUserID);

            if (currentUser.Any()) {
                usersGrouped.Add (new Grouping<string, User> ("", currentUser));
            }*/

                var sorted = from user in Users
                    orderby user.Name
                    group user by user.Name.Substring(0, 1).ToUpper()
                    into userGroup
                    select new Grouping<string, User>(userGroup.Key, userGroup);

                foreach (var sort in sorted)
                {
                    UsersGrouped.Add(sort);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error Sort {0}", ex.Message);
            }
        }

        /// <summary>
        ///     Checks the login.
        /// </summary>
        /// <returns>The login.</returns>
        private async Task CheckLogin()
        {
            if (IsBusy || !DisplayPasswordPopup)
                return;

            IsBusy = true;
            CheckLoginCommand.ChangeCanExecute();
            try
            {
                //isValidPassword = this.page.
                if (!string.IsNullOrEmpty(loginPassword))
                {
                    if (loginPassword == selectedUser.Pin)
                    {
                        Page.EndEditing();
                        HaccpAppSettings.SharedInstance.UserName = selectedUser.Name;
                        HaccpAppSettings.SharedInstance.CurrentUserId = SelectedUserId;
                        HaccpAppSettings.SharedInstance.Permission = selectedUser.Permision;
                        OnPropertyChanged(IsMenuCheckListActivePropertyName);
                        Settings.LastLoginUserId = SelectedUserId;
                        await Page.PopPage();
                    }
                    else
                    {
                        LoginPassword = string.Empty;
                        await
                            Page.ShowAlert(HACCPUtil.GetResourceString("Login"),
                                HACCPUtil.GetResourceString("PasswordincorrectPleasetryagain"));
                        // this.page.DisplayAlertMessage(HACCPUtil.GetResourceString("Login"), HACCPUtil.GetResourceString("PasswordincorrectPleasetryagain"));
                        MessagingCenter.Send(new UserPasswordFocusMessage(true), HaccpConstant.UserPasswordFocusMessage);
                    }
                }
                else
                {
                    Page.DisplayAlertMessage(HACCPUtil.GetResourceString("Login"),
                        HACCPUtil.GetResourceString("Enteryourpassword"));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error On Login. Exception: {0}", ex);
            }
            finally
            {
                IsBusy = false;
                CheckLoginCommand.ChangeCanExecute();
            }
        }

        /// <summary>
        ///     Determines whether this instance cancel login.
        /// </summary>
        /// <returns><c>true</c> if this instance cancel login; otherwise, <c>false</c>.</returns>
        private void CancelLogin()
        {
            if (IsBusy)
                return;
            Page.EndEditing();
            IsBusy = true;
            CancelCommand.ChangeCanExecute();
            try
            {
                LoginPassword = string.Empty;
                SelectedUserId = 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error On logout. Exception: {0}", ex);
            }
            finally
            {
                IsBusy = false;
                CancelCommand.ChangeCanExecute();
            }
        }

        #endregion
    }
}