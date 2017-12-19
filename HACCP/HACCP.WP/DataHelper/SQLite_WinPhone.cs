using System.IO;
using Windows.Storage;
using HACCP.Core;
using HACCP.WP.DataHelper;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLite_WinPhone))]

namespace HACCP.WP.DataHelper
{
    // ...
    public class SQLite_WinPhone : ISQLite
    {
        public SQLiteConnection GetConnection()
        {
            var sqliteFilename = "HACCPSQLite.db3";
            var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, sqliteFilename);
            // Create the connection
            var conn = new SQLiteConnection(new SQLitePlatformWinRT(), path);
            // Return the database connection


            return conn;
        }
    }
}