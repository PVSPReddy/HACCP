using System;
using System.Diagnostics;
using System.Linq;
using HACCP.Core;
using Xamarin.Forms;

namespace HACCP
{
    public partial class CategoryReview : BaseView
    {

        #region Member Variable

        private readonly CategoryReviewViewModel _viewModel;

        private CheckListResponse _selectedItem;

        #endregion

        /// <summary>
        /// CategoryReview Constructor
        /// </summary>
        /// <param name="category"></param>
        public CategoryReview(object category)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, string.Empty);
            BindingContext = _viewModel = new CategoryReviewViewModel(this, category as Category);

            QuestionListView.ItemSelected += (sender, e) =>
            {
                if (QuestionListView.SelectedItem == null)
                    return;

                var response = _selectedItem = (CheckListResponse) QuestionListView.SelectedItem;

                ShowPopupData(response);
                QuestionListView.SelectedItem = null;
            };
        }


        #region Event Handlers

        /// <summary>
        /// PrevButtonClick Event
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

            nextImage.Source = "next";
            nextButton.IsEnabled = true;

            _selectedItem = list[index - 1];
            var response = _viewModel.GetResponseByQuestionId(_selectedItem.QuestionId);

            ShowPopupData(response);
        }

        /// <summary>
        /// NextButtonClick Event
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

            var response = _viewModel.GetResponseByQuestionId(_selectedItem.QuestionId);

            ShowPopupData(response);
        }


        #endregion

        #region Methods


        /// <summary>
        ///To  Show Popup 
        /// </summary>
        /// <param name="response"></param>
        public void ShowPopupData(CheckListResponse response)
        {
            var list = _viewModel.Records;
            var item = list.FirstOrDefault(x => x.RecordNo == _selectedItem.RecordNo);
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
            TimeStamp.Text = string.Format("{0}: {1}", HACCPUtil.GetResourceString("Time"),
                HACCPUtil.GetFormattedDate(date, false));
        }

        /// <summary>
        /// OnAppearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.CurrentPageType = typeof(CategoryReview);

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