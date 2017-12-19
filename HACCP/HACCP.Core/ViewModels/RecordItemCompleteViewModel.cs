using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HACCP.Core
{
    public class RecordItemCompleteViewModel : RecordViewModel
    {
        #region Methods

        private readonly IDataStore dataStore;
        private readonly double TemperatureValue;
        private string correctiveActionText = string.Format("{0}: {1}", HACCPUtil.GetResourceString("CorrectiveAction"),
            "None");
        private bool isCorrectiveOptionsVisible;
        private ItemTemperature itemTemperature;
        private string manualEntryTemperature;
        private CorrectiveAction selectedCorrectiveAction;
        private LocationMenuItem selectedItem;
        private string temperature = "0";
        private TemperatureUnit temperatureUnit;

        #endregion


        /// <summary>
        ///     Initializes a new instance of the <see cref="HACCP.Core.RecordItemCompleteViewModel" /> class.
        /// </summary>
        /// <param name="page">Page.</param>
        /// <param name="item">Item.</param>
        public RecordItemCompleteViewModel(IPage page, object item) : base(page)
        {
            dataStore = new SQLiteDataStore();
            SelectedItem = item as LocationMenuItem;
            if (SelectedItem != null)
            {
                NoteText = SelectedItem.Note;
                TemperatureUnit = SelectedItem.TemperatureUnit;
                TemperatureValue = HACCPUtil.ConvertToDouble(SelectedItem.RecordedTemperature);
                Temperature = TemperatureValue.ToString("0.0");
                itemTemperature = dataStore.GetItemTemperature(SelectedItem);
            }
            LoadCorrectiveActions();
        }

        #region Properties

        /// <summary>
        ///     Gets or sets the temperature unit.
        /// </summary>
        /// <value>The temperature unit.</value>
        public TemperatureUnit TemperatureUnit
        {
            get { return temperatureUnit; }
            set
            {
                temperatureUnit = value;
                UnitString = value == TemperatureUnit.Fahrenheit ? "°F" : "°C";
            }
        }

        /// <summary>
        ///     Gets or sets the unit string.
        /// </summary>
        /// <value>The unit string.</value>
        public string UnitString { get; set; }

        /// <summary>
        ///     Gets or sets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public LocationMenuItem SelectedItem
        {
            get { return selectedItem; }
            set { SetProperty(ref selectedItem, value); }
        }


        /// <summary>
        ///     Gets or sets the minimum temperature.
        /// </summary>
        /// <value>The minimum temperature.</value>
        public string MinimumTemperature
        {
            get
            {
                var min = HACCPUtil.ConvertToDouble(SelectedItem.Min);
                if (TemperatureUnit == TemperatureUnit.Celcius)
                    min = Math.Round(HACCPUtil.ConvertFahrenheitToCelsius(min));
                return string.Format("{0}{1}", min, UnitString);
            }
        }

        /// <summary>
        ///     Gets or sets the maximum temperature.
        /// </summary>
        /// <value>The maximum temperature.</value>
        public string MaximumTemperature
        {
            get
            {
                var max = HACCPUtil.ConvertToDouble(SelectedItem.Max);
                if (TemperatureUnit == TemperatureUnit.Celcius)
                    max = Math.Round(HACCPUtil.ConvertFahrenheitToCelsius(max));
                return string.Format("{0}{1}", max, UnitString);
            }
        }

        /// <summary>
        ///     Gets or sets the blue temperature.
        /// </summary>
        /// <value>The blue temperature.</value>
        public string Temperature
        {
            get { return string.Format("{0}{1}", temperature, UnitString); }
            set { SetProperty(ref temperature, value); }
        }


        /// <summary>
        ///     Gets or sets the manual entry temperature.
        /// </summary>
        /// <value>The manual entry temperature.</value>
        public string ManualEntryTemperature
        {
            get
            {
                return manualEntryTemperature;
                //return string.Format ("{0}.0°F", manualEntryTemperature);
            }
            set { SetProperty(ref manualEntryTemperature, value); }
        }


        /// <summary>
        ///     Gets or sets my property.
        /// </summary>
        /// <value>My property.</value>
        public CorrectiveAction SelectedCorrectiveAction
        {
            get { return selectedCorrectiveAction; }
            set
            {
                if (value != null)
                {
                    SetProperty(ref selectedCorrectiveAction, value);
                    IsCorrectiveOptionsVisible = false;

                    CorrectiveActionText = string.Format("{0}: {1}", HACCPUtil.GetResourceString("CorrectiveAction"),
                        value.CorrActionName);
                }
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is corrctive options visible.
        /// </summary>
        /// <value><c>true</c> if this instance is corrctive options visible; otherwise, <c>false</c>.</value>
        public bool IsCorrectiveOptionsVisible
        {
            get { return isCorrectiveOptionsVisible; }
            set { SetProperty(ref isCorrectiveOptionsVisible, value); }
        }


        /// <summary>
        ///     Gets or sets the corrective action text.
        /// </summary>
        /// <value>The corrective action text.</value>
        public string CorrectiveActionText
        {
            get { return correctiveActionText; }
            set { SetProperty(ref correctiveActionText, value); }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is note enable.
        /// </summary>
        /// <value><c>true</c> if this instance is note enable; otherwise, <c>false</c>.</value>
        public bool IsNoteEnable
        {
            get { return HaccpAppSettings.SharedInstance.DeviceSettings.AllowTextMemo == 1 ? true : false; }
        }

        /// <summary>
        ///     Gets the note icon.
        /// </summary>
        /// <value>The note icon.</value>
        public string NoteIcon
        {
            get
            {
                return HaccpAppSettings.SharedInstance.DeviceSettings.AllowTextMemo == 1
                    ? "edit.png"
                    : "noteDisabled.png";
            }
        }

        /// <summary>
        ///     Gets the color of the note hint.
        /// </summary>
        /// <value>The color of the note hint.</value>
        public Color NoteHintColor
        {
            get
            {
                return HaccpAppSettings.SharedInstance.DeviceSettings.AllowTextMemo == 1
                    ? Color.White
                    : Color.FromHex("#a2b9c1");
            }
        }


        private string noteText;

        /// <summary>
        ///     Gets or sets the note text.
        /// </summary>
        /// <value>The note text.</value>
        public string NoteText
        {
            get { return noteText; }
            set { SetProperty(ref noteText, value); }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Executes the corrective action command.
        /// </summary>
        /// <returns>The corrective action command.</returns>
        protected override void ExecuteCorrectiveActionCommand()
        {
            IsCorrectiveOptionsVisible = true;
        }


        /// <summary>
        ///     Loads the corrective actions.
        /// </summary>
        public void LoadCorrectiveActions()
        {
            var minTemp = HACCPUtil.ConvertToDouble(SelectedItem.Min);
            var maxTemp = HACCPUtil.ConvertToDouble(SelectedItem.Max);

            var temp = TemperatureValue;
            if (TemperatureUnit == TemperatureUnit.Celcius)
            {
                minTemp = Math.Round(HACCPUtil.ConvertFahrenheitToCelsius(minTemp));
                maxTemp = Math.Round(HACCPUtil.ConvertFahrenheitToCelsius(maxTemp));
                //temp = HACCPUtil.ConvertCelsiusToFahrenheit (TemperatureValue);
            }

            var actions = dataStore.GetCorrectiveAction().ToList();


//			foreach (var action in actions) {
//				if (action.CorrActionName.ToLower () == "none")
//					action.IsEnabled = temp >= minTemp && temp <= maxTemp;
//				else
//					action.IsEnabled = true;
//			}

            CorrectiveActions = new ObservableCollection<CorrectiveAction>(actions);


            foreach (var action in CorrectiveActions)
            {
                switch (action.CorrActionName)
                {
                    case "Notify Manager":
                        action.CorrActionName = HACCPUtil.GetResourceString("NotifyManager");
                        break;
                    case "Reheat to 165F":
                        action.CorrActionName = HACCPUtil.GetResourceString("Reheatto165F");
                        break;
                    case "Rechill":
                        action.CorrActionName = HACCPUtil.GetResourceString("Rechill");
                        break;
                    case "Discard":
                        action.CorrActionName = HACCPUtil.GetResourceString("Discard");
                        break;
                }
            }

            if (temp >= minTemp && temp <= maxTemp)
                CorrectiveActions.Insert(0, new CorrectiveAction
                {
                    CorrActionId = -1,
                    CorrActionName = HACCPUtil.GetResourceString("None")
                });

            IsCorrectiveOptionsVisible = temp < minTemp || temp > maxTemp;
        }

        /// <summary>
        ///     Executes the save command.
        /// </summary>
        /// <returns>The save command.</returns>
        protected override async Task ExecuteSaveCommand()
        {
            try
            {
                IsBusy = true;
                SaveCommand.ChangeCanExecute();

                HACCPUtil.ConvertToDouble(SelectedItem.Min);
                HACCPUtil.ConvertToDouble(SelectedItem.Max);

                var temp = TemperatureValue;
                if (TemperatureUnit == TemperatureUnit.Celcius)
                    temp = HACCPUtil.ConvertCelsiusToFahrenheit(TemperatureValue);


                if (
                    await
                        Page.ShowConfirmAlert(HACCPUtil.GetResourceString("SaveRecord"),
                            string.Format(
                                HACCPUtil.GetResourceString("RecordedTemperature0Correctiveactiontaken1User2"),
                                Temperature + HaccpConstant.NewlineCharacter,
                                SelectedCorrectiveAction != null
                                    ? SelectedCorrectiveAction.CorrActionName + HaccpConstant.NewlineCharacter
                                    : HACCPUtil.GetResourceString("None") + HaccpConstant.NewlineCharacter,
                                HaccpAppSettings.SharedInstance.UserName)))
                {
                    //if (itemTemperature == null) {					
                    var date = DateTime.Now;
                    long ccpid;
                    long.TryParse(SelectedItem.Ccpid, out ccpid);
                    var location = dataStore.GetLocationById(SelectedItem.LocationId);

                    itemTemperature = new ItemTemperature
                    {
                        IsManualEntry = SelectedItem.IsManualEntry ? (short) 1 : (short) 0,
                        ItemID = SelectedItem.ItemId,
                        Temperature = temp.ToString("0.0"),
                        ItemName = SelectedItem.Name,
                        Max = SelectedItem.Max,
                        Min = SelectedItem.Min,
                        CorrAction =
                            SelectedCorrectiveAction != null
                                ? SelectedCorrectiveAction.CorrActionName
                                : HACCPUtil.GetResourceString("None"),
                        LocationID = SelectedItem.LocationId,
                        Ccp = SelectedItem.Ccp,
                        CCPID = ccpid,
                        IsNA = 0,
                        Hour = date.Hour.ToString(),
                        Day = date.Day.ToString(),
                        Minute = date.Minute.ToString(),
                        Month = date.Month.ToString(),
                        Sec = date.Second.ToString(),
                        Year = date.Year.ToString(),
                        SiteID = HaccpAppSettings.SharedInstance.SiteSettings.SiteId,
                        UserName = HaccpAppSettings.SharedInstance.UserName,
                        DeviceId = HaccpAppSettings.SharedInstance.DeviceId,
                        MenuID = HaccpAppSettings.SharedInstance.SiteSettings.MenuId,
                        TZID = HaccpAppSettings.SharedInstance.SiteSettings.TimeZoneId,
                        //BatchId = HACCPAppSettings.SharedInstance.SiteSettings.LastBatchNumber,
                        LocName = location.Name,
                        Note =
                            !string.IsNullOrEmpty(NoteText) && !string.IsNullOrWhiteSpace(NoteText)
                                ? NoteText.Trim()
                                : string.Empty,
                        Blue2ID = SelectedItem.Blue2Id
                    };
                    dataStore.AddTemperature(itemTemperature);

                    SelectedItem.RecordStatus = 1;
                    dataStore.RecordLocationItem(SelectedItem);

                    MessagingCenter.Send(SelectedItem, HaccpConstant.RecorditemMessage);
                    dataStore.UpdateLocationItemRecordStatus(SelectedItem.LocationId);
                    MessagingCenter.Send(new MenuLocationId
                    {
                        LocationId = SelectedItem.LocationId
                    }, HaccpConstant.MenulocationMessage);

                    if (HaccpAppSettings.SharedInstance.IsWindows)
                    {
                        await Page.PopPages(2);
                    }
                    else
                    {
                        Page.RemoveRecordItemPage();
                        await Page.PopPage();
                    }

                    if (HaccpAppSettings.SharedInstance.DeviceSettings.AutoAdvance)
                        MessagingCenter.Send(new AutoAdvanceLocationMessage
                        {
                            CurrentId = SelectedItem.ItemId
                        }, HaccpConstant.AutoadvancelocationMessage);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error occured while saving temperature {0}", ex.Message);
            }
            finally
            {
                IsBusy = false;
                SaveCommand.ChangeCanExecute();
            }
        }

        #endregion
    }
}