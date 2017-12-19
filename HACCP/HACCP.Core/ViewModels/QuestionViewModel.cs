using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HACCP.Core
{
    public class QuestionViewModel : BaseViewModel
    {
        #region Member Variables

        private readonly IDataStore dataStore;
        private Command filterCommand;
        public bool isBackNavigation = true;
        private bool isFilter;
        private bool isQuestionExists;
        private bool isReviewAnswerVisible;
        private ObservableCollection<Question> questions;
        private short recordStatus;
        private Command reviewAnswerOKCommand;
        private Command reviewListCommand;
        private Question selectedQuestion;


        #endregion


        /// <summary>
        ///     Initializes a new instance of the <see cref="HACCP.Core.QuestionViewModel" /> class.
        /// </summary>
        /// <param name="page">Page.</param>
        /// <param name="category"></param>
        public QuestionViewModel(IPage page, Category category) : base(page)
        {
            AutoBack = false;
            dataStore = new SQLiteDataStore();
            SelectedCategory = category;
            SelectedCategoryId = category.CategoryId;
            Title = HACCPUtil.GetResourceString("SelectQuestion");
            Subtitle = category.CategoryName;
            LoadQuestions(false);

            MessagingCenter.Subscribe<Question>(this, HaccpConstant.QuestionMessage, sender =>
            {
                var ques = sender;
                if (ques != null)
                {
                    var selectedQues = Questions.FirstOrDefault(x => x.QuestionId == ques.QuestionId);
                    if (selectedQues != null)
                        Questions.FirstOrDefault(x => x.QuestionId == ques.QuestionId).RecordStatus = 1;
                }
            });

            MessagingCenter.Subscribe<AutoAdvanceCheckListMessage>(this, HaccpConstant.AutoadvancechecklistMessage,
                sender =>
                {
                    var message = sender;
                    var currentItem = Questions.FirstOrDefault(x => x.QuestionId == message.CurrentId);
                    if (currentItem != null)
                    {
                        var index = Questions.IndexOf(currentItem);
                        if (Questions.Count - 1 > index)
                        {
                            isBackNavigation = false;
						   
							var ques = Questions [index + 1];
							ques.CategoryName = Subtitle;
                            page.NavigateToWithSelectedObject(PageEnum.RecordAnswer, true, ques);
                        }
                        else
                        {
                            AutoBack = true;
                            if (!HaccpAppSettings.SharedInstance.IsWindows)
                                page.PopPage();
                        }
                    }
                });


            MessagingCenter.Subscribe<UploadRecordRefreshMessage>(this, HaccpConstant.UploadRecordRefresh, sender =>
            {
                LoadQuestions(!isFilter);
                IsReviewAnswerVisible = false;
            });
        }

        #region Properties

        /// <summary>
        /// AutoBack
        /// </summary>
        public bool AutoBack { get; set; }

        /// <summary>
        ///     Gets or sets the selected category identifier.
        /// </summary>
        /// <value>The selected category identifier.</value>
        public long SelectedCategoryId { get; set; }

        /// <summary>
        ///     Gets or sets the selected category.
        /// </summary>
        /// <value>The selected category.</value>
        public Category SelectedCategory { get; set; }

        /// <summary>
        ///     Gets or sets the questions.
        /// </summary>
        /// <value>The questions.</value>
        public ObservableCollection<Question> Questions
        {
            get { return questions; }
            set { SetProperty(ref questions, value); }
        }

        /// <summary>
        ///     Gets or sets the selected question.
        /// </summary>
        /// <value>The selected question.</value>
        public Question SelectedQuestion
        {
            get { return selectedQuestion; }
            set
            {
                SetProperty(ref selectedQuestion, value);
                if (value != null)
                {
                    isBackNavigation = false;
                    LoadCheckList(value);
                }
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is question exists.
        /// </summary>
        /// <value><c>true</c> if this instance is question exists; otherwise, <c>false</c>.</value>
        public bool IsQuestionExists
        {
            get { return isQuestionExists; }
            set { SetProperty(ref isQuestionExists, value); }
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
        ///     Gets or sets a value indicating whether this instance is review answer visible.
        /// </summary>
        /// <value><c>true</c> if this instance is review answer visible; otherwise, <c>false</c>.</value>
        public bool IsReviewAnswerVisible
        {
            get { return isReviewAnswerVisible; }
            set { SetProperty(ref isReviewAnswerVisible, value); }
        }


        /// <summary>
        ///     Gets the filter command.
        /// </summary>
        /// <value>The filter command.</value>
        public Command FilterCommand
        {
            get
            {
                return filterCommand ??
                       (filterCommand =
                           new Command(async () => await ExecuteFilterCommand(), () => !IsBusy && IsLoggedIn));
            }
        }

        /// <summary>
        ///     Gets the review answer OK command.
        /// </summary>
        /// <value>The review answer OK command.</value>
        public Command ReviewAnswerOKCommand
        {
            get
            {
                return reviewAnswerOKCommand ??
                       (reviewAnswerOKCommand =
                           new Command(ExecuteReviewAnswerOKCommand, () => !IsBusy && IsLoggedIn));
            }
        }

        /// <summary>
        ///     Gets the save command.
        /// </summary>
        /// <value>The save command.</value>
        public Command ReviewListCommand
        {
            get
            {
                return reviewListCommand ??
                       (reviewListCommand = new Command(ExecuteReviewListCommand, () => !IsBusy));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when the view appears.
        /// </summary>
        public override void OnViewAppearing()
        {
            base.OnViewAppearing();
            if (HaccpAppSettings.SharedInstance.IsWindows && HaccpAppSettings.SharedInstance.DeviceSettings.AutoAdvance &&
                AutoBack)
            {
                Page.PopPage();
            }
        }

        /// <summary>
        /// OnViewDisappearing
        /// </summary>
        public override void OnViewDisappearing()
        {
            if (isBackNavigation)
            {
                MessagingCenter.Unsubscribe<CategoryStatus>(this, HaccpConstant.CategoryMessage);
                MessagingCenter.Unsubscribe<AutoAdvanceCheckListMessage>(this, HaccpConstant.AutoadvancechecklistMessage);
                MessagingCenter.Unsubscribe<UploadRecordRefreshMessage>(this, HaccpConstant.UploadRecordRefresh);
            }
            base.OnViewDisappearing();
            isBackNavigation = true;
        }

        /// <summary>
        ///     Executes the review list command.
        /// </summary>
        private void ExecuteReviewListCommand()
        {
            isBackNavigation = false;
            Page.NavigateToWithSelectedObject(PageEnum.CategoryReview, true, SelectedCategory);
        }

        /// <summary>
        ///     Loads the questions.
        /// </summary>
        public async Task LoadQuestions(bool isFilterStatus)
        {
			isFilter = isFilterStatus;

            Title = HACCPUtil.GetResourceString("SelectQuestion");
            if (isFilter)
                Title = HACCPUtil.GetResourceString("ReviewChecklist");
        
            var questionLIst = dataStore.GetQuestions(SelectedCategoryId, isFilter);
            var filteredList = new List<Question>();
            if (isFilter)
            {
                foreach (var question in questionLIst)
                {
                    var response = dataStore.GetChecklistResponseById(question.QuestionId);
                    if ((question.QuestionType == (short) QuestionType.YesOrNo &&
                         response.Answer == HACCPUtil.GetResourceString("No")) ||
                        (question.QuestionType != (short) QuestionType.YesOrNo &&
                         (HACCPUtil.ConvertToDouble(response.Answer) < HACCPUtil.ConvertToDouble(question.Min) ||
                          HACCPUtil.ConvertToDouble(response.Answer) > HACCPUtil.ConvertToDouble(question.Max))))
                    {
                        filteredList.Add(question);
                    }
                }

                questionLIst = filteredList;
            }

            isFilter = !isFilter;
            Questions = new ObservableCollection<Question>(questionLIst);
            if (Questions.Count == 0)
            {
                if (isFilterStatus)
                {
                    //this.page.ShowAlert ("", AppResources.Noquestionsfound);
                    IsQuestionExists = true;
                    //LoadQuestions (!isFilterStatus);
                }
                else
                {
                    IsQuestionExists = false;
                    await Page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("Noquestionsfound"));
                    await Page.PopPage();
                }
            }
        }

        /// <summary>
        ///     Loads the check list.
        /// </summary>
        /// <param name="question">Question.</param>
        public async void LoadCheckList(Question question)
        {
            if (IsBusy)
                return;

            //  await page.ShowAlert("", HACCPUtil.GetResourceString("NotImplemented"));
            //  return;

            IsBusy = true;


            if (!isFilter)
            {
                var response = GetResponseByQuestionId(SelectedQuestion.QuestionId);

                //	ShowPopupData (response);

                MessagingCenter.Send(new ShowCheckListReviewMessage(response, SelectedQuestion),
                    HaccpConstant.ChecklistReviewMessage);
            }
            else
            {
                IsReviewAnswerVisible = false;

                isBackNavigation = false;
                SelectedQuestion.CategoryName = Subtitle;
                await Page.NavigateToWithSelectedObject(PageEnum.RecordAnswer, true, SelectedQuestion);
            }

            IsBusy = false;
        }

        /// <summary>
        ///     Gets the response by question identifier.
        /// </summary>
        /// <returns>The response by question identifier.</returns>
        /// <param name="questionId">Question identifier.</param>
        public CheckListResponse GetResponseByQuestionId(long questionId)
        {
            return dataStore.GetChecklistResponseById(questionId);
        }

        /// <summary>
        ///     Executes the filter command.
        /// </summary>
        /// <returns>The filter command.</returns>
        private async Task ExecuteFilterCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;
            FilterCommand.ChangeCanExecute();
            IsQuestionExists = false;
            await LoadQuestions(isFilter);

            IsBusy = false;
            FilterCommand.ChangeCanExecute();
        }


        /// <summary>
        ///     Executes the review answer OK command.
        /// </summary>
        /// <returns>The review answer OK command.</returns>
        private void ExecuteReviewAnswerOKCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;
            ReviewAnswerOKCommand.ChangeCanExecute();

            IsReviewAnswerVisible = false;

            IsBusy = false;
            ReviewAnswerOKCommand.ChangeCanExecute();
        }

        #endregion
    }
}