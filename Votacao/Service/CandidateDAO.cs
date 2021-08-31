using Dapper;
using System.Collections.Generic;
using System.Linq;
using Votacao.Data;
using Votacao.Service.DTO;
using Votacao.Service.Interface;

namespace Votacao.Service
{
    public class CandidateDAO : ICandidate
    {

        #region Public Methods

        public Candidate GetValidByNumber(int number, int regionId)
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                //cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                cnn.Open();
                return cnn.Query<Candidate>(@"  SELECT      cand.id Id,
                                                            name Name,
                                                            nickname Nickname,
                                                            data Picture
                                                FROM        candidate cand
                                                LEFT JOIN   candidate_picture candpic
                                                ON          cand.id = candpic.candidate_id
                                                WHERE       cand.number = @number
                                                AND         cand.region_id = @regionId",
                                                new
                                                {
                                                    number,
                                                    regionId
                                                }).FirstOrDefault();
            }
        }

        public List<Candidate> ListCandidate(int regionId)
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                //cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                cnn.Open();
                return cnn.Query<Candidate>(@"  SELECT  id Id, 
                                                        name Name, 
                                                        region_id RegionId, 
                                                        number Number 
                                                FROM    candidate 
                                                WHERE   region_id = @regionId",
                                                new { regionId }).ToList();
            }
        }

        public int GetMaxSizeCandidateNumber()
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                //cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                cnn.Open();
                return cnn.Query<int>(@"SELECT  LENGTH(number) 
                                        FROM    candidate 
                                        WHERE   number IS NOT NULL 
                                        ORDER BY 1 DESC").FirstOrDefault();
            }
        }

        #endregion

    }
}
