using System;
using System.IO;
using HACCP.Core;
using HACCP.Droid;
using SQLite.Net;
using SQLite.Net.Platform.XamarinAndroid;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLite_Android))]

namespace HACCP.Droid
{
    // ...
    public class SQLite_Android : ISQLite
    {
        public SQLiteConnection GetConnection()
        {
            var sqliteFilename = "HACCP.db3";
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            var path = Path.Combine(documentsPath, sqliteFilename);
            // Create the connection
            var conn = new SQLiteConnection(new SQLitePlatformAndroid(), path);
            // Return the database connection
            return conn;
        }
    }
}