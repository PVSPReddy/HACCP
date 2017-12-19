namespace HACCP.Core
{
    /// <summary>
    ///     Page enum.
    /// </summary>
    public enum PageEnum
    {
        Home = 1,
        WirelessTask = 2,
        ServerSettings = 3,
        SelectUser = 4,
        PerformCheckList = 5,
        SelectQuestion = 6,
        SelectLocations = 7,
        SelectLocationItem = 8,
        RecordAnswer = 9,
        RecordItem = 10,
        RecordComplete = 11,
        ServerSettingsConfirmation = 12,
        Blue2Settings = 13,
        MenuChecklist = 14,
        ViewStatus = 15,
        ThermometerMode = 16,
        ClearCheckmarks = 17,
        Temperaturereview = 18,
        CategoryReview = 19,
        ViewStatusLines = 20
    }

    public enum WebServiceEnum
    {
        ActiveSiteDetails = 1,
        DeviceSettings = 2,
        HandheldUsers = 3,
        ListMenuName = 4,
        MenuDetails = 5,
        CorrActions = 6,
        ListCheckListNames = 7,
        ChecklistDetails = 8,
        OpenBatch = 9,
        UploadTemperatures = 10,
        UploadChecklists = 11,
        CloseBatch = 12,
        LanguageList = 13,
        LanguageDictionary = 14
    }

    public enum QuestionType
    {
        NumericAnswer = 1,
        YesOrNo = 2
    }

    public enum RecordStatusTypes
    {
        NotCompleted = 0,
        Completed = 1,
        PartialCompleted = 2
    }

    public enum TemperatureUnit
    {
        Celcius = 0,
        Fahrenheit = 1
    }

    public enum ButtonType
    {
        YesNo = 0,
        OkCancel = 1
    }

    public enum TimeFormat
    {
        TwelveHour = 0,
        TwentyFourHour = 1
    }

    public enum DateFormat
    {
        M_d_yyyy = 0,
        yyyy_MM_dd = 1,
        yy_MM_dd = 2,
        dd_MMMM_yy = 3
    }

    public enum TemperatureScale
    {
        Fahrenheit = 0,
        Celsius = 1
    }

    public enum RequiredPin
    {
        NotReqired = 0,
        Required = 1
    }

    public enum AllowManualTemps
    {
        NotAllow = 0,
        Allow = 1
    }

    public enum Languages
    {
        English = 1,
        Spanish = 2,
        Dutch = 3,
        Prortughese = 4,
        Portughese_Br = 5,
        French = 6,
        Chinese = 7,
        Chinese_Std = 8,
        Chinese_Smpl = 9
    }

    public enum RecordingMode
    {
        BlueTooth = 0,
        Manual = 1
    }

    public enum BLEChar
    {
        Scale = 0,
        Time = 1,
        AutoOff = 2,
        Sleep = 3,
        Prob = 4
    }
}