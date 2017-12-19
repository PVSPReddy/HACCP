using SQLite.Net.Attributes;

namespace HACCP.Core
{
    public class CorrectiveAction
    {
        [PrimaryKey, AutoIncrement]
        public long CorrActionId { get; set; }

        public long QuestionId { get; set; }
        public string CorrActionName { get; set; }
    }
}