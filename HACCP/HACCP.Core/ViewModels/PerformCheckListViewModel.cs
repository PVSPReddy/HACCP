using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HACCP.Core
{
    public class PerformCheckListViewModel : BaseViewModel
    {
        #region Member Variables

        private readonly IDataStore dataStore;
        private ObservableCollection<Category> categories;
        public bool isBackNavigation = true;
        private bool isCategoryExists;
        private short recordStatus;
        private Category selectedCategory;
        private Command logInCommand;

        #endregion

        /// <summary>
        /// PerformCheckListViewModel Constructor
        /// </summary>
        /// <param name="page"></param>
        public PerformCheckListViewModel(IPage page) : base(page)
        {
            dataStore = new SQLiteDataStore();
            var categorylist = dataStore.GetCategories();

            var enumerable = categorylist as IList<Category> ?? categorylist.ToList();
            if (categorylist != null && !enumerable.Any())
            {
                isCategoryExists = false;
            }
            else
            {
                isCategoryExists = true;
            }

            Categories = new ObservableCollection<Category>(enumerable);


            MessagingCenter.Subscribe<CategoryStatus>(this, HaccpConstant.CategoryMessage, sender =>
            {
                var cat = sender;
                if (cat != null)
                {
                    var selectedCat = Categories.FirstOrDefault(x => x.CategoryId == cat.CategoryId);
                    if (selectedCat != null)
                    {
                        selectedCat.RecordStatus = dataStore.GetCategoryRecordStatus(selectedCat.CategoryId);
                    }
                }
            });

            MessagingCenter.Subscribe<UploadRecordRefreshMessage>(this, HaccpConstant.UploadRecordRefresh, sender =>
            {
                var list = dataStore.GetCategories();
                var collection = list as IList<Category> ?? list.ToList();
                if (list != null && !collection.Any())
                {
                    isCategoryExists = false;
                }
                else
                {
                    isCategoryExists = true;
                }
                Categories = new ObservableCollection<Category>(collection);
            });
        }

        #region Properties

        /// <summary>
        /// Gets or sets the categories.
        /// </summary>
        /// <value>The categories.</value>
        public ObservableCollection<Category> Categories
        {
            get { return categories; }
            set { SetProperty(ref categories, value); }
        }


        /// <summary>
        ///     Gets or sets the selected category.
        /// </summary>
        /// <value>The selected category.</value>
        public Category SelectedCategory
        {
            get { return selectedCategory; }
            set
            {
                SetProperty(ref selectedCategory, value);
                if (value != null)
                {
                    isBackNavigation = false;
                    LoadQuestions();
                }
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is category exists.
        /// </summary>
        /// <value><c>true</c> if this instance is category exists; otherwise, <c>false</c>.</value>
        public bool IsCategoryExists
        {
            get { return isCategoryExists; }
            set { SetProperty(ref isCategoryExists, value); }
        }

        /// <summary>
        ///     Gets or sets the record status.
        /// </summary>
        /// <value>The record status.</value>
        public short RecordStatus
        {
            get { return recordStatus; }
            set { SetProperty(ref recordStatus, value); }
        }

        /// <summary>
        ///     Gets the save command.
        /// </summary>
        /// <value>The save command.</value>
        public new Command LogInCommand
        {
            get
            {
                return logInCommand ??
                       (logInCommand = new Command(ExecuteLogInCommand, () => !IsBusy));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// OnViewDisappearing
        /// </summary>
        public override void OnViewDisappearing()
        {
            if (isBackNavigation)
            {
                MessagingCenter.Unsubscribe<CategoryStatus>(this, HaccpConstant.CategoryMessage);
                MessagingCenter.Unsubscribe<UploadRecordRefreshMessage>(this, HaccpConstant.UploadRecordRefresh);
            }
            base.OnViewDisappearing();
            isBackNavigation = true;
        }


        /// <summary>
        ///     Executes the log in command.
        /// </summary>
        /// <returns>The log in command.</returns>
        private async void ExecuteLogInCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            LogInCommand.ChangeCanExecute();
            var checkLogout = false;
            try
            {
                checkLogout = await ShowLogoutConfirm();
                if (checkLogout)
                    HaccpAppSettings.SharedInstance.CurrentUserId = 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ooops! Something went wrong while login. Exception: {0}", ex);
            }
            finally
            {
                IsBusy = false;
                LogInCommand.ChangeCanExecute();
                if (checkLogout)
                    Page.PopPage();
            }
        }

        /// <summary>
        ///     Loads the questions.
        /// </summary>
        /// <returns>The questions.</returns>
        public async Task LoadQuestions()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            if (dataStore.CheckQuestionExists(SelectedCategory.CategoryId))
                await Page.NavigateToWithSelectedObject(PageEnum.SelectQuestion, true, SelectedCategory);
            else
                await Page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("Noquestionsfound"));


            IsBusy = false;
        }

        #endregion
    }
}