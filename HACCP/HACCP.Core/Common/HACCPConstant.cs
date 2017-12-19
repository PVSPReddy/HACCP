namespace HACCP.Core
{
    public static class HaccpConstant
    {
        //Message Center Notiificaiotn keys
        public const string MsLoginChanged = "MS_LoginChanged";
        public const string MenulocationMessage = "MenuLocation";
        public const string RecorditemMessage = "RecordItem";
        public const string CategoryMessage = "Category";
        public const string QuestionMessage = "Question";
        public const string AutoadvancelocationMessage = "AutoAdvanceLocation";
        public const string AutoadvancechecklistMessage = "AutoAdvanceChecklist";
        public const string UploadrecordMessage = "Upload Records";
        public const string MsCheckpending = "MS_CheckPending";

        public const string AckCodeSuccess = "200";
        public const int AckCodeDeviceDisabled = 201;
        public const string AckStatusSuccess = "Success";
        public const string YesNoQuestionType = "Yes NO";
        public const string NumericQuestionType = "Numeric";
        public const string UserPermisionElevated = "Elevated";
        public const string UserPermisionStandard = "Standard";
        public const string DefultServerDirectyory = "haccpservice";


        public const string Blue2DeviceName = "blue2";
        public const string TemperatureServiceUuid = "f2b32c77-ea68-464b-9cd7-a22cbffb98bd";
        public const string DeviceServiceUuid = "0000180a-0000-1000-8000-00805f9b34fb";
        //public const string MEASUREMENTTIMING_CHARACTERISTICS_UUID = "4134a840-cbfa-455a-a3a9-6b78cc64dc72";
        public const string MeasurementtimingCharacteristicsUuid = "4134a840-cbfa-455a-a3a9-6b78cc64dc72";

        public const string AutoOffIntervalUuid = "7d53f0b7-33aa-4939-98c6-39fcd1fa9b06";
        public const string MeasurementScaleUuid = "28ef0770-3640-4565-a99b-d07cf6a9103f";
        public const string SleepTimeUuid = "fb441c3a-87fc-477d-9f53-7f1c53f0bb03";
        public const string ProbeUuid = "aa40faaa-8674-4b6f-8bda-f1d2a05fc081";
        public const string ResetAutooffUuid = "67586205-9cde-4b5c-a565-57197e3b7319";
        public const string AsciiTemperatureUuid = "78544003-4394-4fc2-8cfd-be6a00aa701b";
        public const string BatteryServiceUuid = "0000180f-0000-1000-8000-00805f9b34fb";
        public const string DiscoonectPeripheralUuid = "c717cbae-241c-4856-9829-a26351bae626";
        public const string BatteryServiceName = "battery";

        public const string DeviceInformationServiceName = "Device Information";
        public const string SerialNumber = "serial number";
        public const string SerialNumberUuid = "2a25";

        public const string BletemperatureReading = "BLE Temperature Reading";
        public const string BleconnectionStatus = "BLE Connection Status";
        public const string WindowsScanningStatus = "Windows Scanning Status";
        public const string Bleblue2SettingsUpdate = "BLE Blue2Settings Update";
        public const string Blue2SleepStateValue = "sleeping";
        public const string Blue2HighStateValue = "high";
        public const string Blue2LowStateValue = "low";
        public const string Blue2BatteryLowState = "battery";
        public const string Blue2ScanComplete = "scan completed";
        public const string Blue2PlaceholderVisibility = "placeholder visibility";
        public const string ServiceResponseAckcode = "AckCode";
        public const string BleconnectionTimeout = "Connection time out";
        public const string UploadRecordRefresh = "Upload Record Refresh";
        public const string RecordAnswerFocus = "Record Answer Focus";
		public const string UploadRecordProgress = "Upload Record Progress";


        public const string Plusparameter = "Plus";
        public const string C = "C";
        public const string FahrentValue = "F";
        public const string ToastMessage = "Toast";

        public const string CleanupMessage = "cleanup";
        public const string UserPasswordFocusMessage = "user password focus";
        public const string ReviewMessage = "review";
        public const string ChecklistReviewMessage = "checklist review";
        public const string NextPrevMessage = "nexttprev message";

        public const string NewlineCharacter = "\n";

        public const string GetLanguageServiceUrl =
            "http://137.117.92.68/HACCPService/ProcessRequest.svc/HACCPManagerService/LanguageList";

        public const string GetLanguagedictionaryServiceUrl =
            "http://137.117.92.68/HACCPService/ProcessRequest.svc/HACCPManagerService/LanguageDictionary?LanguageID={0}";
    }
}