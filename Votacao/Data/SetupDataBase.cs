using Dapper;

namespace Votacao.Data
{
    public class SetupDatabase
    {
        public void Init() 
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                cnn.Open();
                cnn.Execute(System.IO.File.ReadAllText(@"data\setupSQLite.sql"));
            }
        }
    }
}
