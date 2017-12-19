using System;
using System.Diagnostics;
using System.Linq;
using HACCP.Core;
using Xamarin.Forms;

namespace HACCP
{
    public partial class SelectQuestion : BaseView
    {
        #region Member Variables

        private readonly QuestionViewModel _viewModel;
        private bool IsListViewSelected { get; set; }
        private Question _selectedItem;

        #endregion

        /// <summary>
        /// SelectQuestion Constructor
        /// </summary>
        /// <param name="selectedCategory"></param>
        public SelectQuestion(object selectedCategory)
        {
            InitializeComponent();

            var category = (Category) selectedCategory;
            NavigationPage.SetBackButtonTitle(this, string.Empty);

            BindingContext = _viewModel = new QuestionViewModel(this, category);

            ToolbarItems.Add(new ToolbarItem
            {
                Icon = "menu.png",
                Order = ToolbarItemOrder.Primary,
                Command = _viewModel.ReviewListCommand
            });
            ToolbarItems.Add(new ToolbarItem
            {
                Icon = "fault.png",
                Order = ToolbarItemOrder.Primary,
                Command = new Command(FaultIconClick)
            });


            QuestionListView.ItemSelected += (sender, e) =>
            {
                
                if (QuestionListView.SelectedItem == null)
                    return;
                if (!IsListViewSelected || !HaccpAppSettings.SharedInstance.IsWindows)
                {
                    IsListViewSelected = true;
                    _viewModel.SelectedQuestion = QuestionListView.SelectedItem as Question;
                    QuestionListView.SelectedItem = null;
                }
                else
                {
                    IsListViewSelected = false;
                    QuestionListView.SelectedItem = null;
                }
            };
        }

        #region Event Handlers

        /// <summary>
        ///     Faults the icon click.
        /// </summary>
        public void FaultIconClick()
        {
            if (_viewModel.IsReviewAnswerVisible)
                return;

            if (ToolbarItems.Count == 1)
            {
                ToolbarItems.Insert(0, new ToolbarItem
                {
                    Icon = "menu.png",
                    Order = ToolbarItemOrder.Primary,
                    Command = _viewModel.ReviewListCommand
                });
            }
            else
            {
                ToolbarItems.RemoveAt(0);
            }
            _viewModel.FilterCommand.Execute(null);
        }

        /// <summary>
        /// PrevButtonClick Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void PrevButtonClick(object sender, EventArgs args)
        {
            var list = _viewModel.Questions;
            var item = list.FirstOrDefault(x => x.QuestionId == _selectedItem.QuestionId);
            var index = list.IndexOf(item);


            if (index == 1)
            {
                prevImage.Source = "prevDisable.png";
                prevButton.IsEnabled = false;
            }

            nextImage.Source = "next.png";
            nextButton.IsEnabled = true;

            _selectedItem = list[index - 1];
            var response = _viewModel.GetResponseByQuestionId(_selectedItem.QuestionId);

            ShowPopupData(response);
        }

        /// <summary>
        /// NextButtonClick Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void NextButtonClick(object sender, EventArgs args)
        {
            var list = _viewModel.Questions;
            var item = list.FirstOrDefault(x => x.QuestionId == _selectedItem.QuestionId);
            var index = list.IndexOf(item);

            if (index == list.Count - 2)
            {
                nextImage.Source = "nextDisable.png";
                nextButton.IsEnabled = false;
            }
            prevImage.Source = "prev.png";
            prevButton.IsEnabled = true;

            _selectedItem = list[index + 1];

            var response = _viewModel.GetResponseByQuestionId(_selectedItem.QuestionId);

            ShowPopupData(response);
        }

        #endregion

        #region Methods 

        /// <summary>
        /// Show Popup Data
        /// </summary>
        /// <param name="response"></param>
        public void ShowPopupData(CheckListResponse response)
        {
            var list = _viewModel.Questions;
            var item = list.FirstOrDefault(x => x.QuestionId == _selectedItem.QuestionId);
            var index = list.IndexOf(item);


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


            _viewModel.IsReviewAnswerVisible = true;

            questionLabel.Text = response.Question;


            if (response.IsNa == 1)
            {
                correctiveActionLabel.IsVisible = false;
                answerLabel.Text = HACCPUtil.GetResourceString("NA");
            }
            else
            {
                correctiveActionLabel.IsVisible = true;
                correctiveActionLabel.Text = string.Format("{0}: {1}", HACCPUtil.GetResourceString("CorrectiveAction"),
                    response.CorrAction);
               

				answerLabel.Text = response.QuestionType == "2"
					? ((response.Answer == HACCPUtil.GetResourceString("Yes"))?HACCPUtil.GetResourceString("Yes"):HACCPUtil.GetResourceString("No"))
					: response.Answer;
            }

            UserName.Text = string.Format("{0}: {1}", HACCPUtil.GetResourceString("Recordedby"), response.UserName);

            var date = new DateTime(Convert.ToInt32(response.Year), Convert.ToInt32(response.Month),
                Convert.ToInt32(response.Day), Convert.ToInt32(response.Hour), Convert.ToInt32(response.Minute),
                Convert.ToInt32(response.Sec));
            var dateString = date.ToString();


            TimeStamp.Text = string.Format("{0}: {1}", HACCPUtil.GetResourceString("Time"),
                HACCPUtil.GetFormattedDate(dateString));
        }

        /// <summary>
        /// OnAppearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            App.CurrentPageType = typeof(SelectQuestion);

            MessagingCenter.Subscribe<ShowCheckListReviewMessage>(this, HaccpConstant.ChecklistReviewMessage,
                sender =>
                {
                    var item = sender.Response;
                    _selectedItem = sender.Question;
                    ShowPopupData(item);
                });


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
            if (HaccpAppSettings.SharedInstance.IsWindows)
                QuestionListView.SelectedItem = null;
            MessagingCenter.Unsubscribe<ShowCheckListReviewMessage>(this, HaccpConstant.ChecklistReviewMessage);
            MessagingCenter.Unsubscribe<NextPrevButtonClickMessage>(this, HaccpConstant.NextPrevMessage);
        }

        #endregion
    }
}