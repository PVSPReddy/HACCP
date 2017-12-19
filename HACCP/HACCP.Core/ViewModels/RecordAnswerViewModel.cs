using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HACCP.Core
{
    public class RecordAnswerViewModel : RecordViewModel
    {

#region Member Variables

        private readonly IDataStore dataStore;
        private string answer;
        private string correctiveAction = HACCPUtil.GetResourceString("None");
        private bool isCorrctiveOptionsVisible;
        private bool isNumeric;
        private bool isSwitchedOn;
        private bool isYesNo;
        private string numericAnswer;
        private string selectedCorrectiveActionLabel = string.Empty;
        private string toggleImage;
        private Command toggleYesNoImageCommand;
        private string yesNoButtonText;

#endregion

        /// <summary>
        ///     Initializes a new instance of the <see cref="HACCP.Core.RecordAnswerViewModel" /> class.
        /// </summary>
        /// <param name="page">Page.</param>
        public RecordAnswerViewModel(IPage page) : base(page)
        {
            dataStore = new SQLiteDataStore();
            SelectedCorrectiveAction = HACCPUtil.GetResourceString("None");
        }

      
        #region Properties

        /// <summary>
        ///     Gets or sets the numeric answer.
        /// </summary>
        /// <value>The numeric answer.</value>
        public string NumericAnswer
        {
            get { return numericAnswer; }
            set
            {
                Answer = value;
                SetProperty(ref numericAnswer, value);
            }
        }

        /// <summary>
        ///     Gets or sets the answer.
        /// </summary>
        /// <value>The answer.</value>
        public string Answer
        {
            get { return answer; }
            set { SetProperty(ref answer, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is numeric.
        /// </summary>
        /// <value><c>true</c> if this instance is numeric; otherwise, <c>false</c>.</value>
        public bool IsNumeric
        {
            get { return isNumeric; }
            set { SetProperty(ref isNumeric, value); }
        }


        /// <summary>
        ///     Gets or sets a value indicating whether this instance is yes no.
        /// </summary>
        /// <value><c>true</c> if this instance is yes no; otherwise, <c>false</c>.</value>
        public bool IsYesNo
        {
            get { return isYesNo; }
            set { SetProperty(ref isYesNo, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is switched on.
        /// </summary>
        /// <value><c>true</c> if this instance is switched on; otherwise, <c>false</c>.</value>
        public bool IsSwitchedOn
        {
            get { return isSwitchedOn; }
            set
            {
                SetProperty(ref isSwitchedOn, value);
                Answer = HACCPUtil.GetResourceString("Yes");
            }
        }


        /// <summary>
        ///     Gets or sets the record response.
        /// </summary>
        /// <value>The record response.</value>
        public Question RecordResponse { get; set; }

        /// <summary>
        /// IsNA
        /// </summary>
        public bool IsNA { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is corrctive options visible.
        /// </summary>
        /// <value><c>true</c> if this instance is corrctive options visible; otherwise, <c>false</c>.</value>
        public bool IsCorrctiveOptionsVisible
        {
            get { return isCorrctiveOptionsVisible; }
            set { SetProperty(ref isCorrctiveOptionsVisible, value); }
        }

        /// <summary>
        ///     Gets or sets the selected corrective action.
        /// </summary>
        /// <value>The selected corrective action.</value>
        public string SelectedCorrectiveAction
        {
            get { return correctiveAction; }
            set
            {
                SetProperty(ref correctiveAction, value);
                SelectedCorrectiveActionLabel = !string.IsNullOrEmpty(value) ? string.Format("{0}: {1}",HACCPUtil.GetResourceString("CorrectiveAction"), SelectedCorrectiveAction) : string.Empty;
            }
        }

        /// <summary>
        ///     Gets or sets the yes no toggle image.
        /// </summary>
        /// <value>The yes no toggle image.</value>
        public string ToggleImage
        {
            get { return toggleImage; }
            set { SetProperty(ref toggleImage, value); }
        }

        /// <summary>
        ///     Gets or sets the selected corrective action label.
        /// </summary>
        /// <value>The selected corrective action label.</value>
        public string SelectedCorrectiveActionLabel
        {
            get { return selectedCorrectiveActionLabel; }
            set { SetProperty(ref selectedCorrectiveActionLabel, value); }
        }


        /// <summary>
        ///     Gets the toggle yes no image command.
        /// </summary>
        /// <value>The toggle yes no image command.</value>
        public Command ToggleYesNoImageCommand
        {
            get
            {
                return toggleYesNoImageCommand ??
                       (toggleYesNoImageCommand =
                           new Command(ExecuteToggleYesNoImageCommand, () => !IsBusy));
            }
        }


        /// <summary>
        ///     Gets or sets the yes no button text.
        /// </summary>
        /// <value>The yes no button text.</value>
        public string YesNoButtonText
        {
            get { return yesNoButtonText; }
            set { SetProperty(ref yesNoButtonText, value); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// OnViewAppearing
        /// </summary>
        public override void OnViewAppearing()
        {
            base.OnViewAppearing();
            if (IsNumeric)
            {
                ToggleImage = "toggle.png";
                YesNoButtonText = string.Empty;
            }
            else
            {
                ToggleImage = string.Empty;
                YesNoButtonText = HACCPUtil.GetResourceString("No");
            }
        }


        /// <summary>
        ///     Executes the corrective action command.
        /// </summary>
        /// <returns>The corrective action command.</returns>
        protected override void ExecuteCorrectiveActionCommand()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                CorrectiveActionCommand.ChangeCanExecute();
                if (string.IsNullOrEmpty(Answer) && !IsYesNo)
                {
                    Answer = string.Empty;
                    Page.DisplayAlertMessage(HACCPUtil.GetResourceString("RecordAnswer"),
                        HACCPUtil.GetResourceString("Specifyanumericvalue"));
                }
                else
                {
                    CorrectiveActions =
                        new ObservableCollection<CorrectiveAction>(
                            dataStore.GetCorrectiveActionForQuestion(RecordResponse.QuestionId));


                    if (
                        !((IsYesNo && Answer == HACCPUtil.GetResourceString("No")) ||
                          (!IsYesNo &&
                           (HACCPUtil.ConvertToDouble(Answer) < HACCPUtil.ConvertToDouble(RecordResponse.Min) ||
                            HACCPUtil.ConvertToDouble(Answer) > HACCPUtil.ConvertToDouble(RecordResponse.Max)))))
                        CorrectiveActions.Insert(0, new CorrectiveAction {CorrActionId = -1, CorrActionName = "None"});

                    if (CorrectiveActions.Count < 1)
                    {
                        Page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("Nocorrectiveactionsfound"));
                    }
                    else
                        IsCorrctiveOptionsVisible = true;
                }
                IsBusy = false;
                CorrectiveActionCommand.ChangeCanExecute();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ooops! Something went wrong. Exception: {0}", ex);
            }
            finally
            {
                IsBusy = false;
                CorrectiveActionCommand.ChangeCanExecute();
            }
        }


        /// <summary>
        ///     Executes the save command.
        /// </summary>
        /// <returns>The save command.</returns>
        protected override async Task ExecuteSaveCommand()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                SaveCommand.ChangeCanExecute();

                if (string.IsNullOrEmpty(Answer) && !IsYesNo)
                {
                    Answer = string.Empty;
                    Page.DisplayAlertMessage(HACCPUtil.GetResourceString("RecordAnswer"),
                        HACCPUtil.GetResourceString("Specifyanumericvalue"));
                }
                else if (SelectedCorrectiveAction == HACCPUtil.GetResourceString("None"))
                {
                    if ((IsYesNo && Answer != HACCPUtil.GetResourceString("Yes")) ||
                        (!IsYesNo &&
                         (HACCPUtil.ConvertToDouble(Answer) < HACCPUtil.ConvertToDouble(RecordResponse.Min) ||
                          HACCPUtil.ConvertToDouble(Answer) > HACCPUtil.ConvertToDouble(RecordResponse.Max))))
                    {
                        CorrectiveActions =
                            new ObservableCollection<CorrectiveAction>(
                                dataStore.GetCorrectiveActionForQuestion(RecordResponse.QuestionId));
                        if (CorrectiveActions != null && CorrectiveActions.Count > 0)
                        {
                            IsCorrctiveOptionsVisible = true;
                        }
                        else if (
                            await
                                Page.ShowConfirmAlert(HACCPUtil.GetResourceString("RecordAnswer"),
                                    string.Format(
                                        HACCPUtil.GetResourceString("RecordedAnswer01CorrectiveActiontaken2"),
                                        Answer, Environment.NewLine,
                                        SelectedCorrectiveAction == string.Empty
                                            ? HACCPUtil.GetResourceString("None")
                                            : SelectedCorrectiveAction)))
                        {
                            SaveAnswer();
                        }
                    }
                    else
                    {
                        if (RecordResponse.QuestionType == (short) QuestionType.NumericAnswer)
                        {
                            try
                            {
                                var answerVal = HACCPUtil.ConvertToDouble(Answer);
                                Answer = answerVal.ToString();
                            }
                            catch
                            {
                                  Page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("Specifyavalidanswer"));
                                return;
                            }
                        }

                        if (
                            await
                                Page.ShowConfirmAlert(HACCPUtil.GetResourceString("RecordAnswer"),
                                    string.Format(
                                        HACCPUtil.GetResourceString("RecordedAnswer01CorrectiveActiontaken2"), Answer,
                                        Environment.NewLine,
                                        SelectedCorrectiveAction == string.Empty
                                            ? HACCPUtil.GetResourceString("None")
                                            : SelectedCorrectiveAction)))
                        {
                            SaveAnswer();
                        }
                    }
                }
                else
                {
                    if (RecordResponse.QuestionType == (short) QuestionType.NumericAnswer)
                    {
                        try
                        {
                            var answerVal = HACCPUtil.ConvertToDouble(Answer);
                            Answer = answerVal.ToString();
                        }
                        catch
                        {
                              Page.ShowAlert(string.Empty, HACCPUtil.GetResourceString("Specifyavalidanswer"));
                            return;
                        }
                    }
                    if (
                        await
                            Page.ShowConfirmAlert(HACCPUtil.GetResourceString("RecordAnswer"),
                                string.Format(HACCPUtil.GetResourceString("RecordedAnswer01CorrectiveActiontaken2"),
                                    Answer, Environment.NewLine,
                                    SelectedCorrectiveAction == string.Empty
                                        ? HACCPUtil.GetResourceString("None")
                                        : SelectedCorrectiveAction)))
                    {
                        SaveAnswer();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ooops! Something went wrong. Exception: {0}", ex);
            }
            finally
            {
                IsBusy = false;
                SaveCommand.ChangeCanExecute();
            }
        }

        /// <summary>
        ///     Saves the answer.
        /// </summary>
        private async void SaveAnswer()
        {
            var date = DateTime.Now;
            var response = new CheckListResponse
            {
                CategoryId = RecordResponse.CategoryId,
                Question = RecordResponse.QuestionName,
                Answer = Answer,
                CorrAction =
                    SelectedCorrectiveAction != string.Empty
                        ? SelectedCorrectiveAction
                        : HACCPUtil.GetResourceString("None"),
                QuestionType = RecordResponse.QuestionType.ToString(),
                UserName = HaccpAppSettings.SharedInstance.UserName,
                DeviceId = HaccpAppSettings.SharedInstance.DeviceId,
                SiteId = HaccpAppSettings.SharedInstance.SiteSettings.SiteId,
                IsNa = 0,
                Hour = date.Hour.ToString(),
                Day = date.Day.ToString(),
                Minute = date.Minute.ToString(),
                Month = date.Month.ToString(),
                Sec = date.Second.ToString(),
                Year = date.Year.ToString(),
                Tzid = HaccpAppSettings.SharedInstance.SiteSettings.TimeZoneId,
                Min = RecordResponse.Min,
                Max = RecordResponse.Max,
                QuestionId = RecordResponse.QuestionId,
                Catname = RecordResponse.CategoryName,
                ChecklistId = HaccpAppSettings.SharedInstance.SiteSettings.CheckListId
            };

			Debug.WriteLine ("++++++++++++++++++++++++++++++");
			Debug.WriteLine ("***Category Name= {0}***",response.Catname);
			Debug.WriteLine ("++++++++++++++++++++++++++++++");
            dataStore.SaveCheckListResponse(response);
            RecordResponse.RecordStatus = 1;
            dataStore.UpdateQuestionRecordStatus(RecordResponse);
            MessagingCenter.Send(RecordResponse, HaccpConstant.QuestionMessage);
            dataStore.UpdateCategoryRecordStatus(RecordResponse.CategoryId);
            MessagingCenter.Send(new CategoryStatus {CategoryId = RecordResponse.CategoryId},
                HaccpConstant.CategoryMessage);
            IsBusy = false;
            SaveCommand.ChangeCanExecute();
            await Page.PopPage();

            if (HaccpAppSettings.SharedInstance.DeviceSettings.AutoAdvance)
                MessagingCenter.Send(new AutoAdvanceCheckListMessage {CurrentId = RecordResponse.QuestionId},
                    HaccpConstant.AutoadvancechecklistMessage);
        }

        /// <summary>
        ///     Executes the NA command.
        /// </summary>
        /// <returns>The NA command.</returns>
        protected override async Task ExecuteNACommand()
        {
            if (IsBusy)
                return;

            var result =
                await
                    Page.ShowConfirmAlert(string.Empty, HACCPUtil.GetResourceString("SavedNAastheAnswerfortheQuestion"));

            if (result)
            {
                try
                {
                    IsBusy = true;
                    NACommand.ChangeCanExecute();
                    var response = new CheckListResponse();
                    var date = DateTime.Now;
                    response.CategoryId = RecordResponse.CategoryId;
                    response.Question = RecordResponse.QuestionName;
                    response.QuestionType = RecordResponse.QuestionType.ToString();
                    response.UserName = HaccpAppSettings.SharedInstance.UserName;
                    response.DeviceId = HaccpAppSettings.SharedInstance.DeviceId;
                    response.SiteId = HaccpAppSettings.SharedInstance.SiteSettings.SiteId;
                    response.IsNa = 1;
                    response.Hour = date.Hour.ToString();
                    response.Day = date.Day.ToString();
                    response.Minute = date.Minute.ToString();
                    response.Month = date.Month.ToString();
                    response.Sec = date.Second.ToString();
                    response.Year = date.Year.ToString();
                    response.Tzid = HaccpAppSettings.SharedInstance.SiteSettings.TimeZoneId;
                    response.Answer = "0";
                    response.Min = RecordResponse.Min;
                    response.Max = RecordResponse.Max;
                    response.QuestionId = RecordResponse.QuestionId;
                    response.Catname = RecordResponse.CategoryName;
                    response.ChecklistId = HaccpAppSettings.SharedInstance.SiteSettings.CheckListId;
                    response.CorrAction = string.Empty;
                    dataStore.SaveCheckListResponse(response);

                    RecordResponse.RecordStatus = 1;
                    dataStore.UpdateQuestionRecordStatus(RecordResponse);
                    MessagingCenter.Send(RecordResponse, HaccpConstant.QuestionMessage);
                    dataStore.UpdateCategoryRecordStatus(RecordResponse.CategoryId);
                    MessagingCenter.Send(new CategoryStatus {CategoryId = RecordResponse.CategoryId},
                        HaccpConstant.CategoryMessage);

                    IsBusy = false;
                    NACommand.ChangeCanExecute();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Ooops! Something went wrong. Exception: {0}", ex);
                }
                finally
                {
                    IsBusy = false;
                    NACommand.ChangeCanExecute();
                     Page.PopPage();
                    if (HaccpAppSettings.SharedInstance.DeviceSettings.AutoAdvance)
                        MessagingCenter.Send(new AutoAdvanceCheckListMessage {CurrentId = RecordResponse.QuestionId},
                            HaccpConstant.AutoadvancechecklistMessage);
                }
            }
        }

        /// <summary>
        ///     Executes the toggle yes no image command.
        /// </summary>
        /// <returns>The toggle yes no image command.</returns>
        protected void ExecuteToggleYesNoImageCommand()
        {
            if (IsBusy)
                return;

            if (IsNumeric )
            {
                MessagingCenter.Send(string.Empty, HaccpConstant.RecordAnswerFocus);
                return;
            }

            try
            {
                IsBusy = true;
                //	ToggleYesNoImageCommand.ChangeCanExecute ();


                if (YesNoButtonText == HACCPUtil.GetResourceString("No"))
                {
                    YesNoButtonText = HACCPUtil.GetResourceString("Yes");
                    Answer = HACCPUtil.GetResourceString("No");

                    //SelectedCorrectiveAction = AppResources.None;
                }
                else
                {
                    YesNoButtonText = HACCPUtil.GetResourceString("No");
                    Answer = HACCPUtil.GetResourceString("Yes");
                    SelectedCorrectiveAction = HACCPUtil.GetResourceString("None");
                    ExecuteCorrectiveActionCommand();
                }

                IsBusy = false;
                //	ToggleYesNoImageCommand.ChangeCanExecute ();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ooops! Something went wrong. Exception: {0}", ex);
            }
            finally
            {
                IsBusy = false;
                //	ToggleYesNoImageCommand.ChangeCanExecute ();
            }
        }

        #endregion
    }
}