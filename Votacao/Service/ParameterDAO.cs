using Dapper;
using System.Data.SQLite;
using System.Linq;
using Votacao.Data;
using Votacao.Service.Interface;

namespace Votacao.Service
{
    public class ParameterDAO: IParameter
    {
        #region Public Methods

        public void Add(string key, string value)
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                ////cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                cnn.Open();
                cnn.Execute(@"  INSERT INTO parameter (key, value) 
                                VALUES (@key, @value)", 
                                new
                                {
                                    key,
                                    value
                                });
            }
        }

        public void Update(string key, string value)
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                //cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                cnn.Open();
                cnn.Execute(@"  UPDATE  parameter 
                                SET     value = @value 
                                WHERE   key = @key",
                                new
                                {
                                    key,
                                    value
                                });
            }
        }

        public string Get(string key)
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                //SetConnection(cnn);

                return cnn.Query<string>(@" SELECT  value 
                                            FROM    parameter 
                                            WHERE   key = @key",
                                            new { key }).FirstOrDefault();
            }
        }
        public  void SetConnection(SQLiteConnection cnn) {

            if(Properties.Settings.Default.ChangedPasswordData)
                //cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                       
            cnn.Open();

            if (!Properties.Settings.Default.ChangedPasswordData)
            {
                cnn.ChangePassword(ParametersSingleton.Instance.PasswordSqlite);
                Properties.Settings.Default.ChangedPasswordData = true;
                Properties.Settings.Default.Save();
            }               

        }

        public bool Exists(string key)
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                ////cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                cnn.Open();
                return cnn.Query<string>(@" SELECT  value 
                                            FROM    parameter 
                                            WHERE   key = @key",
                                            new { key }).Any();
            }
        }

        #endregion

    }
}
