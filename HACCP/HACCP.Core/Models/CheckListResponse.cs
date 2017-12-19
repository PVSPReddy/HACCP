using SQLite.Net.Attributes;

namespace HACCP.Core
{
    public class CheckListResponse
    {
        [PrimaryKey, AutoIncrement]
        public int RecordNo { get; set; }

        public string Catname { get; set; }
        public string Question { get; set; }
        public string Month { get; set; }
        public string Day { get; set; }
        public string Year { get; set; }
        public string Hour { get; set; }
        public string Minute { get; set; }
        public string Sec { get; set; }
        public string UserName { get; set; }
        public string QuestionType { get; set; }
        public string Min { get; set; }
        public string Max { get; set; }
        public string CorrAction { get; set; }
        public string Answer { get; set; }
        public string Tzid { get; set; }
        public string DeviceId { get; set; }
        public long QuestionId { get; set; }
        public long BatchId { get; set; }
        public long SiteId { get; set; }
        public long ChecklistId { get; set; }
        public long CategoryId { get; set; }
        public int IsNa { get; set; }
    }
}