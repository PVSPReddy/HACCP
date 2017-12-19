using SQLite.Net.Attributes;

namespace HACCP.Core
{
    public class UploadStatus
    {
        [PrimaryKey]
        public int BcatchNumber { get; set; }

        public bool IsOpen { get; set; }
        public bool IsClose { get; set; }
        public int LastRecordNumberCheck { get; set; }
        public int LastRecordNumberTemp { get; set; }
    }
}