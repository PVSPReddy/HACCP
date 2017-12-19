using SQLite.Net;

namespace HACCP.Core
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection();
    }
}