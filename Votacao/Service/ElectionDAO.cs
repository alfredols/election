using Dapper;
using System;
using System.Data.SQLite;
using System.Linq;
using Votacao.Data;
using Votacao.Service.DTO;
using Votacao.Service.Interface;

namespace Votacao
{
    public class ElectionDAO : IElection
    {
        #region Attributes

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Public Methods

        public string GetName(string electionId)
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                //cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                cnn.Open();
                return cnn.Query<string>(@" SELECT  name
                                            FROM    election 
                                            WHERE   id = @electionId",
                                            new { electionId }).FirstOrDefault();
            }
        }

        public TypeVoting GetVoting(string electionId)
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                //cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                cnn.Open();

                if (String.IsNullOrEmpty(electionId))
                {
                    electionId = GetElectionId(cnn);
                }

                int id = Convert.ToInt32(electionId);
                TypeVotingStringConverter converter = new TypeVotingStringConverter();

                return converter.ConvertStringToTypeVoting(
                                    cnn.Query<string>(@"SELECT  voting
                                                        FROM    election 
                                                        WHERE   id = @id",
                                                        new { id }).FirstOrDefault());
            }
        }

        public int GetVotesPerVoter(string electionId) 
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                //cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                cnn.Open();

                if (String.IsNullOrEmpty(electionId))
                {
                    electionId = GetElectionId(cnn);
                }

                int id = Convert.ToInt32(electionId);

                return cnn.Query<int>(@"SELECT  votesPerVoter
                                        FROM    election 
                                        WHERE   id = @id",
                                        new { id }).FirstOrDefault();
            }
        }

        public DateTime GetElectionDateTimeStart(string electionId)
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                //cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                cnn.Open();

                if (String.IsNullOrEmpty(electionId))
                {
                    electionId = GetElectionId(cnn);
                }

                int id = Convert.ToInt32(electionId);

                return cnn.Query<DateTime>(@"   SELECT  start
                                                FROM    election 
                                                WHERE   id = @id",
                                                new { id }).FirstOrDefault();
            }
        }

        public DateTime GetElectionDateTimeEnd(string electionId)
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                //cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                cnn.Open();

                if (String.IsNullOrEmpty(electionId))
                {
                    electionId = GetElectionId(cnn);
                }

                int id = Convert.ToInt32(electionId);

                return cnn.Query<DateTime>(@"   SELECT  [end]
                                                FROM    election 
                                                WHERE   id = @id",
                                                new { id }).FirstOrDefault();
            }
        }

        #endregion

        #region Private Methods

        private string GetElectionId(SQLiteConnection cnn)
        {
            return cnn.Query<string>(@"SELECT  id
                                       FROM    election").FirstOrDefault();
        }

        #endregion

    }
}