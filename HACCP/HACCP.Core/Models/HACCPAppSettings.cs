using System;
using SQLite.Net.Attributes;
using Xamarin.Forms;

namespace HACCP.Core
{
    public class HaccpAppSettings
    {
        private static volatile HaccpAppSettings _instance;

        private bool _checkPendingRecords;

        private long _currentLanguageId;


        private long _currentUserId = -1;

        private HaccpAppSettings()
        {
        }

        public static HaccpAppSettings SharedInstance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new HaccpAppSettings
                    {
                        SiteSettings = new SiteSettings {ServerDirectory = HaccpConstant.DefultServerDirectyory}
                    };
                    _instance.DeviceSettings = new DeviceSettings();
                    _instance.DeviceId = GetDeviceId(false);
                    _instance.LanguageId = Settings.CurrentLanguageID;
                }
                return _instance;
            }
        }

        public string DeviceId { get; set; }

        [Ignore]
        public bool IsWindows { get; set; }

        [Ignore]
        public bool IsUploadingProgress { get; set; }

        [Ignore]
        public bool AppResuming { get; set; }

		[Ignore]
		public bool IsLanguageChanged { get; set; }

        [Ignore]
        public bool CheckPendingRecords
        {
            get { return _checkPendingRecords; }
            set
            {
                if (_checkPendingRecords != value)
                {
                    _checkPendingRecords = value;
                    if (value)
                    {
                        MessagingCenter.Send(this, HaccpConstant.MsCheckpending);
                    }
                }
            }
        }

        public long LanguageId
        {
            get { return _currentLanguageId; }
            set
            {
                if (_currentLanguageId != value)
                {
                    _currentLanguageId = value;
                    //if(currentLanguageId != 0)
                    //Settings.CurrentLanguageID = currentLanguageId;
                }
            }
        }

        [Ignore]
        public long CurrentUserId
        {
            get { return _currentUserId; }
            set
            {
                if (_currentUserId != value)
                {
                    _currentUserId = value;

                    MessagingCenter.Send(this, HaccpConstant.MsLoginChanged);
                }
            }
        }

        [Ignore]
        public string UserName { get; set; }

        [Ignore]
        public string Permission { get; set; }

        [Ignore]
        public string ResourceString { get; set; }

        public DeviceSettings DeviceSettings { get; private set; }

        public SiteSettings SiteSettings { get; private set; }

        public void ResetDeviceId()
        {
            DeviceId = GetDeviceId(true);
        }

        private static string GetDeviceId(bool shouldReset)
        {
            var deviceId = string.Empty;
            if (shouldReset == false)
                deviceId = Settings.DeviceID;
            if (string.IsNullOrEmpty(deviceId))
            {
                var secs = (DateTime.Now - new DateTime(2010, 01, 01)).TotalSeconds;
                var decValue = Convert.ToInt32(secs);
                var hexValue = decValue.ToString("X8");
                    // convert to string with 8 hexadecimal letters also left padded with zeros
                deviceId = hexValue.Insert(6, "-").Insert(3, "-"); // insert '-' after third and sixth charecters

                Settings.DeviceID = deviceId;
            }

            return deviceId;
        }
    }

    public class DeviceSettings
    {
        public bool AutoAdvance { get; set; }
        public bool SkipRecordPreview { get; set; }
        public short AutoOffTime { get; set; }
        public short AllowManualTemp { get; set; }
        public short DateFormat { get; set; }
        public short TimeFormat { get; set; }
        public short TempScale { get; set; }
        public short RequirePin { get; set; }
        public short AllowTextMemo { get; set; }
        public string CustomProbDescription { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
    }

    public class SiteSettings
    {
        public string ServerAddress { get; set; }
        public string ServerPort { get; set; }
        public string ServerDirectory { get; set; }
        public long SiteId { get; set; }
        public string SiteName { get; set; }
        public long MenuId { get; set; }
        public string MenuName { get; set; }
        public long CheckListId { get; set; }
        public string CheckListName { get; set; }
        public string TimeZoneId { get; set; }
        public long LastBatchNumber { get; set; }
        public string LastUploaded { get; set; }
    }
}