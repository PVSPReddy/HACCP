using SQLite.Net.Attributes;

namespace HACCP.Core
{
    public class Menu
    {
        [PrimaryKey]
        public long MenuId { get; set; }

        public string Name { get; set; }
    }
}