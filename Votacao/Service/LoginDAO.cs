using Dapper;
using System;
using System.Linq;
using Votacao.Data;
using Votacao.Data.Model;
using Votacao.Service.DTO;
using Votacao.Service.Interface;

namespace Votacao
{
    public class LoginDAO : ILogin
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Public Methods

        public Login Login(string login, string password)
        {
            try
            {
                User userBD = GetUserByLogin(login);

                Data.Model.Election electionBD = GetElection();

                if (userBD != null
                    && BCrypt.Net.BCrypt.Verify(password, System.Text.Encoding.UTF8.GetString(userBD.AccessToken)))
                {
                    return new Login()
                    {
                        Status = StatusTransaction.OK,
                        NameUser = userBD.Name,
                        Token = string.Empty,
                        Election = electionBD.Id.ToString()
                    };
                }

                return new Login()
                {
                    Status = StatusTransaction.ACCESSDENIED
                };
            }
            catch (Exception ex) 
            {
                log.Error("Error login", ex);

                return new Login()
                {
                    Status = StatusTransaction.ACCESSDENIED
                };
            }
        }

        #endregion

        #region Private Methods

        private User GetUserByLogin(string login) 
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                ////cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                cnn.Open();
                return cnn.Query<User>(@"SELECT id Id, 
                                                login Login, 
                                                name Name, 
                                                accessToken AccessToken 
                                        FROM    user 
                                        WHERE   login = @login",
                                        new { login }).FirstOrDefault();
            }
        }

        private Data.Model.Election GetElection()
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                ////cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                cnn.Open();
                return cnn.Query<Data.Model.Election>(@"SELECT  id Id, 
                                                                name Name, 
                                                                voting Voting, 
                                                                votesPerVoter VotesPerVoter 
                                                        FROM    election").FirstOrDefault();
            }
        }

        #endregion
    }
}