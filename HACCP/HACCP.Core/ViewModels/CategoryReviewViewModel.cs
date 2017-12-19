using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace HACCP.Core
{
    public class CategoryReviewViewModel : BaseViewModel
    {
        #region Member Variables

        private readonly IDataStore _dataStore;
        private string _categoryName;
        private bool _hasItems;
        private bool _isReviewAnswerVisible;
        private ObservableCollection<CheckListResponse> _questions;
        private Command _reviewAnswerOkCommand;

        #endregion

        /// <summary>
        ///     Initializes a new instance of the <see cref="HACCP.Core.CategoryReviewViewModel" /> class.
        /// </summary>
        /// <param name="page">Page.</param>
        /// <param name="category">Category.</param>
        public CategoryReviewViewModel(IPage page, Category category) : base(page)
        {
            _dataStore = new SQLiteDataStore();
            IsReviewAnswerVisible = false;
            CategoryName = category.CategoryName;
            var items = _dataStore.GetChecklistResponseCollectionById(category.CategoryId);
            Records = new ObservableCollection<CheckListResponse>(items);
            HasItems = Records != null && Records.Count > 0;


            MessagingCenter.Subscribe<UploadRecordRefreshMessage>(this, HaccpConstant.UploadRecordRefresh, sender =>
                {
                    var list = _dataStore.GetChecklistResponseCollectionById(category.CategoryId);
                    Records = new ObservableCollection<CheckListResponse>(list);
                    HasItems = Records != null && Records.Count > 0;
                    IsReviewAnswerVisible = false;
                });
        }

        #region Properties

        /// <summary>
        ///     Gets or sets the questions.
        /// </summary>
        /// <value>The questions.</value>
        public ObservableCollection<CheckListResponse> Records
        {
            get { return _questions; }
            set { SetProperty(ref _questions, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is review answer visible.
        /// </summary>
        /// <value><c>true</c> if this instance is review answer visible; otherwise, <c>false</c>.</value>
        public bool IsReviewAnswerVisible
        {
            get { return _isReviewAnswerVisible; }
            set { SetProperty(ref _isReviewAnswerVisible, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance has items.
        /// </summary>
        /// <value><c>true</c> if this instance has items; otherwise, <c>false</c>.</value>
        public bool HasItems
        {
            get { return _hasItems; }
            set { SetProperty(ref _hasItems, value); }
        }

        /// <summary>
        ///     Gets or sets the name of the category.
        /// </summary>
        /// <value>The name of the category.</value>
        public string CategoryName
        {
            get { return _categoryName; }
            set { SetProperty(ref _categoryName, value); }
        }

        /// <summary>
        ///     Gets the review answer OK command.
        /// </summary>
        /// <value>The review answer OK command.</value>
        public Command ReviewAnswerOkCommand
        {
            get
            {
                return _reviewAnswerOkCommand ??
                       (_reviewAnswerOkCommand =
                           new Command(ExecuteReviewAnswerOkCommand, () => !IsBusy && IsLoggedIn));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the response by question identifier.
        /// </summary>
        /// <returns>The response by question identifier.</returns>
        /// <param name="questionId">Question identifier.</param>
        public CheckListResponse GetResponseByQuestionId(long questionId)
        {
            return _dataStore.GetChecklistResponseById(questionId);
        }

        /// <summary>
        ///     Executes the review answer OK command.
        /// </summary>
        /// <returns>The review answer OK command.</returns>
        private void ExecuteReviewAnswerOkCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;
            ReviewAnswerOkCommand.ChangeCanExecute();

            IsReviewAnswerVisible = false;

            IsBusy = false;
            ReviewAnswerOkCommand.ChangeCanExecute();
        }

        /// <summary>
        /// OnViewDisappearing
        /// </summary>
        public override void OnViewDisappearing()
        {
            base.OnViewDisappearing();
            MessagingCenter.Unsubscribe<UploadRecordRefreshMessage>(this, HaccpConstant.UploadRecordRefresh);
        }

        #endregion
    }
}