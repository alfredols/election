using Dapper;
using System.Collections.Generic;
using System.Linq;
using Votacao.Data;
using Votacao.Service.DTO;
using Votacao.Service.Interface;

namespace Votacao.Service
{
    public class VoteDAO : IVote
    {

        #region Public Methods

        public void Save(int? candidateId, string nicknameStation)
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                //cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                cnn.Open();
                cnn.Execute(@"INSERT INTO vote (candidate_id, nickname_station, created_on) VALUES (@candidateId, @nicknameStation, datetime('now'))",
                    new
                    {
                        candidateId,
                        nicknameStation
                    });
            }
        }

        public List<Vote> ListVote()
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                //cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                cnn.Open();
                return cnn.Query<Vote>(@"SELECT id Id, 
                                                candidate_id CandidateId
                                        FROM    vote").ToList();
            }
        }

        #endregion

    }
}
