//using System;
//
//namespace HACCP.iOS
//{
//	public class SQLite_iOS
//	{
//		public SQLite_iOS ()
//		{
//		}
//	}
//}


using System;
using System.IO;
using HACCP.Core;
using HACCP.iOS;
using SQLite.Net;
using SQLite.Net.Platform.XamarinIOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLite_iOS))]

namespace HACCP.iOS
{
    public class SQLite_iOS : ISQLite
    {
        public SQLiteConnection GetConnection()
        {
            var sqliteFilename = "HACCPSQLite.db3";
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            var libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder
            var path = Path.Combine(libraryPath, sqliteFilename);
            // Create the connection
            var conn = new SQLiteConnection(new SQLitePlatformIOS(), path);
            // Return the database connection
            return conn;
        }
    }
}