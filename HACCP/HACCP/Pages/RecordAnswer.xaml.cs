using HACCP.Core;
using Xamarin.Forms;

namespace HACCP
{
    public partial class RecordAnswer : BaseView
    {
        #region Member Variable

        private readonly RecordAnswerViewModel _viewModel;

        #endregion

        /// <summary>
        /// Record Answer screen constructor
        /// </summary>
        /// <param name="selectedQuestion"></param>
        public RecordAnswer(object selectedQuestion)
        {
            InitializeComponent();

            var question = (Question) selectedQuestion;

            NavigationPage.SetBackButtonTitle(this, string.Empty);

            BindingContext = _viewModel = new RecordAnswerViewModel(this);
            _viewModel.Subtitle = question.QuestionName;
            _viewModel.RecordResponse = question;

            switch (question.QuestionType)
            {
                case (short) QuestionType.NumericAnswer:
                    _viewModel.IsNumeric = true;
                    _viewModel.IsYesNo = false;
                    break;
                case (short) QuestionType.YesOrNo:
                    _viewModel.IsYesNo = true;
                    _viewModel.IsNumeric = false;
                    _viewModel.Answer = HACCPUtil.GetResourceString("Yes");
                    break;
            }
            correctiveActionList.ItemSelected += (sender, e) =>
            {
                if (correctiveActionList.SelectedItem == null)
                    return;
                var correctiveAction = ((CorrectiveAction) e.SelectedItem).CorrActionName;

                if (!string.IsNullOrEmpty(_viewModel.Answer))
                {
                    if ((_viewModel.IsYesNo && _viewModel.Answer == HACCPUtil.GetResourceString("No")) ||
                        (!_viewModel.IsYesNo &&
                         (HACCPUtil.ConvertToDouble(_viewModel.Answer) <
                          HACCPUtil.ConvertToDouble(_viewModel.RecordResponse.Min) ||
                          HACCPUtil.ConvertToDouble(_viewModel.Answer) >
                          HACCPUtil.ConvertToDouble(_viewModel.RecordResponse.Max))))
                    {
                        if (correctiveAction != HACCPUtil.GetResourceString("None"))
                        {
                            _viewModel.SelectedCorrectiveAction = correctiveAction;
                            _viewModel.IsCorrctiveOptionsVisible = false;
                        }
                    }
                    else
                    {
                        _viewModel.SelectedCorrectiveAction = correctiveAction;
                        _viewModel.IsCorrctiveOptionsVisible = false;
                    }
                }
                else
                {
                    _viewModel.SelectedCorrectiveAction = correctiveAction;
                    _viewModel.IsCorrctiveOptionsVisible = false;
                }
                correctiveActionList.SelectedItem = null;
            };

            actionBtn.Clicked += (sender, e) => { numericValueEntry.Unfocus(); };

            toggleBtn.Clicked += (sender, e) =>
            {
                if (_viewModel.IsNumeric)
                {
                    if (numericValueEntry.IsFocused)
                    {
                        numericValueEntry.Unfocus();
                    }
                    else
                    {
                        numericValueEntry.Focus();
                    }
                }
            };
            if (HaccpAppSettings.SharedInstance.IsWindows)
                ToolbarItems.Add(new ToolbarItem
                {
                    Icon = "logout.png",
                    Order = ToolbarItemOrder.Primary,
                    Command = _viewModel.LogInCommand
                });
        }


        #region Methods

        /// <summary>
        /// OnAppearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.CurrentPageType = typeof(RecordAnswer);
        }

        /// <summary>
        /// OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            correctiveActionList.SelectedItem = null;
            MessagingCenter.Unsubscribe<string>(this, HaccpConstant.RecordAnswerFocus);
        }

        #endregion 
    }
}