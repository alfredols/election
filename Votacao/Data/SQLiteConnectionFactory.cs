using System.Data.SQLite;

namespace Votacao.Data
{
    public class SQLiteConnectionFactory
    {
        public static SQLiteConnection Create()
        {
            return new SQLiteConnection(@"Data Source=data\station.dat");
        }
    }
}
