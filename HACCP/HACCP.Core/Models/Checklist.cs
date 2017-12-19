using SQLite.Net.Attributes;

namespace HACCP.Core
{
    public class Checklist
    {
        [PrimaryKey]
        public long ChecklistId { get; set; }

        public string Name { get; set; }
    }
}