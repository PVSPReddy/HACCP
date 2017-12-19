using SQLite.Net.Attributes;

namespace HACCP.Core
{
    public class ItemTemperature
    {
        [PrimaryKey, AutoIncrement]
        public int RecordNo { get; set; }

        public string ItemName { get; set; }
        public string Temperature { get; set; }
        public string Month { get; set; }
        public string Day { get; set; }
        public string Year { get; set; }
        public string Hour { get; set; }
        public string Minute { get; set; }
        public string Sec { get; set; }
        public string UserName { get; set; }
        public string CorrAction { get; set; }
        public string Min { get; set; }
        public string Max { get; set; }
        public string LocName { get; set; }
        public string Ccp { get; set; }
        public string TZID { get; set; }
        public string DeviceId { get; set; }
        public long BatchId { get; set; }
        public long SiteID { get; set; }
        public long ItemID { get; set; }
        public long LocationID { get; set; }
        public long CCPID { get; set; }
        public long MenuID { get; set; }
        public int IsNA { get; set; }
        public short IsManualEntry { get; set; }
        public string Blue2ID { get; set; }
        public string Note { get; set; }
    }
}