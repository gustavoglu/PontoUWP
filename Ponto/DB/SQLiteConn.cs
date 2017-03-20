using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ponto.DB
{
    public class SQLiteConn
    {
        public const string dbaseName = "pontodb";

        public static SQLiteConnection Conn()
        {
            string patch = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path,dbaseName);

            return new SQLiteConnection(new SQLitePlatformWinRT(),patch);

        }
    }
}
